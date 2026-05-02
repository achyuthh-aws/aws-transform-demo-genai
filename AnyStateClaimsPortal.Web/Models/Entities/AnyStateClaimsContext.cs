using System.Data.Entity;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class AnyStateClaimsContext : DbContext
    {
        public AnyStateClaimsContext() : base("name=AnyStateClaimsDB")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimNote> ClaimNotes { get; set; }
        public DbSet<ClaimStatusHistory> ClaimStatusHistories { get; set; }
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }
        public DbSet<ClaimPayment> ClaimPayments { get; set; }
        public DbSet<MedicalProvider> MedicalProviders { get; set; }
        public DbSet<MedicalTreatment> MedicalTreatments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<InjuryCode> InjuryCodes { get; set; }
        public DbSet<BodyPartCode> BodyPartCodes { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agency>().ToTable("Agencies");
            modelBuilder.Entity<Employee>().ToTable("Employees");
            modelBuilder.Entity<Claim>().ToTable("Claims");
            modelBuilder.Entity<ClaimNote>().ToTable("ClaimNotes");
            modelBuilder.Entity<ClaimStatusHistory>().ToTable("ClaimStatusHistory");
            modelBuilder.Entity<ClaimDocument>().ToTable("ClaimDocuments");
            modelBuilder.Entity<ClaimPayment>().ToTable("ClaimPayments");
            modelBuilder.Entity<MedicalProvider>().ToTable("MedicalProviders");
            modelBuilder.Entity<MedicalTreatment>().ToTable("MedicalTreatments");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<InjuryCode>().ToTable("InjuryCodes");
            modelBuilder.Entity<BodyPartCode>().ToTable("BodyPartCodes");
            modelBuilder.Entity<SystemConfiguration>().ToTable("SystemConfiguration");
            modelBuilder.Entity<AuditLog>().ToTable("AuditLog");

            // Employee -> Agency
            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Agency)
                .WithMany(a => a.Employees)
                .HasForeignKey(e => e.AgencyId);

            // Claim -> Employee
            modelBuilder.Entity<Claim>()
                .HasRequired(c => c.Employee)
                .WithMany(e => e.Claims)
                .HasForeignKey(c => c.EmployeeId);

            // Claim -> Adjuster (User)
            modelBuilder.Entity<Claim>()
                .HasOptional(c => c.AssignedAdjuster)
                .WithMany()
                .HasForeignKey(c => c.AssignedAdjusterId);

            // Claim -> MedicalReviewer (User)
            modelBuilder.Entity<Claim>()
                .HasOptional(c => c.MedicalReviewer)
                .WithMany()
                .HasForeignKey(c => c.MedicalReviewerId);

            // ClaimNote -> Claim
            modelBuilder.Entity<ClaimNote>()
                .HasRequired(n => n.Claim)
                .WithMany(c => c.ClaimNotes)
                .HasForeignKey(n => n.ClaimId);

            // ClaimStatusHistory -> Claim
            modelBuilder.Entity<ClaimStatusHistory>()
                .HasRequired(h => h.Claim)
                .WithMany(c => c.ClaimStatusHistories)
                .HasForeignKey(h => h.ClaimId);

            // ClaimDocument -> Claim
            modelBuilder.Entity<ClaimDocument>()
                .HasRequired(d => d.Claim)
                .WithMany(c => c.ClaimDocuments)
                .HasForeignKey(d => d.ClaimId);

            // ClaimPayment -> Claim
            modelBuilder.Entity<ClaimPayment>()
                .HasRequired(p => p.Claim)
                .WithMany(c => c.ClaimPayments)
                .HasForeignKey(p => p.ClaimId);

            // MedicalTreatment -> Claim
            modelBuilder.Entity<MedicalTreatment>()
                .HasRequired(t => t.Claim)
                .WithMany(c => c.MedicalTreatments)
                .HasForeignKey(t => t.ClaimId);

            // MedicalTreatment -> Provider
            modelBuilder.Entity<MedicalTreatment>()
                .HasRequired(t => t.Provider)
                .WithMany()
                .HasForeignKey(t => t.ProviderId);

            // User -> Agency (optional)
            modelBuilder.Entity<User>()
                .HasOptional<Agency>(u => u.Agency)
                .WithMany()
                .HasForeignKey(u => u.AgencyId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
