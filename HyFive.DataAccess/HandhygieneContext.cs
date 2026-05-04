using HyFive.Domain.Observation;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Place;
using HyFive.Domain.Session;
using HyFive.Domain.User;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Emit;

namespace HyFive.DataAccess
{
    public class HandHygieneContext : DbContext
    {
        public HandHygieneContext(DbContextOptions<HandHygieneContext> options) : base(options)
        {
        }

        public DbSet<Session> Session { get; set; }
        public DbSet<FiveIndicationsSession> FiveIndicationsSession { get; set; }
        public DbSet<HandJewelrySession> HandJewelrySession { get; set; }
        public DbSet<ProtectiveEquipmentSession> ProtectiveEquipmentSession { get; set; }
        public DbSet<GloveSession> GloveSession { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<TransferStatusType> TransferStatusType { get; set; }
        public DbSet<OrganisationUnit> OrganisationUnit { get; set; }
        public DbSet<OrganisationUnitType> OrganisationUnitType { get; set; }
        public DbSet<OrganisationUnitLevel> OrganisationUnitLevel { get; set; }
        public DbSet<OrganisationUnitAssociation> OrganisationUnitAssociation { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<PredefinedComment> PredefinedComment { get; set; }
        public DbSet<ActivityType> ActivityType { get; set; }
        public DbSet<HandJewelryType> HandJewelryType { get; set; }
        public DbSet<ProtectiveEquipmentSettingType> ProtectiveEquipmentSettingType { get; set; }
        public DbSet<ProtectiveEquipmentType> ProtectiveEquipmentType { get; set; }
        public DbSet<GloveWithIndicationType> GloveWithIndicationType { get; set; }
        public DbSet<GloveWithoutIndicationType> GloveWithoutIndicationType { get; set; }
        public DbSet<HandHygieneAfterGloveUseType> HandHygieneAfterGloveUseType { get; set; }
        public DbSet<FiveIndicationsObservation> FiveIndicationsObservation { get; set; }
        public DbSet<GloveObservation> GloveObservation { get; set; }
        public DbSet<HandJewelryObservation> HandJewelryObservation { get; set; }
        public DbSet<ProtectiveEquipmentObservation> ProtectiveEquipmentObservation { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<IndicationTypes> IndicationTypes { get; set; }
        public DbSet<MisuseType> MisuseType { get; set; }
        public DbSet<UserPermission> UserPermission { get; set; }
        public DbSet<OrganisationUnitRole> OrganisationUnitRole { get; set; }
        public DbSet<UserIdentifierType> UserIdentifierTypes { get; set; }
        public DbSet<UserIdentifier> UserIdentifier { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Session>(b =>
            {
                b.Property(s => s.Comment)
                    .HasMaxLength(1000);

                b.Property(s => s.Discriminator)
                    .HasMaxLength(34)
                    .IsRequired();

                b.HasIndex(s => s.Discriminator);
                b.HasIndex(s => s.CreatedDate);
                b.HasIndex(s => s.StartDate);

                // NEW: Session -> OrganisationUnit
                b.HasIndex(s => s.OrganisationUnitId);

                b.HasOne(s => s.OrganisationUnit)
                    .WithMany() // or .WithMany(o => o.Sessions) if you add navigation
                    .HasForeignKey(s => s.OrganisationUnitId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Observer relationship
                b.HasIndex(s => s.ObserverId);

                b.HasOne(s => s.Observer)
                    .WithMany()
                    .HasForeignKey(s => s.ObserverId)
                    .OnDelete(DeleteBehavior.Restrict);

                // TransferStatus relationship
                b.HasIndex(s => s.TransferStatusId);

                b.HasOne(s => s.TransferStatus)
                    .WithMany()
                    .HasForeignKey(s => s.TransferStatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<FiveIndicationsObservation>().Property(fio => fio.Comment).HasMaxLength(1000);
            mb.Entity<FiveIndicationsObservation>().HasIndex(fio => fio.CreatedTime);
            mb.Entity<FiveIndicationsObservation>().HasIndex(fio => fio.RegisteredTime);

            mb.Entity<ProtectiveEquipmentObservation>().HasIndex(bo => bo.CreatedTime);
            mb.Entity<ProtectiveEquipmentObservation>().HasIndex(bo => bo.RegisteredTime);

            mb.Entity<HandJewelryObservation>().HasIndex(ho => ho.CreatedTime);
            mb.Entity<HandJewelryObservation>().HasIndex(ho => ho.RegisteredTime);

            mb.Entity<GloveObservation>().HasIndex(ho => ho.CreatedTime);
            mb.Entity<GloveObservation>().HasIndex(ho => ho.RegisteredTime);

            mb.Entity<IndicationTypes>().Property(it => it.Code).HasMaxLength(50).IsRequired();
            mb.Entity<IndicationTypes>().HasIndex(it => it.Code).IsUnique();
            mb.Entity<IndicationTypes>().Property(it => it.Name).HasMaxLength(50);
            mb.Entity<IndicationTypes>().HasIndex(it => it.Name);
            mb.Entity<IndicationTypes>().Property(it => it.Number).HasMaxLength(2);

            mb.Entity<TransferStatusType>().Property(ost => ost.Name).HasMaxLength(50);
            mb.Entity<TransferStatusType>().HasIndex(ost => ost.Name);
            mb.Entity<TransferStatusType>().Property(ost => ost.Code).HasMaxLength(50).IsRequired();
            mb.Entity<TransferStatusType>().HasIndex(ost => ost.Code).IsUnique();

            mb.Entity<Role>().Property(r => r.Name).HasMaxLength(50);
            mb.Entity<Role>().Property(r => r.Description).HasMaxLength(250);
            mb.Entity<Role>().HasIndex(r => r.Name);

            mb.Entity<ActivityType>().Property(at => at.Code).HasMaxLength(50).IsRequired();
            mb.Entity<ActivityType>().HasIndex(at => at.Code).IsUnique();
            mb.Entity<ActivityType>().Property(at => at.Name).HasMaxLength(100);
            mb.Entity<ActivityType>().HasIndex(at => at.Name);

            mb.Entity<HandJewelryType>().Property(ht => ht.Code).HasMaxLength(50).IsRequired();
            mb.Entity<HandJewelryType>().HasIndex(ht => ht.Code).IsUnique();
            mb.Entity<HandJewelryType>().Property(ht => ht.Name).HasMaxLength(100);

            mb.Entity<ProtectiveEquipment>().Property(i => i.Comment).HasMaxLength(1000);

            mb.Entity<ProtectiveEquipmentType>().Property(but => but.Code).HasMaxLength(50).IsRequired();
            mb.Entity<ProtectiveEquipmentType>().HasIndex(but => but.Code).IsUnique();
            mb.Entity<ProtectiveEquipmentType>().Property(but => but.Name).HasMaxLength(100);
            mb.Entity<ProtectiveEquipmentType>().HasIndex(but => but.Name);
            

            mb.Entity<ProtectiveEquipmentSettingType>().Property(bust => bust.Code).HasMaxLength(50).IsRequired();
            mb.Entity<ProtectiveEquipmentSettingType>().HasIndex(bust => bust.Code).IsUnique();
            mb.Entity<ProtectiveEquipmentSettingType>().Property(bust => bust.Name).HasMaxLength(100);
            mb.Entity<ProtectiveEquipmentSettingType>().HasIndex(bust => bust.Name);
            

            mb.Entity<GloveWithIndicationType>().Property(h => h.Code).HasMaxLength(50).IsRequired();
            mb.Entity<GloveWithIndicationType>().HasIndex(h => h.Code).IsUnique();
            mb.Entity<GloveWithIndicationType>().Property(h => h.Name).HasMaxLength(100);
            mb.Entity<GloveWithIndicationType>().HasIndex(h => h.Name);

            mb.Entity<GloveWithoutIndicationType>().Property(h => h.Code).HasMaxLength(50).IsRequired();
            mb.Entity<GloveWithoutIndicationType>().HasIndex(h => h.Code).IsUnique();
            mb.Entity<GloveWithoutIndicationType>().Property(h => h.Name).HasMaxLength(100);
            mb.Entity<GloveWithoutIndicationType>().HasIndex(h => h.Name);

            mb.Entity<HandHygieneAfterGloveUseType>().Property(h => h.Code).HasMaxLength(50).IsRequired();
            mb.Entity<HandHygieneAfterGloveUseType>().HasIndex(h => h.Code).IsUnique();
            mb.Entity<HandHygieneAfterGloveUseType>().Property(h => h.Name).HasMaxLength(100);
            mb.Entity<HandHygieneAfterGloveUseType>().HasIndex(h => h.Name);
       

            mb.Entity<MisuseType>().Property(i => i.Name).HasMaxLength(100);
            mb.Entity<MisuseType>().HasIndex(i => i.Name);

            mb.Entity<PredefinedComment>(b =>
            {
                b.Property(x => x.Comment).HasMaxLength(1000);
                b.Property(x => x.SessionType).IsRequired();

                b.HasIndex(x => x.OrganisationUnitId);

                b.HasOne(x => x.OrganisationUnit)
                 .WithMany()
                 .HasForeignKey(x => x.OrganisationUnitId)
                 .OnDelete(DeleteBehavior.Cascade); // or Restrict if you want to block deletion when comments exist
            });
            mb.Entity<User>(b =>
            {
                b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
                b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
                b.Property(x => x.Email).HasMaxLength(500).IsRequired();
                b.Property(x => x.IdentityPseudonym).HasMaxLength(100);
                b.HasIndex(x => x.IdentityPseudonym);

                b.HasMany(x => x.UserIdentifiers)
                 .WithOne(x => x.User)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(x => x.UserPermissions)
                 .WithOne(x => x.User)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<ProtectiveEquipmentSettingTypeProtectiveEquipmentType>()
                .HasKey(bu => new { bu.ProtectiveEquipmentTypeId, bu.ProtectiveEquipmentSettingTypeId });
            mb.Entity<ProtectiveEquipmentSettingTypeProtectiveEquipmentType>()
                .HasOne(bu => bu.ProtectiveEquipmentType)
                .WithMany(b => b.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes)
                .HasForeignKey(bu => bu.ProtectiveEquipmentTypeId);
            mb.Entity<ProtectiveEquipmentSettingTypeProtectiveEquipmentType>()
                .HasOne(bc => bc.ProtectiveEquipmentSettingType)
                .WithMany(c => c.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes)
                .HasForeignKey(bc => bc.ProtectiveEquipmentSettingTypeId);

            mb.Entity<OrganisationUnitLevel>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Level).HasMaxLength(50).IsRequired();
                e.Property(x => x.Description).HasMaxLength(250);

                e.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy).HasMaxLength(100);
                e.Property(x => x.LastModified);
                e.Property(x => x.LastModifiedBy).HasMaxLength(100);
            });

            mb.Entity<OrganisationUnitType>(e =>
            {
                e.Property(x => x.Code).HasMaxLength(50).IsRequired();
                e.HasIndex(x => x.Code).IsUnique();

                e.Property(x => x.Name).HasMaxLength(150).IsRequired();
                e.Property(x => x.Description).HasMaxLength(250);

                e.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy).HasMaxLength(100);
                e.Property(x => x.LastModified);
                e.Property(x => x.LastModifiedBy).HasMaxLength(100);
            });

            mb.Entity<OrganisationUnit>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(250).IsRequired();
                e.Property(x => x.Abbreviation).HasMaxLength(50);
                e.Property(x => x.Description).HasMaxLength(500);

                e.HasIndex(x => new { x.ParentId, x.Name }).IsUnique();

                e.HasOne(x => x.Parent)
                    .WithMany(x => x.Children)
                    .HasForeignKey(x => x.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Type)
                    .WithMany()
                    .HasForeignKey(x => x.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.LevelRef)
                    .WithMany()
                    .HasForeignKey(x => x.LevelId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Address)
                    .WithMany()
                    .HasForeignKey(x => x.AddressId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy)
                    .HasMaxLength(100);
                e.Property(x => x.LastModified);
                e.Property(x => x.LastModifiedBy)
                    .HasMaxLength(100);
            });

            mb.Entity<OrganisationUnitAssociation>(entity =>
            {
                entity.ToTable("OrganisationUnitAssociation");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.AssociationType)
                    .IsRequired();

                entity.HasOne(x => x.SourceOrganisationUnit)
                    .WithMany(x => x.OutgoingAssociations)
                    .HasForeignKey(x => x.SourceOrganisationUnitId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TargetOrganisationUnit)
                    .WithMany(x => x.IncomingAssociations)
                    .HasForeignKey(x => x.TargetOrganisationUnitId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new
                {
                    x.SourceOrganisationUnitId,
                    x.TargetOrganisationUnitId,
                    x.AssociationType
                }).IsUnique();
            });

            mb.Entity<Address>(e =>
            {
                e.Property(x => x.City).HasMaxLength(150);
                e.Property(x => x.Street).HasMaxLength(250);
                e.Property(x => x.PostalCode).HasMaxLength(30);

                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy)
                    .HasMaxLength(100);
                e.Property(x => x.LastModified);
                e.Property(x => x.LastModifiedBy)
                    .HasMaxLength(100);
            });

            mb.Entity<UserPermission>(e =>
            {
                e.Property(x => x.PermissionLevel)
                    .HasMaxLength(50)
                    .IsRequired();

                e.HasIndex(x => new { x.UserId, x.OrganisationUnitId })
                    .IsUnique();

                e.HasOne(x => x.User)
                    .WithMany(u => u.UserPermissions)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.OrganisationUnit)
                    .WithMany()
                    .HasForeignKey(x => x.OrganisationUnitId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy)
                    .HasMaxLength(100);
                e.Property(x => x.LastModified);
                e.Property(x => x.LastModifiedBy)
                    .HasMaxLength(100);
            });

            mb.Entity<OrganisationUnitRole>(e =>
            {
                e.HasIndex(x => new { x.OrganisationUnitId, x.RoleId })
                    .IsUnique();

                e.HasOne(x => x.OrganisationUnit)
                    .WithMany(r => r.OrganisationUnitRoles)
                    .HasForeignKey(x => x.OrganisationUnitId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Role)
                    .WithMany(r => r.OrganisationUnitRoles)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy)
                    .HasMaxLength(100);
                e.Property(x => x.LastModified);
                e.Property(x => x.LastModifiedBy)
                    .HasMaxLength(100);
            });

            mb.Entity<UserIdentifierType>(e =>
            {
                e.ToTable("UserIdentifierType");

                e.HasKey(x => x.Id);

                e.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                e.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();
                e.HasIndex(x => x.Code)
                    .IsUnique();
                e.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();
                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy)
                    .HasMaxLength(100);
                e.Property(x => x.LastModifiedBy)
                    .HasMaxLength(100);
            });

            mb.Entity<UserIdentifier>(e =>
            {
                e.ToTable("UserIdentifier");

                e.HasKey(x => x.Id);

                e.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                e.Property(x => x.Value)
                    .HasMaxLength(250)
                    .IsRequired();

                e.Property(x => x.UserId)
                    .IsRequired();

                e.Property(x => x.UserIdentifierTypeId)
                    .IsRequired();

                e.HasIndex(x => new { x.UserId, x.UserIdentifierTypeId })
                    .IsUnique();

                e.HasOne(x => x.User)
                    .WithMany(u => u.UserIdentifiers)   
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.UserIdentifierType)
                    .WithMany()
                    .HasForeignKey(x => x.UserIdentifierTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.CreatedBy)
                    .HasMaxLength(100);
                e.Property(x => x.LastModifiedBy)
                    .HasMaxLength(100);
            });
        }
    }
}