USE [AnyStateClaimsDB]
GO

-- System Configuration
INSERT INTO dbo.SystemConfiguration (ConfigKey, ConfigValue, Description, DataType, Category) VALUES
('StateAverageWeeklyWage', '1025.00', 'State average weekly wage for benefit calculations', 'Decimal', 'Benefits'),
('MaxWeeklyBenefitPct', '0.90', 'Maximum weekly benefit as pct of state avg wage', 'Decimal', 'Benefits'),
('MinWeeklyBenefitPct', '0.20', 'Minimum weekly benefit as pct of state avg wage', 'Decimal', 'Benefits'),
('WaitingPeriodDays', '5', 'Waiting period before benefits begin', 'Integer', 'Benefits'),
('MaxExportRows', '50000', 'Maximum rows for report export', 'Integer', 'Reports'),
('SessionTimeoutMinutes', '30', 'User session timeout', 'Integer', 'Security'),
('MaxLoginAttempts', '5', 'Max failed login attempts before lockout', 'Integer', 'Security'),
('FiscalYearStart', '07', 'Fiscal year start month', 'Integer', 'Financial')
GO

-- Injury Codes
INSERT INTO dbo.InjuryCodes (Code, Description, Category, Severity, TypicalDuration) VALUES
('INJ001', 'Strain/Sprain - Back', 'Trauma', 'Moderate', 42),
('INJ002', 'Strain/Sprain - Shoulder', 'Trauma', 'Moderate', 35),
('INJ003', 'Fracture - Upper Extremity', 'Trauma', 'Severe', 56),
('INJ004', 'Fracture - Lower Extremity', 'Trauma', 'Severe', 84),
('INJ005', 'Laceration/Cut', 'Trauma', 'Minor', 14),
('INJ006', 'Contusion/Bruise', 'Trauma', 'Minor', 10),
('INJ007', 'Burns - Chemical', 'Environmental', 'Moderate', 28),
('INJ008', 'Burns - Thermal', 'Environmental', 'Moderate', 35),
('INJ009', 'Carpal Tunnel Syndrome', 'Repetitive', 'Moderate', 90),
('INJ010', 'Repetitive Strain - Wrist', 'Repetitive', 'Moderate', 60),
('INJ011', 'Heat Exhaustion/Stroke', 'Environmental', 'Severe', 14),
('INJ012', 'Concussion/Head Injury', 'Trauma', 'Severe', 42),
('INJ013', 'Eye Injury', 'Trauma', 'Moderate', 21),
('INJ014', 'Hearing Loss', 'Occupational', 'Moderate', 0),
('INJ015', 'Respiratory Condition', 'Occupational', 'Severe', 90)
GO

-- Body Part Codes
INSERT INTO dbo.BodyPartCodes (Code, Description, BodyRegion) VALUES
('BP01', 'Head', 'Head'), ('BP02', 'Neck', 'Head'), ('BP03', 'Face', 'Head'),
('BP04', 'Upper Back', 'Torso'), ('BP05', 'Lower Back', 'Torso'), ('BP06', 'Chest', 'Torso'),
('BP07', 'Abdomen', 'Torso'), ('BP08', 'Right Shoulder', 'UpperExtremity'),
('BP09', 'Left Shoulder', 'UpperExtremity'), ('BP10', 'Right Arm', 'UpperExtremity'),
('BP11', 'Left Arm', 'UpperExtremity'), ('BP12', 'Right Wrist', 'UpperExtremity'),
('BP13', 'Left Wrist', 'UpperExtremity'), ('BP14', 'Right Hand', 'UpperExtremity'),
('BP15', 'Left Hand', 'UpperExtremity'), ('BP16', 'Right Knee', 'LowerExtremity'),
('BP17', 'Left Knee', 'LowerExtremity'), ('BP18', 'Right Ankle', 'LowerExtremity'),
('BP19', 'Left Ankle', 'LowerExtremity'), ('BP20', 'Multiple Body Parts', 'Multiple')
GO

-- Agencies
SET IDENTITY_INSERT dbo.Agencies ON
INSERT INTO dbo.Agencies (AgencyId, AgencyCode, AgencyName, Division, AgencyType, ContactName, ContactEmail, ContactPhone, Address, City, State, ZipCode, RiskCategory, AnnualPremium) VALUES
(1, 'DOT', 'Department of Transportation', 'Highway Maintenance', 'State', 'Mike Reynolds', 'mreynolds@anystate.gov', '555-0101', '3311 W State St', 'Capitol City', 'AS', '83703', 'High', 285000.00),
(2, 'DHW', 'Department of Health and Welfare', 'Family Services', 'State', 'Sarah Chen', 'schen@anystate.gov', '555-0102', '450 W State St', 'Capitol City', 'AS', '83720', 'Standard', 165000.00),
(3, 'DOL', 'Department of Labor', 'Workforce Services', 'State', 'James Wilson', 'jwilson@anystate.gov', '555-0103', '317 W Main St', 'Capitol City', 'AS', '83735', 'Low', 95000.00),
(4, 'DPW', 'Department of Public Works', 'Facilities Management', 'State', 'Linda Martinez', 'lmartinez@anystate.gov', '555-0104', '502 N 4th St', 'Capitol City', 'AS', '83702', 'High', 210000.00),
(5, 'DOC', 'Department of Corrections', 'Security Operations', 'State', 'Robert Taylor', 'rtaylor@anystate.gov', '555-0105', '1299 N Orchard St', 'Capitol City', 'AS', '83706', 'Critical', 425000.00),
(6, 'BSU', 'State University', 'Campus Operations', 'Education', 'Karen Davis', 'kdavis@anystate.edu', '555-0106', '1910 University Dr', 'Capitol City', 'AS', '83725', 'Standard', 145000.00),
(7, 'SIF', 'State Insurance Fund', 'Claims Operations', 'State', 'Tom Bradley', 'tbradley@anystate.gov', '555-0107', '1215 W State St', 'Capitol City', 'AS', '83702', 'Low', 55000.00),
(8, 'ISP', 'State Police', 'Field Operations', 'State', 'Col. Diana Ross', 'dross@anystate.gov', '555-0108', '700 S Stratford Dr', 'Capitol City', 'AS', '83642', 'Critical', 380000.00),
(9, 'DFG', 'Department of Fish and Game', 'Wildlife Management', 'State', 'Mark Stevens', 'mstevens@anystate.gov', '555-0109', '600 S Walnut St', 'Capitol City', 'AS', '83707', 'High', 175000.00),
(10, 'SCO', 'State Controllers Office', 'Financial Operations', 'State', 'Patricia Moore', 'pmoore@anystate.gov', '555-0110', '700 W State St', 'Capitol City', 'AS', '83720', 'Low', 45000.00)
SET IDENTITY_INSERT dbo.Agencies OFF
GO

PRINT 'Lookup tables and agencies seeded.'
GO
