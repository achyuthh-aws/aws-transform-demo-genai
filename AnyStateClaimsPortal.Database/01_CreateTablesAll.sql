-- AnyState Workers' Compensation Claims Portal
-- Database Creation and Schema Setup
-- Target: SQL Server 2019

USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'AnyStateClaimsDB')
    CREATE DATABASE [AnyStateClaimsDB]
GO

USE [AnyStateClaimsDB]
GO

-- ============================================================
-- Table: Agencies
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Agencies') AND type = 'U')
CREATE TABLE dbo.Agencies (
    AgencyId        INT IDENTITY(1,1) NOT NULL,
    AgencyCode      VARCHAR(10)       NOT NULL,
    AgencyName      VARCHAR(200)      NOT NULL,
    Division        VARCHAR(200)      NULL,
    AgencyType      VARCHAR(50)       NOT NULL DEFAULT 'State',  -- State, Education, Municipal
    ContactName     VARCHAR(100)      NULL,
    ContactEmail    VARCHAR(200)      NULL,
    ContactPhone    VARCHAR(20)       NULL,
    Address         VARCHAR(300)      NULL,
    City            VARCHAR(100)      NULL,
    State           VARCHAR(2)        NOT NULL DEFAULT 'AS',
    ZipCode         VARCHAR(10)       NULL,
    RiskCategory    VARCHAR(20)       NOT NULL DEFAULT 'Standard', -- Low, Standard, High, Critical
    AnnualPremium   DECIMAL(12,2)     NULL,
    IsActive        BIT               NOT NULL DEFAULT 1,
    CreatedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    ModifiedDate    DATETIME          NULL,
    CONSTRAINT PK_Agencies PRIMARY KEY CLUSTERED (AgencyId),
    CONSTRAINT UQ_Agencies_Code UNIQUE (AgencyCode),
    CONSTRAINT CK_Agencies_Type CHECK (AgencyType IN ('State','Education','Municipal')),
    CONSTRAINT CK_Agencies_Risk CHECK (RiskCategory IN ('Low','Standard','High','Critical'))
)
GO

-- ============================================================
-- Table: Employees
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Employees') AND type = 'U')
CREATE TABLE dbo.Employees (
    EmployeeId          INT IDENTITY(1,1) NOT NULL,
    EmployeeNumber      VARCHAR(20)       NOT NULL,
    FirstName           VARCHAR(50)       NOT NULL,
    LastName            VARCHAR(50)       NOT NULL,
    MiddleInitial       CHAR(1)           NULL,
    DateOfBirth         DATE              NOT NULL,
    SSNLast4            CHAR(4)           NULL,
    HireDate            DATE              NOT NULL,
    TerminationDate     DATE              NULL,
    JobTitle            VARCHAR(100)      NOT NULL,
    JobClassCode        VARCHAR(10)       NULL,
    Department          VARCHAR(100)      NULL,
    AnnualSalary        DECIMAL(12,2)     NOT NULL,
    HourlyRate          DECIMAL(8,2)      NULL,
    PayFrequency        VARCHAR(20)       NOT NULL DEFAULT 'Biweekly', -- Weekly, Biweekly, Monthly
    AgencyId            INT               NOT NULL,
    SupervisorName      VARCHAR(100)      NULL,
    SupervisorEmail     VARCHAR(200)      NULL,
    NumberOfDependents  INT               NOT NULL DEFAULT 0,
    EmergencyContactName  VARCHAR(100)    NULL,
    EmergencyContactPhone VARCHAR(20)     NULL,
    IsActive            BIT               NOT NULL DEFAULT 1,
    CreatedDate         DATETIME          NOT NULL DEFAULT GETDATE(),
    ModifiedDate        DATETIME          NULL,
    CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED (EmployeeId),
    CONSTRAINT UQ_Employees_Number UNIQUE (EmployeeNumber),
    CONSTRAINT FK_Employees_Agencies FOREIGN KEY (AgencyId) REFERENCES dbo.Agencies(AgencyId)
)
GO

CREATE NONCLUSTERED INDEX IX_Employees_Agency ON dbo.Employees (AgencyId) INCLUDE (FirstName, LastName, EmployeeNumber)
GO

-- ============================================================
-- Table: InjuryCodes (lookup table)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.InjuryCodes') AND type = 'U')
CREATE TABLE dbo.InjuryCodes (
    InjuryCodeId    INT IDENTITY(1,1) NOT NULL,
    Code            VARCHAR(10)       NOT NULL,
    Description     VARCHAR(200)      NOT NULL,
    Category        VARCHAR(50)       NOT NULL,  -- Trauma, Occupational, Repetitive, Environmental
    Severity        VARCHAR(20)       NOT NULL DEFAULT 'Moderate', -- Minor, Moderate, Severe, Critical
    TypicalDuration INT               NULL,  -- typical recovery days
    IsActive        BIT               NOT NULL DEFAULT 1,
    CONSTRAINT PK_InjuryCodes PRIMARY KEY CLUSTERED (InjuryCodeId),
    CONSTRAINT UQ_InjuryCodes_Code UNIQUE (Code)
)
GO

-- ============================================================
-- Table: BodyPartCodes (lookup table)
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.BodyPartCodes') AND type = 'U')
CREATE TABLE dbo.BodyPartCodes (
    BodyPartCodeId  INT IDENTITY(1,1) NOT NULL,
    Code            VARCHAR(10)       NOT NULL,
    Description     VARCHAR(100)      NOT NULL,
    BodyRegion      VARCHAR(50)       NOT NULL,  -- Head, Torso, UpperExtremity, LowerExtremity, Multiple
    CONSTRAINT PK_BodyPartCodes PRIMARY KEY CLUSTERED (BodyPartCodeId),
    CONSTRAINT UQ_BodyPartCodes_Code UNIQUE (Code)
)
GO

-- ============================================================
-- Table: Users
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Users') AND type = 'U')
CREATE TABLE dbo.Users (
    UserId          INT IDENTITY(1,1) NOT NULL,
    Username        VARCHAR(50)       NOT NULL,
    PasswordHash    VARCHAR(256)      NOT NULL,
    Salt            VARCHAR(64)       NULL,
    FullName        VARCHAR(100)      NOT NULL,
    Email           VARCHAR(200)      NOT NULL,
    Phone           VARCHAR(20)       NULL,
    Role            VARCHAR(30)       NOT NULL,  -- Administrator, ClaimsAdjuster, AgencyStaff, MedicalReviewer, ReadOnly
    AgencyId        INT               NULL,
    IsActive        BIT               NOT NULL DEFAULT 1,
    IsLocked        BIT               NOT NULL DEFAULT 0,
    FailedLoginAttempts INT           NOT NULL DEFAULT 0,
    LastLoginDate   DATETIME          NULL,
    PasswordChangedDate DATETIME      NULL,
    CreatedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    ModifiedDate    DATETIME          NULL,
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT FK_Users_Agencies FOREIGN KEY (AgencyId) REFERENCES dbo.Agencies(AgencyId),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Administrator','ClaimsAdjuster','AgencyStaff','MedicalReviewer','ReadOnly'))
)
GO

-- ============================================================
-- Table: Claims
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Claims') AND type = 'U')
CREATE TABLE dbo.Claims (
    ClaimId                 INT IDENTITY(1,1) NOT NULL,
    ClaimNumber             VARCHAR(20)       NOT NULL,
    EmployeeId              INT               NOT NULL,
    InjuryDate              DATE              NOT NULL,
    InjuryTime              TIME              NULL,
    ReportedDate            DATE              NOT NULL,
    InjuryDescription       VARCHAR(4000)     NOT NULL,
    InjuryCodeId            INT               NULL,
    BodyPartCodeId          INT               NULL,
    InjuryType              VARCHAR(50)       NOT NULL,  -- Medical, Temporary, Permanent, Fatal
    BodyPartAffected        VARCHAR(100)      NOT NULL,
    IncidentLocation        VARCHAR(200)      NOT NULL,
    LocationType            VARCHAR(50)       NULL,  -- Office, Field, Vehicle, Construction, Other
    WitnessName             VARCHAR(100)      NULL,
    WitnessPhone            VARCHAR(20)       NULL,
    WitnessStatement        VARCHAR(2000)     NULL,
    EmployerNotifiedDate    DATE              NULL,
    Status                  VARCHAR(20)       NOT NULL DEFAULT 'Submitted',
    Priority                VARCHAR(10)       NOT NULL DEFAULT 'Normal', -- Low, Normal, High, Urgent
    AssignedAdjusterId      INT               NULL,
    MedicalReviewerId       INT               NULL,
    WeeklyBenefitAmount     DECIMAL(10,2)     NULL,
    TotalPaidAmount         DECIMAL(12,2)     NOT NULL DEFAULT 0,
    TotalMedicalCost        DECIMAL(12,2)     NOT NULL DEFAULT 0,
    TotalReserveAmount      DECIMAL(12,2)     NOT NULL DEFAULT 0,
    DenialReason            VARCHAR(500)      NULL,
    ReturnToWorkDate        DATE              NULL,
    MaxMedicalImprovementDate DATE            NULL,
    ClosedDate              DATE              NULL,
    ReopenedDate            DATE              NULL,
    IsLitigated             BIT               NOT NULL DEFAULT 0,
    AttorneyName            VARCHAR(100)      NULL,
    AttorneyPhone           VARCHAR(20)       NULL,
    CreatedBy               VARCHAR(50)       NOT NULL DEFAULT SYSTEM_USER,
    CreatedDate             DATETIME          NOT NULL DEFAULT GETDATE(),
    ModifiedDate            DATETIME          NULL,
    ModifiedBy              VARCHAR(50)       NULL,
    CONSTRAINT PK_Claims PRIMARY KEY CLUSTERED (ClaimId),
    CONSTRAINT UQ_Claims_Number UNIQUE (ClaimNumber),
    CONSTRAINT FK_Claims_Employees FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(EmployeeId),
    CONSTRAINT FK_Claims_InjuryCodes FOREIGN KEY (InjuryCodeId) REFERENCES dbo.InjuryCodes(InjuryCodeId),
    CONSTRAINT FK_Claims_BodyPartCodes FOREIGN KEY (BodyPartCodeId) REFERENCES dbo.BodyPartCodes(BodyPartCodeId),
    CONSTRAINT FK_Claims_Adjuster FOREIGN KEY (AssignedAdjusterId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Claims_MedReviewer FOREIGN KEY (MedicalReviewerId) REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_Claims_Status CHECK (Status IN ('Submitted','UnderReview','MedicalReview','Approved','Denied','Closed','Reopened')),
    CONSTRAINT CK_Claims_InjuryType CHECK (InjuryType IN ('Medical','Temporary','Permanent','Fatal')),
    CONSTRAINT CK_Claims_Priority CHECK (Priority IN ('Low','Normal','High','Urgent'))
)
GO

CREATE NONCLUSTERED INDEX IX_Claims_Status ON dbo.Claims (Status) INCLUDE (ClaimNumber, EmployeeId, InjuryDate)
GO
CREATE NONCLUSTERED INDEX IX_Claims_Employee ON dbo.Claims (EmployeeId) INCLUDE (ClaimNumber, Status)
GO
CREATE NONCLUSTERED INDEX IX_Claims_Adjuster ON dbo.Claims (AssignedAdjusterId) INCLUDE (ClaimNumber, Status)
GO
CREATE NONCLUSTERED INDEX IX_Claims_Date ON dbo.Claims (InjuryDate) INCLUDE (ClaimNumber, Status, EmployeeId)
GO

-- ============================================================
-- Table: ClaimNotes
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.ClaimNotes') AND type = 'U')
CREATE TABLE dbo.ClaimNotes (
    NoteId          INT IDENTITY(1,1) NOT NULL,
    ClaimId         INT               NOT NULL,
    NoteText        VARCHAR(4000)     NOT NULL,
    NoteType        VARCHAR(30)       NOT NULL DEFAULT 'General',
    IsConfidential  BIT               NOT NULL DEFAULT 0,
    CreatedBy       VARCHAR(50)       NOT NULL,
    CreatedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_ClaimNotes PRIMARY KEY CLUSTERED (NoteId),
    CONSTRAINT FK_ClaimNotes_Claims FOREIGN KEY (ClaimId) REFERENCES dbo.Claims(ClaimId),
    CONSTRAINT CK_ClaimNotes_Type CHECK (NoteType IN ('General','StatusChange','Medical','Payment','Legal','Internal'))
)
GO

-- ============================================================
-- Table: ClaimStatusHistory
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.ClaimStatusHistory') AND type = 'U')
CREATE TABLE dbo.ClaimStatusHistory (
    HistoryId       INT IDENTITY(1,1) NOT NULL,
    ClaimId         INT               NOT NULL,
    OldStatus       VARCHAR(20)       NULL,
    NewStatus       VARCHAR(20)       NOT NULL,
    ChangedBy       VARCHAR(50)       NOT NULL,
    ChangedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    Remarks         VARCHAR(500)      NULL,
    IPAddress       VARCHAR(45)       NULL,
    CONSTRAINT PK_ClaimStatusHistory PRIMARY KEY CLUSTERED (HistoryId),
    CONSTRAINT FK_ClaimStatusHistory_Claims FOREIGN KEY (ClaimId) REFERENCES dbo.Claims(ClaimId)
)
GO

-- ============================================================
-- Table: ClaimDocuments
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.ClaimDocuments') AND type = 'U')
CREATE TABLE dbo.ClaimDocuments (
    DocumentId      INT IDENTITY(1,1) NOT NULL,
    ClaimId         INT               NOT NULL,
    FileName        VARCHAR(255)      NOT NULL,
    FileType        VARCHAR(50)       NOT NULL,
    FileSizeBytes   BIGINT            NOT NULL,
    DocumentType    VARCHAR(50)       NOT NULL,  -- MedicalRecord, IncidentReport, WitnessStatement, LegalDocument, Photo, Other
    Description     VARCHAR(500)      NULL,
    StoragePath     VARCHAR(500)      NOT NULL,
    UploadedBy      VARCHAR(50)       NOT NULL,
    UploadedDate    DATETIME          NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_ClaimDocuments PRIMARY KEY CLUSTERED (DocumentId),
    CONSTRAINT FK_ClaimDocuments_Claims FOREIGN KEY (ClaimId) REFERENCES dbo.Claims(ClaimId)
)
GO

-- ============================================================
-- Table: ClaimPayments
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.ClaimPayments') AND type = 'U')
CREATE TABLE dbo.ClaimPayments (
    PaymentId       INT IDENTITY(1,1) NOT NULL,
    ClaimId         INT               NOT NULL,
    PaymentDate     DATE              NOT NULL,
    PaymentType     VARCHAR(30)       NOT NULL,  -- WeeklyBenefit, Medical, Rehabilitation, Legal, Settlement
    Amount          DECIMAL(10,2)     NOT NULL,
    CheckNumber     VARCHAR(20)       NULL,
    PayeeName       VARCHAR(100)      NOT NULL,
    PayeeType       VARCHAR(30)       NOT NULL DEFAULT 'Employee', -- Employee, Provider, Attorney, Other
    Description     VARCHAR(500)      NULL,
    VoucherNumber   VARCHAR(20)       NULL,
    FiscalYear      INT               NOT NULL,
    ApprovedBy      VARCHAR(50)       NULL,
    ApprovedDate    DATETIME          NULL,
    Status          VARCHAR(20)       NOT NULL DEFAULT 'Pending', -- Pending, Approved, Issued, Voided
    CreatedBy       VARCHAR(50)       NOT NULL DEFAULT SYSTEM_USER,
    CreatedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_ClaimPayments PRIMARY KEY CLUSTERED (PaymentId),
    CONSTRAINT FK_ClaimPayments_Claims FOREIGN KEY (ClaimId) REFERENCES dbo.Claims(ClaimId),
    CONSTRAINT CK_ClaimPayments_Type CHECK (PaymentType IN ('WeeklyBenefit','Medical','Rehabilitation','Legal','Settlement')),
    CONSTRAINT CK_ClaimPayments_Status CHECK (Status IN ('Pending','Approved','Issued','Voided'))
)
GO

-- ============================================================
-- Table: MedicalProviders
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.MedicalProviders') AND type = 'U')
CREATE TABLE dbo.MedicalProviders (
    ProviderId      INT IDENTITY(1,1) NOT NULL,
    ProviderName    VARCHAR(200)      NOT NULL,
    ProviderType    VARCHAR(50)       NOT NULL,  -- Hospital, Clinic, Specialist, PhysicalTherapy, Pharmacy
    NPI             VARCHAR(10)       NULL,
    TaxId           VARCHAR(10)       NULL,
    Address         VARCHAR(300)      NULL,
    City            VARCHAR(100)      NULL,
    State           VARCHAR(2)        NULL,
    ZipCode         VARCHAR(10)       NULL,
    Phone           VARCHAR(20)       NULL,
    Fax             VARCHAR(20)       NULL,
    IsPreferred     BIT               NOT NULL DEFAULT 0,
    IsActive        BIT               NOT NULL DEFAULT 1,
    CreatedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_MedicalProviders PRIMARY KEY CLUSTERED (ProviderId)
)
GO

-- ============================================================
-- Table: MedicalTreatments
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.MedicalTreatments') AND type = 'U')
CREATE TABLE dbo.MedicalTreatments (
    TreatmentId     INT IDENTITY(1,1) NOT NULL,
    ClaimId         INT               NOT NULL,
    ProviderId      INT               NOT NULL,
    TreatmentDate   DATE              NOT NULL,
    TreatmentType   VARCHAR(50)       NOT NULL,  -- InitialVisit, FollowUp, Surgery, PhysicalTherapy, Diagnostic, Prescription
    Description     VARCHAR(2000)     NOT NULL,
    Diagnosis       VARCHAR(500)      NULL,
    ICDCode         VARCHAR(10)       NULL,
    CPTCode         VARCHAR(10)       NULL,
    BilledAmount    DECIMAL(10,2)     NULL,
    ApprovedAmount  DECIMAL(10,2)     NULL,
    Status          VARCHAR(20)       NOT NULL DEFAULT 'Submitted', -- Submitted, Approved, Denied, Paid
    Notes           VARCHAR(1000)     NULL,
    NextAppointment DATE              NULL,
    CreatedBy       VARCHAR(50)       NOT NULL DEFAULT SYSTEM_USER,
    CreatedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_MedicalTreatments PRIMARY KEY CLUSTERED (TreatmentId),
    CONSTRAINT FK_MedicalTreatments_Claims FOREIGN KEY (ClaimId) REFERENCES dbo.Claims(ClaimId),
    CONSTRAINT FK_MedicalTreatments_Providers FOREIGN KEY (ProviderId) REFERENCES dbo.MedicalProviders(ProviderId)
)
GO

-- ============================================================
-- Table: AuditLog
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AuditLog') AND type = 'U')
CREATE TABLE dbo.AuditLog (
    AuditId         BIGINT IDENTITY(1,1) NOT NULL,
    TableName       VARCHAR(100)      NOT NULL,
    RecordId        INT               NOT NULL,
    Action          VARCHAR(10)       NOT NULL,  -- INSERT, UPDATE, DELETE
    FieldName       VARCHAR(100)      NULL,
    OldValue        VARCHAR(MAX)      NULL,
    NewValue        VARCHAR(MAX)      NULL,
    ChangedBy       VARCHAR(50)       NOT NULL DEFAULT SYSTEM_USER,
    ChangedDate     DATETIME          NOT NULL DEFAULT GETDATE(),
    IPAddress       VARCHAR(45)       NULL,
    CONSTRAINT PK_AuditLog PRIMARY KEY CLUSTERED (AuditId)
)
GO

CREATE NONCLUSTERED INDEX IX_AuditLog_Table ON dbo.AuditLog (TableName, RecordId)
GO
CREATE NONCLUSTERED INDEX IX_AuditLog_Date ON dbo.AuditLog (ChangedDate) INCLUDE (TableName, Action)
GO

-- ============================================================
-- Table: SystemConfiguration
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SystemConfiguration') AND type = 'U')
CREATE TABLE dbo.SystemConfiguration (
    ConfigId        INT IDENTITY(1,1) NOT NULL,
    ConfigKey       VARCHAR(100)      NOT NULL,
    ConfigValue     VARCHAR(500)      NOT NULL,
    Description     VARCHAR(500)      NULL,
    DataType        VARCHAR(20)       NOT NULL DEFAULT 'String',
    Category        VARCHAR(50)       NOT NULL DEFAULT 'General',
    ModifiedBy      VARCHAR(50)       NULL,
    ModifiedDate    DATETIME          NULL,
    CONSTRAINT PK_SystemConfiguration PRIMARY KEY CLUSTERED (ConfigId),
    CONSTRAINT UQ_SystemConfig_Key UNIQUE (ConfigKey)
)
GO

PRINT 'All tables created successfully.'
GO
