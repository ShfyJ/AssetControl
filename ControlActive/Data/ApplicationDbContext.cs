using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ControlActive.Models;

namespace ControlActive.Data
{
    public class ApplicationDbContext : AuditableIdentityContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RealEstateInfrastructure>().HasKey(sc => new { sc.RealEstateId, sc.InfrastuctureId });
            //modelBuilder.Entity<RealEstateTechnicalCharcs>().HasKey(sc => new { sc.RealEstateId, sc.TechnicalCharcId });
            modelBuilder.Entity<SharesAndHolders>().HasKey(sc => new { sc.ShareId, sc.ShareholderId });

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<FileModel> FileModels { get; set; }
        public DbSet<RealEstate> RealEstates { get; set; }
        public DbSet<RealEstateInfrastructure> RRealEstateInfrastructures { get; set; }
       // public DbSet<RealEstateTechnicalCharcs> RealEstateTechnicalCharcs { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Shareholder> Shareholders { get; set; }
        public DbSet<SharesAndHolders> SharesAndHolders { get; set; }
        public DbSet<AssetEvaluation> AssetEvaluations { get; set; }
        public DbSet<InstallmentAsset> InstallmentAssets { get; set; }
        public DbSet<InstallmentStep2> InstallmentStep2 { get; set; }
        public DbSet<OneTimePaymentAsset> OneTimePaymentAssets { get; set; }
        public DbSet<OneTimePaymentStep2> OneTimePaymentStep2 { get; set; }
        public DbSet<OneTimePaymentStep3> OneTimePaymentStep3 { get; set; }
        public DbSet<ReductionInAsset> ReductionInAssets { get; set; }
        public DbSet<SubmissionOnBidding> SubmissionOnBiddings { get; set; }
        public DbSet<TransferredAsset> TransferredAssets { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<GoverningBody> GoverningBodies{ get; set; }
        public DbSet<Infrastucture> Infrastuctures { get; set; }
        public DbSet<Proposal> Proposals { get; set; }
        //public DbSet<TechnicalCharc> TechnicalCharcs { get; set; }
        public DbSet<TransferForm> TransferForms { get; set; }
        public DbSet<Noti> Notifications { get; set; }
        public DbSet<NotiMessage> NotiMessageLibrary { get; set; }

        [DbFunction(Name = "SOUNDEX", Schema="")]
        public static string SoundsLike(string keyword)
        {
            throw new NotImplementedException();
        }

        public DbSet<Template> Template { get; set; }
    }
}
