USE [AnyStateClaimsDB]
GO

-- ============================================================
-- View: vw_ClaimsSummary
-- ============================================================
IF OBJECT_ID('dbo.vw_ClaimsSummary', 'V') IS NOT NULL DROP VIEW dbo.vw_ClaimsSummary
GO
CREATE VIEW dbo.vw_ClaimsSummary
AS
    SELECT c.ClaimId, c.ClaimNumber, c.InjuryDate, c.ReportedDate,
        c.InjuryType, c.BodyPartAffected, c.Status, c.Priority,
        c.WeeklyBenefitAmount, c.TotalPaidAmount, c.TotalMedicalCost,
        c.TotalReserveAmount, c.DenialReason, c.ClosedDate, c.CreatedDate,
        c.IsLitigated, c.ReturnToWorkDate,
        e.EmployeeNumber, e.FirstName + ' ' + e.LastName AS EmployeeName,
        e.JobTitle, e.AnnualSalary, e.NumberOfDependents, e.Department,
        a.AgencyId, a.AgencyCode, a.AgencyName, a.RiskCategory,
        adj.FullName AS AdjusterName, mr.FullName AS MedicalReviewerName,
        DATEDIFF(DAY, c.InjuryDate, ISNULL(c.ClosedDate, GETDATE())) AS DaysSinceInjury,
        DATEDIFF(DAY, c.ReportedDate, c.InjuryDate) AS ReportingDelay,
        c.TotalPaidAmount + c.TotalMedicalCost AS TotalIncurredCost,
        CASE WHEN c.Status IN ('Submitted','UnderReview','MedicalReview') THEN 1 ELSE 0 END AS IsOpen
    FROM dbo.Claims c
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    INNER JOIN dbo.Agencies a ON e.AgencyId = a.AgencyId
    LEFT JOIN dbo.Users adj ON c.AssignedAdjusterId = adj.UserId
    LEFT JOIN dbo.Users mr ON c.MedicalReviewerId = mr.UserId
GO

-- ============================================================
-- View: vw_AgencyClaimsReport
-- ============================================================
IF OBJECT_ID('dbo.vw_AgencyClaimsReport', 'V') IS NOT NULL DROP VIEW dbo.vw_AgencyClaimsReport
GO
CREATE VIEW dbo.vw_AgencyClaimsReport
AS
    SELECT a.AgencyId, a.AgencyCode, a.AgencyName, a.AgencyType, a.RiskCategory,
        a.AnnualPremium,
        COUNT(c.ClaimId) AS TotalClaims,
        SUM(CASE WHEN c.Status = 'Submitted' THEN 1 ELSE 0 END) AS SubmittedCount,
        SUM(CASE WHEN c.Status = 'UnderReview' THEN 1 ELSE 0 END) AS UnderReviewCount,
        SUM(CASE WHEN c.Status = 'MedicalReview' THEN 1 ELSE 0 END) AS MedicalReviewCount,
        SUM(CASE WHEN c.Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
        SUM(CASE WHEN c.Status = 'Denied' THEN 1 ELSE 0 END) AS DeniedCount,
        SUM(CASE WHEN c.Status = 'Closed' THEN 1 ELSE 0 END) AS ClosedCount,
        SUM(CASE WHEN c.IsLitigated = 1 THEN 1 ELSE 0 END) AS LitigatedCount,
        ISNULL(SUM(c.TotalPaidAmount), 0) AS TotalPaidAmount,
        ISNULL(SUM(c.TotalMedicalCost), 0) AS TotalMedicalCost,
        ISNULL(SUM(c.TotalPaidAmount + c.TotalMedicalCost), 0) AS TotalIncurredCost,
        ISNULL(AVG(c.WeeklyBenefitAmount), 0) AS AvgWeeklyBenefit,
        CASE WHEN a.AnnualPremium > 0
            THEN ROUND(ISNULL(SUM(c.TotalPaidAmount + c.TotalMedicalCost), 0) / a.AnnualPremium * 100, 1)
            ELSE 0 END AS LossRatio
    FROM dbo.Agencies a
    LEFT JOIN dbo.Employees e ON a.AgencyId = e.AgencyId
    LEFT JOIN dbo.Claims c ON e.EmployeeId = c.EmployeeId
    WHERE a.IsActive = 1
    GROUP BY a.AgencyId, a.AgencyCode, a.AgencyName, a.AgencyType, a.RiskCategory, a.AnnualPremium
GO

-- ============================================================
-- View: vw_PaymentSummary
-- ============================================================
IF OBJECT_ID('dbo.vw_PaymentSummary', 'V') IS NOT NULL DROP VIEW dbo.vw_PaymentSummary
GO
CREATE VIEW dbo.vw_PaymentSummary
AS
    SELECT p.PaymentId, p.ClaimId, c.ClaimNumber, p.PaymentDate, p.PaymentType,
        p.Amount, p.CheckNumber, p.PayeeName, p.PayeeType, p.Status AS PaymentStatus,
        p.FiscalYear, p.VoucherNumber,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        a.AgencyName, a.AgencyCode
    FROM dbo.ClaimPayments p
    INNER JOIN dbo.Claims c ON p.ClaimId = c.ClaimId
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    INNER JOIN dbo.Agencies a ON e.AgencyId = a.AgencyId
GO

-- ============================================================
-- View: vw_OpenClaimsAging
-- ============================================================
IF OBJECT_ID('dbo.vw_OpenClaimsAging', 'V') IS NOT NULL DROP VIEW dbo.vw_OpenClaimsAging
GO
CREATE VIEW dbo.vw_OpenClaimsAging
AS
    SELECT c.ClaimId, c.ClaimNumber, c.Status, c.Priority,
        c.InjuryDate, c.CreatedDate,
        DATEDIFF(DAY, c.CreatedDate, GETDATE()) AS DaysOpen,
        CASE
            WHEN DATEDIFF(DAY, c.CreatedDate, GETDATE()) <= 30 THEN '0-30 Days'
            WHEN DATEDIFF(DAY, c.CreatedDate, GETDATE()) <= 60 THEN '31-60 Days'
            WHEN DATEDIFF(DAY, c.CreatedDate, GETDATE()) <= 90 THEN '61-90 Days'
            WHEN DATEDIFF(DAY, c.CreatedDate, GETDATE()) <= 180 THEN '91-180 Days'
            ELSE 'Over 180 Days'
        END AS AgingBucket,
        c.TotalPaidAmount, c.TotalReserveAmount,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        a.AgencyName, adj.FullName AS AdjusterName
    FROM dbo.Claims c
    INNER JOIN dbo.Employees e ON c.EmployeeId = e.EmployeeId
    INNER JOIN dbo.Agencies a ON e.AgencyId = a.AgencyId
    LEFT JOIN dbo.Users adj ON c.AssignedAdjusterId = adj.UserId
    WHERE c.Status NOT IN ('Closed', 'Denied')
GO

-- ============================================================
-- Trigger: trg_ClaimStatusAudit
-- ============================================================
IF OBJECT_ID('dbo.trg_ClaimStatusAudit', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_ClaimStatusAudit
GO
CREATE TRIGGER dbo.trg_ClaimStatusAudit ON dbo.Claims AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Status)
    BEGIN
        INSERT INTO dbo.ClaimStatusHistory (ClaimId, OldStatus, NewStatus, ChangedBy, ChangedDate, Remarks)
        SELECT i.ClaimId, d.Status, i.Status, ISNULL(i.ModifiedBy, SYSTEM_USER), GETDATE(),
            CASE
                WHEN i.Status = 'Denied' THEN i.DenialReason
                WHEN i.Status = 'Closed' THEN 'Claim closed'
                WHEN i.Status = 'Reopened' THEN 'Claim reopened for review'
                ELSE NULL
            END
        FROM inserted i INNER JOIN deleted d ON i.ClaimId = d.ClaimId
        WHERE i.Status <> d.Status
    END

    UPDATE c SET c.ModifiedDate = GETDATE()
    FROM dbo.Claims c INNER JOIN inserted i ON c.ClaimId = i.ClaimId
END
GO

-- ============================================================
-- Trigger: trg_AuditLog_Claims
-- ============================================================
IF OBJECT_ID('dbo.trg_AuditLog_Claims', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_AuditLog_Claims
GO
CREATE TRIGGER dbo.trg_AuditLog_Claims ON dbo.Claims AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.AuditLog (TableName, RecordId, Action, FieldName, NewValue, ChangedBy)
    SELECT 'Claims', i.ClaimId, 'INSERT', 'ClaimNumber', i.ClaimNumber, ISNULL(i.CreatedBy, SYSTEM_USER)
    FROM inserted i
END
GO

-- ============================================================
-- Trigger: trg_Payment_UpdateClaimTotals
-- ============================================================
IF OBJECT_ID('dbo.trg_Payment_UpdateClaimTotals', 'TR') IS NOT NULL DROP TRIGGER dbo.trg_Payment_UpdateClaimTotals
GO
CREATE TRIGGER dbo.trg_Payment_UpdateClaimTotals ON dbo.ClaimPayments AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c SET
        c.TotalPaidAmount = sub.TotalPaid,
        c.TotalMedicalCost = sub.TotalMedical,
        c.ModifiedDate = GETDATE()
    FROM dbo.Claims c
    INNER JOIN (
        SELECT p.ClaimId,
            SUM(CASE WHEN p.Status IN ('Approved','Issued') THEN p.Amount ELSE 0 END) AS TotalPaid,
            SUM(CASE WHEN p.Status IN ('Approved','Issued') AND p.PaymentType = 'Medical' THEN p.Amount ELSE 0 END) AS TotalMedical
        FROM dbo.ClaimPayments p
        WHERE p.ClaimId IN (SELECT DISTINCT ClaimId FROM inserted)
        GROUP BY p.ClaimId
    ) sub ON c.ClaimId = sub.ClaimId
END
GO

PRINT 'All views and triggers created successfully.'
GO
