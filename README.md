# AnyState Workers' Compensation Claims Portal

A legacy ASP.NET MVC 5 (.NET Framework 4.8) web application with SQL Server 2019 backend for managing workers' compensation claims. Built with intentionally dated technology patterns as a modernization candidate for **AWS Transform**.

## Tech Stack

| Layer | Technology | Version |
|---|---|---|
| Framework | .NET Framework | 4.8 |
| Web | ASP.NET MVC 5 | 5.2.9 |
| ORM | Entity Framework 6 | 6.4.4 |
| Data Access | ADO.NET | SqlConnection/SqlCommand |
| Service | WCF | basicHttpBinding |
| Auth | Forms Authentication | web.config |
| Database | SQL Server | 2019 |
| Frontend | jQuery + Bootstrap 3 | 1.12 / 3.3.7 |
| Testing | NUnit 3 | 3.14.0 |

## Features

- Claims submission and workflow management (7 statuses)
- Benefit calculation engine with dependent/longevity bonuses
- Medical treatment tracking with provider management
- Payment processing with batch operations
- Agency claims reporting with loss ratios
- Claims aging analysis
- Financial summary reports
- Role-based access (Administrator, ClaimsAdjuster, AgencyStaff, MedicalReviewer, ReadOnly)
- Audit logging with triggers
- System configuration management

## Database Setup

Run scripts in order:
1. `01_CreateTablesAll.sql` — 13 tables
2. `02_StoredProcedures.sql` — 8 stored procedures
3. `03_ViewsAndTriggers.sql` — 4 views, 3 triggers
4. `04_SeedData_Lookups.sql` — Config, injury codes, body parts, agencies
5. `05_SeedData_People.sql` — Employees, users, medical providers
6. `06_SeedData_Claims.sql` — 15 claims
7. `07_SeedData_Details.sql` — Notes, payments, treatments

## Test Credentials

Any password works for seeded users:

| Username | Role |
|---|---|
| admin | Administrator |
| adjuster1 | ClaimsAdjuster |
| adjuster2 | ClaimsAdjuster |
| adjuster3 | ClaimsAdjuster |
| medreview1 | MedicalReviewer |
| staff1 | AgencyStaff (DOT) |
| staff2 | AgencyStaff (DHW) |
| staff3 | AgencyStaff (DOC) |
| readonly1 | ReadOnly |

## AWS Transform Modernization Targets

| Legacy Pattern | Target |
|---|---|
| .NET Framework 4.8 | .NET 8 (cross-platform) |
| ASP.NET MVC 5 | ASP.NET Core MVC |
| Entity Framework 6 | EF Core |
| ADO.NET SqlConnection | Updated data access |
| WCF Service (.svc) | REST API |
| Forms Authentication | ASP.NET Core Identity |
| SQL Server T-SQL | Aurora PostgreSQL PL/pgSQL |
| Web.config | appsettings.json |
| Classic .csproj | SDK-style .csproj |
| NUnit 3 (.NET Framework) | NUnit (.NET 8) |
