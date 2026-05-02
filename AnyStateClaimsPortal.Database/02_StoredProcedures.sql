USE [AnyStateClaimsDB]
GO

-- ============================================================
-- usp_CalculateWeeklyBenefit
-- Complex benefit calculation with multiple tiers
-- ============================================================
IF OBJECT_ID('dbo.usp_CalculateWeeklyBenefit', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CalculateWeeklyBenefit
GO
CREATE PROCEDURE dbo.usp_CalculateWeeklyBenefit
    @EmployeeId     INT,
    @InjuryType     VARCHAR(50),
    @WeeklyBenefit  DECIMAL(10,2) OUTPUT,
    @BenefitRate    DECIMAL(5,4) OUTPUT,
    @EffectiveDate  DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @EffectiveDate IS NULL SET @EffectiveDate = GETDATE()

    DECLARE @AnnualSalary DECIMAL(12,2), @WeeklySalary DECIMAL(10,2)
    DECLARE @Dependents INT, @HireDate DATE, @YearsOfService INT
    DECLARE @MaxBenefit DECIMAL(10,2), @MinBenefit DECIMAL(10,2)
    DECLARE @StateAvgWeekly DECIMAL(10,2), @DependentSupplement DECIMAL(5,4)
    DECLARE @LongevityBonus DECIMAL(5,4)

    -- Get state average weekly wage from config
    SELECT @StateAvgWeekly = CAST(ConfigValue AS DECIMAL(10,2))
    FROM dbo.SystemConfiguration WHERE ConfigKey = 'StateAverageWeeklyWage'
    IF @StateAvgWeekly IS NULL SET @StateAvgWeekly = 1025.00

    SET @MaxBenefit = @StateAvgWeekly * 0.90
    SET @MinBenefit = @StateAvgWeekly * 0.20

    SELECT @AnnualSalary = e.AnnualSalary, @Dependents = e.NumberOfDependents,
           @HireDate = e.HireDate
    FROM dbo.Employees e WHERE e.EmployeeId = @EmployeeId AND e.IsActive = 1

    IF @AnnualSalary IS NULL BEGIN SET @WeeklyBenefit = 0; SET @BenefitRate = 0; RETURN END

    SET @WeeklySalary = @AnnualSalary / 52.0
    SET @YearsOfService = DATEDIFF(YEAR, @HireDate, @EffectiveDate)

    -- Base rate by injury type
    SET @BenefitRate = CASE @InjuryType
        WHEN 'Medical'   THEN 0.6000
        WHEN 'Temporary' THEN 0.6667
        WHEN 'Permanent' THEN 0.7000
        WHEN 'Fatal'     THEN 0.7500
        ELSE 0.6000
    END

    -- Dependent supplement: 2% per dependent, max 10%
    SET @DependentSupplement = CASE WHEN @Dependents > 5 THEN 0.1000 ELSE @Dependents * 0.0200 END

    -- Longevity bonus: 0.5% per 5 years of service, max 2%
    SET @LongevityBonus = CASE
        WHEN @YearsOfService >= 20 THEN 0.0200
        WHEN @YearsOfService >= 15 THEN 0.0150
        WHEN @YearsOfService >= 10 THEN 0.0100
        WHEN @YearsOfService >= 5  THEN 0.0050
        ELSE 0.0000
    END

    SET @BenefitRate = @BenefitRate + @DependentSupplement + @LongevityBonus
    SET @WeeklyBenefit = ROUND(@WeeklySalary * @BenefitRate, 2)

    IF @WeeklyBenefit > @MaxBenefit SET @WeeklyBenefit = @MaxBenefit
    IF @WeeklyBenefit < @MinBenefit SET @WeeklyBenefit = @MinBenefit
END
GO

-- ============================================================
-- usp_GetClaimsDashboard
-- Returns multiple result sets for dashboard
-- ============================================================
IF OBJECT_ID('dbo.usp_GetClaimsDashboard', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetClaimsDashboard
GO
CREATE PROCEDURE dbo.usp_GetClaimsDashboard
    @UserId INT = NULL,
    @UserRole VARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Result Set 1: Status summary
    SELECT c.Status, COUNT(*) AS ClaimCount,
        ISNULL(SUM(c.WeeklyBenefitAmount), 0) AS TotalWeeklyBenefits,
        ISNULL(SUM(c.TotalPaidAmount), 0) AS TotalPaid,
        ISNULL(SUM(c.TotalReserveAmount), 0) AS TotalReserves
    FROM dbo.Claims c
    GROUP BY c.Status

    -- Result Set 2: Recent claims (last 30 days)
    SELECT TOP 15 c.ClaimId, c.ClaimNumber, c.InjuryDate, c.InjuryType,
        c.Status, c.Priority,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        a.AgencyName, a.AgencyCode, c.CreatedDate
    FROM dbo.Claims c
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    INNER JOIN dbo.Agencies a ON e.AgencyId = a.AgencyId
    WHERE c.CreatedDate >= DATEADD(DAY, -30, GETDATE())
    ORDER BY c.CreatedDate DESC

    -- Result Set 3: Claims by agency
    SELECT a.AgencyName, a.AgencyCode, a.RiskCategory,
        COUNT(c.ClaimId) AS ClaimCount,
        ISNULL(SUM(c.TotalPaidAmount), 0) AS TotalPaid,
        ISNULL(SUM(c.TotalMedicalCost), 0) AS TotalMedical
    FROM dbo.Agencies a
    LEFT JOIN dbo.Employees e ON a.AgencyId = e.AgencyId
    LEFT JOIN dbo.Claims c ON e.EmployeeId = c.EmployeeId
    WHERE a.IsActive = 1
    GROUP BY a.AgencyName, a.AgencyCode, a.RiskCategory
    ORDER BY ClaimCount DESC

    -- Result Set 4: Monthly trend (last 12 months)
    SELECT
        YEAR(c.InjuryDate) AS ClaimYear,
        MONTH(c.InjuryDate) AS ClaimMonth,
        COUNT(*) AS ClaimCount,
        ISNULL(SUM(c.TotalPaidAmount), 0) AS TotalPaid
    FROM dbo.Claims c
    WHERE c.InjuryDate >= DATEADD(MONTH, -12, GETDATE())
    GROUP BY YEAR(c.InjuryDate), MONTH(c.InjuryDate)
    ORDER BY ClaimYear, ClaimMonth

    -- Result Set 5: Top injury types
    SELECT TOP 10 c.InjuryType, c.BodyPartAffected,
        COUNT(*) AS Occurrences,
        ISNULL(AVG(c.WeeklyBenefitAmount), 0) AS AvgBenefit
    FROM dbo.Claims c
    GROUP BY c.InjuryType, c.BodyPartAffected
    ORDER BY Occurrences DESC
END
GO

-- ============================================================
-- usp_SearchClaims
-- Advanced search with pagination
-- ============================================================
IF OBJECT_ID('dbo.usp_SearchClaims', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_SearchClaims
GO
CREATE PROCEDURE dbo.usp_SearchClaims
    @SearchTerm     VARCHAR(100) = NULL,
    @Status         VARCHAR(20)  = NULL,
    @AgencyId       INT          = NULL,
    @InjuryType     VARCHAR(50)  = NULL,
    @Priority       VARCHAR(10)  = NULL,
    @DateFrom       DATE         = NULL,
    @DateTo         DATE         = NULL,
    @AdjusterId     INT          = NULL,
    @IsLitigated    BIT          = NULL,
    @PageNumber     INT          = 1,
    @PageSize       INT          = 25,
    @SortColumn     VARCHAR(50)  = 'CreatedDate',
    @SortDirection  VARCHAR(4)   = 'DESC',
    @TotalCount     INT          OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Get total count
    SELECT @TotalCount = COUNT(*)
    FROM dbo.Claims c
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    INNER JOIN dbo.Agencies a ON e.AgencyId = a.AgencyId
    WHERE (@SearchTerm IS NULL
            OR c.ClaimNumber LIKE '%' + @SearchTerm + '%'
            OR e.FirstName LIKE '%' + @SearchTerm + '%'
            OR e.LastName LIKE '%' + @SearchTerm + '%'
            OR e.EmployeeNumber LIKE '%' + @SearchTerm + '%')
        AND (@Status IS NULL OR c.Status = @Status)
        AND (@AgencyId IS NULL OR e.AgencyId = @AgencyId)
        AND (@InjuryType IS NULL OR c.InjuryType = @InjuryType)
        AND (@Priority IS NULL OR c.Priority = @Priority)
        AND (@DateFrom IS NULL OR c.InjuryDate >= @DateFrom)
        AND (@DateTo IS NULL OR c.InjuryDate <= @DateTo)
        AND (@AdjusterId IS NULL OR c.AssignedAdjusterId = @AdjusterId)
        AND (@IsLitigated IS NULL OR c.IsLitigated = @IsLitigated)

    -- Return paged results
    SELECT c.ClaimId, c.ClaimNumber, c.InjuryDate, c.ReportedDate,
        c.InjuryType, c.BodyPartAffected, c.Status, c.Priority,
        c.WeeklyBenefitAmount, c.TotalPaidAmount, c.TotalMedicalCost,
        c.IsLitigated, c.CreatedDate,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        e.EmployeeNumber, a.AgencyName, a.AgencyCode,
        adj.FullName AS AdjusterName
    FROM dbo.Claims c
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    INNER JOIN dbo.Agencies a ON e.AgencyId = a.AgencyId
    LEFT JOIN dbo.Users adj ON c.AssignedAdjusterId = adj.UserId
    WHERE (@SearchTerm IS NULL
            OR c.ClaimNumber LIKE '%' + @SearchTerm + '%'
            OR e.FirstName LIKE '%' + @SearchTerm + '%'
            OR e.LastName LIKE '%' + @SearchTerm + '%'
            OR e.EmployeeNumber LIKE '%' + @SearchTerm + '%')
        AND (@Status IS NULL OR c.Status = @Status)
        AND (@AgencyId IS NULL OR e.AgencyId = @AgencyId)
        AND (@InjuryType IS NULL OR c.InjuryType = @InjuryType)
        AND (@Priority IS NULL OR c.Priority = @Priority)
        AND (@DateFrom IS NULL OR c.InjuryDate >= @DateFrom)
        AND (@DateTo IS NULL OR c.InjuryDate <= @DateTo)
        AND (@AdjusterId IS NULL OR c.AssignedAdjusterId = @AdjusterId)
        AND (@IsLitigated IS NULL OR c.IsLitigated = @IsLitigated)
    ORDER BY
        CASE WHEN @SortColumn = 'ClaimNumber' AND @SortDirection = 'ASC' THEN c.ClaimNumber END ASC,
        CASE WHEN @SortColumn = 'ClaimNumber' AND @SortDirection = 'DESC' THEN c.ClaimNumber END DESC,
        CASE WHEN @SortColumn = 'InjuryDate' AND @SortDirection = 'ASC' THEN c.InjuryDate END ASC,
        CASE WHEN @SortColumn = 'InjuryDate' AND @SortDirection = 'DESC' THEN c.InjuryDate END DESC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortDirection = 'ASC' THEN c.CreatedDate END ASC,
        CASE WHEN @SortColumn = 'CreatedDate' AND @SortDirection = 'DESC' THEN c.CreatedDate END DESC,
        c.CreatedDate DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO

-- ============================================================
-- usp_GetClaimsByStatus
-- ============================================================
IF OBJECT_ID('dbo.usp_GetClaimsByStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetClaimsByStatus
GO
CREATE PROCEDURE dbo.usp_GetClaimsByStatus @Status VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.ClaimId, c.ClaimNumber, c.InjuryDate, c.ReportedDate,
        c.InjuryDescription, c.InjuryType, c.BodyPartAffected, c.Status, c.Priority,
        c.WeeklyBenefitAmount, c.TotalPaidAmount, c.TotalMedicalCost, c.CreatedDate,
        e.FirstName + ' ' + e.LastName AS EmployeeName, e.EmployeeNumber,
        a.AgencyName, a.AgencyCode, adj.FullName AS AdjusterName
    FROM dbo.Claims c
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    INNER JOIN dbo.Agencies a ON e.AgencyId = a.AgencyId
    LEFT JOIN dbo.Users adj ON c.AssignedAdjusterId = adj.UserId
    WHERE (@Status IS NULL OR c.Status = @Status)
    ORDER BY c.CreatedDate DESC
END
GO

-- ============================================================
-- usp_GetClaimsByAgency
-- ============================================================
IF OBJECT_ID('dbo.usp_GetClaimsByAgency', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetClaimsByAgency
GO
CREATE PROCEDURE dbo.usp_GetClaimsByAgency @AgencyId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.ClaimId, c.ClaimNumber, c.InjuryDate, c.InjuryType, c.Status, c.Priority,
        c.WeeklyBenefitAmount, c.TotalPaidAmount, c.TotalMedicalCost,
        e.FirstName + ' ' + e.LastName AS EmployeeName, e.EmployeeNumber, e.JobTitle
    FROM dbo.Claims c
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    WHERE e.AgencyId = @AgencyId
    ORDER BY c.InjuryDate DESC
END
GO

-- ============================================================
-- usp_GetClaimFinancialSummary
-- Complex financial reporting
-- ============================================================
IF OBJECT_ID('dbo.usp_GetClaimFinancialSummary', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetClaimFinancialSummary
GO
CREATE PROCEDURE dbo.usp_GetClaimFinancialSummary
    @FiscalYear INT = NULL,
    @AgencyId   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @FiscalYear IS NULL SET @FiscalYear = YEAR(GETDATE())

    -- Payment summary by type
    SELECT p.PaymentType, COUNT(*) AS PaymentCount,
        SUM(p.Amount) AS TotalAmount, AVG(p.Amount) AS AvgAmount,
        MIN(p.Amount) AS MinAmount, MAX(p.Amount) AS MaxAmount
    FROM dbo.ClaimPayments p
    INNER JOIN dbo.Claims c ON p.ClaimId = c.ClaimId
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    WHERE p.FiscalYear = @FiscalYear
        AND (@AgencyId IS NULL OR e.AgencyId = @AgencyId)
        AND p.Status <> 'Voided'
    GROUP BY p.PaymentType

    -- Monthly payment trend
    SELECT MONTH(p.PaymentDate) AS PaymentMonth,
        SUM(CASE WHEN p.PaymentType = 'WeeklyBenefit' THEN p.Amount ELSE 0 END) AS BenefitPayments,
        SUM(CASE WHEN p.PaymentType = 'Medical' THEN p.Amount ELSE 0 END) AS MedicalPayments,
        SUM(CASE WHEN p.PaymentType NOT IN ('WeeklyBenefit','Medical') THEN p.Amount ELSE 0 END) AS OtherPayments,
        SUM(p.Amount) AS TotalPayments
    FROM dbo.ClaimPayments p
    INNER JOIN dbo.Claims c ON p.ClaimId = c.ClaimId
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    WHERE p.FiscalYear = @FiscalYear
        AND (@AgencyId IS NULL OR e.AgencyId = @AgencyId)
        AND p.Status <> 'Voided'
    GROUP BY MONTH(p.PaymentDate)
    ORDER BY PaymentMonth

    -- Agency cost comparison
    SELECT a.AgencyCode, a.AgencyName, a.RiskCategory,
        COUNT(DISTINCT c.ClaimId) AS ClaimCount,
        ISNULL(SUM(p.Amount), 0) AS TotalPayments,
        ISNULL(a.AnnualPremium, 0) AS AnnualPremium,
        CASE WHEN a.AnnualPremium > 0
            THEN ROUND(ISNULL(SUM(p.Amount), 0) / a.AnnualPremium * 100, 1)
            ELSE 0 END AS LossRatio
    FROM dbo.Agencies a
    LEFT JOIN dbo.Employees e ON a.AgencyId = e.AgencyId
    LEFT JOIN dbo.Claims c ON e.EmployeeId = c.EmployeeId
    LEFT JOIN dbo.ClaimPayments p ON c.ClaimId = p.ClaimId AND p.FiscalYear = @FiscalYear AND p.Status <> 'Voided'
    WHERE a.IsActive = 1
    GROUP BY a.AgencyCode, a.AgencyName, a.RiskCategory, a.AnnualPremium
    ORDER BY TotalPayments DESC
END
GO

-- ============================================================
-- usp_GetMedicalTreatmentHistory
-- ============================================================
IF OBJECT_ID('dbo.usp_GetMedicalTreatmentHistory', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetMedicalTreatmentHistory
GO
CREATE PROCEDURE dbo.usp_GetMedicalTreatmentHistory @ClaimId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TreatmentId, t.TreatmentDate, t.TreatmentType, t.Description,
        t.Diagnosis, t.ICDCode, t.CPTCode, t.BilledAmount, t.ApprovedAmount,
        t.Status, t.Notes, t.NextAppointment,
        p.ProviderName, p.ProviderType, p.Phone AS ProviderPhone
    FROM dbo.MedicalTreatments t
    INNER JOIN dbo.MedicalProviders p ON t.ProviderId = p.ProviderId
    WHERE t.ClaimId = @ClaimId
    ORDER BY t.TreatmentDate DESC
END
GO

-- ============================================================
-- usp_ProcessPaymentBatch
-- Batch payment processing
-- ============================================================
IF OBJECT_ID('dbo.usp_ProcessPaymentBatch', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_ProcessPaymentBatch
GO
CREATE PROCEDURE dbo.usp_ProcessPaymentBatch
    @ApprovedBy VARCHAR(50),
    @ProcessedCount INT OUTPUT,
    @TotalAmount DECIMAL(12,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @ProcessedCount = 0
    SET @TotalAmount = 0

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE dbo.ClaimPayments
        SET Status = 'Issued', ApprovedBy = @ApprovedBy, ApprovedDate = GETDATE()
        WHERE Status = 'Approved'

        SET @ProcessedCount = @@ROWCOUNT

        SELECT @TotalAmount = ISNULL(SUM(Amount), 0)
        FROM dbo.ClaimPayments
        WHERE Status = 'Issued' AND ApprovedBy = @ApprovedBy
            AND ApprovedDate >= CAST(GETDATE() AS DATE)

        -- Update claim totals
        UPDATE c SET
            c.TotalPaidAmount = sub.TotalPaid,
            c.TotalMedicalCost = sub.TotalMedical,
            c.ModifiedDate = GETDATE()
        FROM dbo.Claims c
        INNER JOIN (
            SELECT p.ClaimId,
                SUM(CASE WHEN p.Status = 'Issued' THEN p.Amount ELSE 0 END) AS TotalPaid,
                SUM(CASE WHEN p.Status = 'Issued' AND p.PaymentType = 'Medical' THEN p.Amount ELSE 0 END) AS TotalMedical
            FROM dbo.ClaimPayments p
            GROUP BY p.ClaimId
        ) sub ON c.ClaimId = sub.ClaimId

        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO

PRINT 'All stored procedures created successfully.'
GO
