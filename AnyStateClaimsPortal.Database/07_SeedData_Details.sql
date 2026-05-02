USE [AnyStateClaimsDB]
GO

-- Claim Notes
INSERT INTO dbo.ClaimNotes (ClaimId, NoteText, NoteType, IsConfidential, CreatedBy) VALUES
(1, 'Initial review complete. Medical records received from St. Lukes Regional.', 'General', 0, 'adjuster1'),
(1, 'Claim approved. Temporary disability benefits authorized for 14 weeks.', 'StatusChange', 0, 'adjuster1'),
(1, 'Physical therapy sessions approved - 3x per week for 6 weeks.', 'Medical', 0, 'medreview1'),
(2, 'Incident report from facility warden received and reviewed. Use of force report attached.', 'General', 0, 'adjuster1'),
(2, 'Approved for temporary disability. Physical therapy and orthopedic follow-up authorized.', 'StatusChange', 0, 'adjuster1'),
(2, 'MRI results show partial rotator cuff tear. Surgery may be required.', 'Medical', 1, 'medreview1'),
(3, 'Awaiting orthopedic evaluation report. X-rays confirm non-displaced fracture.', 'Medical', 0, 'adjuster2'),
(3, 'Referred to medical review for treatment plan approval.', 'StatusChange', 0, 'adjuster2'),
(5, 'Denied - employee failed to follow mandatory hydration protocol per agency policy AS-2019-047.', 'StatusChange', 0, 'adjuster2'),
(5, 'Employee has filed appeal. Forwarding to legal review.', 'Legal', 1, 'adjuster2'),
(7, 'ER records confirm lumbar strain. Light duty recommended 6 weeks.', 'Medical', 0, 'adjuster1'),
(7, 'Employee returned to light duty on 2025-07-15. Full duty expected by 2025-08-12.', 'General', 0, 'adjuster1'),
(8, 'Claim resolved. Employee returned to full duty. Final payment issued.', 'StatusChange', 0, 'adjuster2'),
(9, 'Trooper Lewis has extensive field duty. Knee injury consistent with pursuit activities.', 'General', 0, 'adjuster3'),
(9, 'Orthopedic consult recommends arthroscopic surgery. Pre-authorization submitted.', 'Medical', 0, 'medreview1'),
(10, 'Unusual claim - wildlife encounter. Incident report from Fish and Game confirms details.', 'General', 0, 'adjuster1'),
(10, 'Lacerations required 23 stitches. Tetanus and rabies prophylaxis administered.', 'Medical', 0, 'medreview1'),
(11, 'CT scan ordered to rule out intracranial bleeding. Employee on mandatory rest protocol.', 'Medical', 1, 'medreview1'),
(12, 'Potential permanent partial disability. Awaiting hand specialist evaluation.', 'Medical', 1, 'adjuster1'),
(13, 'Multiple officers involved in incident. This claim is for Lt. Wright only.', 'Internal', 1, 'adjuster3'),
(13, 'Approved for temporary disability. Psychological evaluation also recommended.', 'StatusChange', 0, 'adjuster3')
GO

-- Claim Payments
INSERT INTO dbo.ClaimPayments (ClaimId, PaymentDate, PaymentType, Amount, CheckNumber, PayeeName, PayeeType, Description, FiscalYear, Status) VALUES
(1, '2025-02-01', 'WeeklyBenefit', 580.00, 'CHK-10001', 'John Anderson', 'Employee', 'Weekly benefit - week 1', 2025, 'Issued'),
(1, '2025-02-08', 'WeeklyBenefit', 580.00, 'CHK-10002', 'John Anderson', 'Employee', 'Weekly benefit - week 2', 2025, 'Issued'),
(1, '2025-02-15', 'WeeklyBenefit', 580.00, 'CHK-10003', 'John Anderson', 'Employee', 'Weekly benefit - week 3', 2025, 'Issued'),
(1, '2025-01-20', 'Medical', 1250.00, 'CHK-10004', 'St. Lukes Regional Medical Center', 'Provider', 'ER visit and imaging', 2025, 'Issued'),
(1, '2025-02-10', 'Medical', 1200.00, 'CHK-10005', 'Capitol Physical Therapy', 'Provider', 'PT sessions weeks 1-3', 2025, 'Issued'),
(2, '2025-02-20', 'WeeklyBenefit', 720.00, 'CHK-10006', 'Michael Johnson', 'Employee', 'Weekly benefit - week 1', 2025, 'Issued'),
(2, '2025-02-27', 'WeeklyBenefit', 720.00, 'CHK-10007', 'Michael Johnson', 'Employee', 'Weekly benefit - week 2', 2025, 'Issued'),
(2, '2025-02-10', 'Medical', 2400.00, 'CHK-10008', 'AnyState Orthopedic Associates', 'Provider', 'Orthopedic consult and MRI', 2025, 'Issued'),
(7, '2025-07-15', 'WeeklyBenefit', 650.00, 'CHK-10009', 'Patricia Moore', 'Employee', 'Weekly benefit - week 1', 2025, 'Issued'),
(7, '2025-07-22', 'WeeklyBenefit', 650.00, 'CHK-10010', 'Patricia Moore', 'Employee', 'Weekly benefit - week 2', 2025, 'Issued'),
(7, '2025-07-05', 'Medical', 1800.00, 'CHK-10011', 'Mountain View Urgent Care', 'Provider', 'ER visit and X-rays', 2025, 'Issued'),
(8, '2025-09-01', 'WeeklyBenefit', 420.00, 'CHK-10012', 'David Thompson', 'Employee', 'Weekly benefit - weeks 1-4', 2025, 'Issued'),
(8, '2025-08-25', 'Medical', 650.00, 'CHK-10013', 'Mountain View Urgent Care', 'Provider', 'Ankle X-ray and treatment', 2025, 'Issued'),
(9, '2025-09-20', 'WeeklyBenefit', 760.00, 'CHK-10014', 'Angela Lewis', 'Employee', 'Weekly benefit - week 1', 2025, 'Issued'),
(9, '2025-09-27', 'WeeklyBenefit', 760.00, 'CHK-10015', 'Angela Lewis', 'Employee', 'Weekly benefit - week 2', 2025, 'Issued'),
(9, '2025-09-10', 'Medical', 3500.00, 'CHK-10016', 'AnyState Orthopedic Associates', 'Provider', 'Knee MRI and consult', 2025, 'Issued'),
(10, '2025-10-05', 'Medical', 4100.00, 'CHK-10017', 'St. Lukes Regional Medical Center', 'Provider', 'ER treatment - lacerations', 2025, 'Issued'),
(10, '2025-10-10', 'WeeklyBenefit', 590.00, 'CHK-10018', 'Daniel Walker', 'Employee', 'Weekly benefit - week 1', 2025, 'Issued'),
(13, '2025-11-25', 'WeeklyBenefit', 890.00, 'CHK-10019', 'Thomas Wright', 'Employee', 'Weekly benefit - week 1', 2025, 'Issued'),
(13, '2025-12-02', 'WeeklyBenefit', 890.00, 'CHK-10020', 'Thomas Wright', 'Employee', 'Weekly benefit - week 2', 2025, 'Issued'),
(13, '2025-11-20', 'Medical', 5200.00, 'CHK-10021', 'St. Lukes Regional Medical Center', 'Provider', 'ER and radiology', 2025, 'Issued')
GO

-- Medical Treatments
INSERT INTO dbo.MedicalTreatments (ClaimId, ProviderId, TreatmentDate, TreatmentType, Description, Diagnosis, ICDCode, BilledAmount, ApprovedAmount, Status) VALUES
(1, 1, '2025-01-15', 'InitialVisit', 'ER evaluation for acute lower back pain after fall', 'Lumbar strain', 'S39.012A', 1250.00, 1250.00, 'Paid'),
(1, 3, '2025-01-22', 'PhysicalTherapy', 'Initial PT evaluation and treatment plan', 'Lumbar strain - PT', 'S39.012A', 400.00, 400.00, 'Paid'),
(2, 2, '2025-02-05', 'InitialVisit', 'Orthopedic evaluation of right shoulder', 'Partial rotator cuff tear', 'S46.011A', 800.00, 800.00, 'Paid'),
(2, 1, '2025-02-10', 'Diagnostic', 'MRI right shoulder', 'Rotator cuff tear confirmation', 'S46.011A', 1600.00, 1600.00, 'Paid'),
(3, 1, '2025-03-10', 'InitialVisit', 'ER evaluation - left wrist fracture', 'Non-displaced distal radius fracture', 'S52.502A', 950.00, 950.00, 'Paid'),
(3, 2, '2025-03-17', 'FollowUp', 'Orthopedic follow-up, cast application', 'Distal radius fracture', 'S52.502A', 450.00, 450.00, 'Paid'),
(7, 4, '2025-07-01', 'InitialVisit', 'Urgent care evaluation for acute back pain', 'Thoracic strain', 'S29.012A', 650.00, 650.00, 'Paid'),
(8, 4, '2025-08-19', 'InitialVisit', 'Urgent care - right ankle sprain', 'Lateral ankle sprain grade II', 'S93.401A', 450.00, 450.00, 'Paid'),
(9, 2, '2025-09-08', 'InitialVisit', 'Orthopedic evaluation - left knee', 'ACL sprain with possible meniscus tear', 'S83.512A', 900.00, 900.00, 'Paid'),
(9, 1, '2025-09-12', 'Diagnostic', 'MRI left knee', 'Partial ACL tear confirmed', 'S83.512A', 1800.00, 1800.00, 'Paid'),
(10, 1, '2025-09-22', 'InitialVisit', 'ER treatment - multiple lacerations right arm', 'Multiple lacerations, animal encounter', 'W55.11XA', 2800.00, 2800.00, 'Paid'),
(11, 1, '2025-10-10', 'InitialVisit', 'ER evaluation - head injury', 'Concussion, loss of consciousness', 'S06.0X0A', 1500.00, 1500.00, 'Paid'),
(11, 1, '2025-10-12', 'Diagnostic', 'CT scan head', 'Rule out intracranial hemorrhage', 'S06.0X0A', 700.00, 700.00, 'Approved')
GO

PRINT 'Notes, payments, and treatments seeded.'
GO
