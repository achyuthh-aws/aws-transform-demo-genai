USE [AnyStateClaimsDB]
GO

-- Employees (20 employees across agencies)
SET IDENTITY_INSERT dbo.Employees ON
INSERT INTO dbo.Employees (EmployeeId, EmployeeNumber, FirstName, LastName, MiddleInitial, DateOfBirth, HireDate, JobTitle, JobClassCode, Department, AnnualSalary, PayFrequency, AgencyId, SupervisorName, NumberOfDependents) VALUES
(1,  'EMP-001', 'John',     'Anderson',  'R', '1985-03-15', '2010-06-01', 'Highway Maintenance Worker',  'HMW-3', 'Maintenance',    42500.00, 'Biweekly', 1, 'Mike Reynolds', 2),
(2,  'EMP-002', 'Maria',    'Garcia',    'L', '1990-07-22', '2015-01-15', 'Social Worker III',           'SW-3',  'Family Services', 48000.00, 'Biweekly', 2, 'Sarah Chen', 1),
(3,  'EMP-003', 'David',    'Thompson',  'A', '1978-11-08', '2005-09-01', 'Employment Specialist',       'ES-2',  'UI Division',     52000.00, 'Biweekly', 3, 'James Wilson', 3),
(4,  'EMP-004', 'Jennifer', 'Brown',     'M', '1992-04-30', '2018-03-15', 'Facilities Technician II',    'FT-2',  'Maintenance',     39500.00, 'Biweekly', 4, 'Linda Martinez', 0),
(5,  'EMP-005', 'Michael',  'Johnson',   'T', '1982-09-12', '2008-11-01', 'Corrections Officer III',     'CO-3',  'Security',        55000.00, 'Biweekly', 5, 'Robert Taylor', 4),
(6,  'EMP-006', 'Lisa',     'Williams',  'K', '1988-01-25', '2012-08-15', 'Groundskeeper II',            'GK-2',  'Facilities',      36000.00, 'Biweekly', 6, 'Karen Davis', 1),
(7,  'EMP-007', 'Robert',   'Davis',     'J', '1975-06-18', '2002-04-01', 'Bridge Inspector Sr',         'BI-4',  'Engineering',     58000.00, 'Biweekly', 1, 'Mike Reynolds', 2),
(8,  'EMP-008', 'Amanda',   'Miller',    'S', '1995-12-03', '2020-01-06', 'Case Manager I',              'CM-1',  'Child Welfare',   44000.00, 'Biweekly', 2, 'Sarah Chen', 0),
(9,  'EMP-009', 'Chris',    'Wilson',    'D', '1987-08-14', '2014-05-20', 'IT Support Specialist',       'IT-2',  'Technology',      47000.00, 'Biweekly', 3, 'James Wilson', 1),
(10, 'EMP-010', 'Patricia', 'Moore',     'E', '1980-02-28', '2006-10-01', 'Building Engineer III',       'BE-3',  'Engineering',     51000.00, 'Biweekly', 4, 'Linda Martinez', 3),
(11, 'EMP-011', 'James',    'Taylor',    'W', '1983-05-20', '2009-03-15', 'Corrections Sergeant',        'CS-1',  'Security',        62000.00, 'Biweekly', 5, 'Robert Taylor', 2),
(12, 'EMP-012', 'Susan',    'Martinez',  'A', '1991-09-10', '2016-07-01', 'Research Assistant',          'RA-2',  'Sciences',        38000.00, 'Biweekly', 6, 'Karen Davis', 0),
(13, 'EMP-013', 'William',  'Clark',     'H', '1977-12-05', '2003-01-15', 'Claims Examiner Sr',          'CE-4',  'Claims',          56000.00, 'Biweekly', 7, 'Tom Bradley', 3),
(14, 'EMP-014', 'Angela',   'Lewis',     'N', '1989-03-28', '2017-09-01', 'State Trooper',               'ST-2',  'Patrol',          58000.00, 'Biweekly', 8, 'Col. Diana Ross', 1),
(15, 'EMP-015', 'Daniel',   'Walker',    'P', '1984-07-15', '2011-04-01', 'Wildlife Officer II',         'WO-2',  'Enforcement',     46000.00, 'Biweekly', 9, 'Mark Stevens', 2),
(16, 'EMP-016', 'Karen',    'Hall',      'B', '1993-11-22', '2019-06-15', 'Accountant II',               'AC-2',  'Accounting',      49000.00, 'Biweekly', 10, 'Patricia Moore', 0),
(17, 'EMP-017', 'Steven',   'Young',     'C', '1986-04-08', '2013-02-01', 'Heavy Equipment Operator',    'HE-3',  'Construction',    48000.00, 'Biweekly', 1, 'Mike Reynolds', 3),
(18, 'EMP-018', 'Nancy',    'King',      'F', '1994-08-30', '2021-03-15', 'Eligibility Worker I',        'EW-1',  'Medicaid',        37000.00, 'Biweekly', 2, 'Sarah Chen', 0),
(19, 'EMP-019', 'Thomas',   'Wright',    'G', '1981-01-17', '2007-08-01', 'Corrections Lieutenant',      'CL-1',  'Security',        68000.00, 'Biweekly', 5, 'Robert Taylor', 4),
(20, 'EMP-020', 'Betty',    'Scott',     'I', '1976-06-25', '2001-11-01', 'Fish Hatchery Manager',       'FH-3',  'Hatcheries',      52000.00, 'Biweekly', 9, 'Mark Stevens', 2)
SET IDENTITY_INSERT dbo.Employees OFF
GO

-- Users
SET IDENTITY_INSERT dbo.Users ON
INSERT INTO dbo.Users (UserId, Username, PasswordHash, Salt, FullName, Email, Phone, Role, AgencyId, IsActive) VALUES
(1, 'admin',      'demo_hash_admin',      'salt1', 'System Administrator',  'admin@anystate.gov',       '555-9001', 'Administrator',   NULL, 1),
(2, 'adjuster1',  'demo_hash_adj1',       'salt2', 'Patricia Hernandez',    'phernandez@anystate.gov',  '555-9002', 'ClaimsAdjuster',  NULL, 1),
(3, 'adjuster2',  'demo_hash_adj2',       'salt3', 'Thomas Wright',         'twright@anystate.gov',     '555-9003', 'ClaimsAdjuster',  NULL, 1),
(4, 'adjuster3',  'demo_hash_adj3',       'salt4', 'Rachel Kim',            'rkim@anystate.gov',        '555-9004', 'ClaimsAdjuster',  NULL, 1),
(5, 'medreview1', 'demo_hash_med1',       'salt5', 'Dr. James Patterson',   'jpatterson@anystate.gov',  '555-9005', 'MedicalReviewer', NULL, 1),
(6, 'staff1',     'demo_hash_staff1',     'salt6', 'John Anderson',         'janderson@anystate.gov',   '555-9006', 'AgencyStaff',     1,    1),
(7, 'staff2',     'demo_hash_staff2',     'salt7', 'Maria Garcia',          'mgarcia@anystate.gov',     '555-9007', 'AgencyStaff',     2,    1),
(8, 'staff3',     'demo_hash_staff3',     'salt8', 'Michael Johnson',       'mjohnson@anystate.gov',    '555-9008', 'AgencyStaff',     5,    1),
(9, 'readonly1',  'demo_hash_readonly1',  'salt9', 'Budget Analyst',        'budget@anystate.gov',      '555-9009', 'ReadOnly',        NULL, 1)
SET IDENTITY_INSERT dbo.Users OFF
GO

-- Medical Providers
SET IDENTITY_INSERT dbo.MedicalProviders ON
INSERT INTO dbo.MedicalProviders (ProviderId, ProviderName, ProviderType, NPI, Address, City, State, ZipCode, Phone, IsPreferred) VALUES
(1, 'St. Lukes Regional Medical Center', 'Hospital',        '1234567890', '190 E Bannock St',   'Capitol City', 'AS', '83712', '555-8001', 1),
(2, 'AnyState Orthopedic Associates',    'Specialist',      '2345678901', '901 N Curtis Rd',    'Capitol City', 'AS', '83706', '555-8002', 1),
(3, 'Capitol Physical Therapy',          'PhysicalTherapy', '3456789012', '500 S Americana Blvd','Capitol City','AS', '83702', '555-8003', 1),
(4, 'Mountain View Urgent Care',         'Clinic',          '4567890123', '2200 E Fairview Ave', 'Capitol City', 'AS', '83642', '555-8004', 0),
(5, 'AnyState Eye Center',              'Specialist',      '5678901234', '1025 S Capitol Blvd', 'Capitol City', 'AS', '83706', '555-8005', 0),
(6, 'Valley Occupational Medicine',      'Clinic',          '6789012345', '3100 W Elder St',     'Capitol City', 'AS', '83705', '555-8006', 1)
SET IDENTITY_INSERT dbo.MedicalProviders OFF
GO

PRINT 'Employees, users, and providers seeded.'
GO
