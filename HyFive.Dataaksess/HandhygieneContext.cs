using HyFive.Domain.User;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Session;
using HyFive.Domain.Place;
using Microsoft.EntityFrameworkCore;

namespace HyFive.DataAccess
{
    public class HandHygieneContext : DbContext
    {
        public HandHygieneContext(DbContextOptions<HandHygieneContext> options) : base(options)
        {
        }

        public DbSet<Session> Session  { get; set; }
        public DbSet<FourIndicationsSession> FourIndicationsSession { get; set; }
        public DbSet<HandJewelrySession> HandJewelrySession { get; set; }
        public DbSet<ProtectiveEquipmentSession> ProtectiveEquipmentSession { get; set; }
        public DbSet<GloveSession> GloveSession { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Coordinator> Coordinator { get; set; }
        public DbSet<Observer> Observer { get; set; }
        public DbSet<FhiAdmin> FhiAdmin { get; set; }
        public DbSet<UserAccessRequest> UserAccessRequest { get; set; }
        public DbSet<TransmissionStatusType> TransmissionStatusType { get; set; }
        public DbSet<Institution> Institution { get; set; }
        public DbSet<InstitutionType> InstitutionType { get; set; }
        public DbSet<SectionType> SectionType { get; set; }
        public DbSet<PredefinedComments> PredefinedComments { get; set; }
        public DbSet<IndicationTypes> Indication { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<ActivityType> ActivityType { get; set; }
        public DbSet<HandJewelryType> HandJewelryType { get; set; }
        public DbSet<ProtectiveEquipmentSettingType> ProtectiveEquipmentSettingType { get; set; }
        public DbSet<ProtectiveEquipmentType> ProtectiveEquipmentType { get; set; }
        public DbSet<IndicatedGloveType> IndicatedGloveType { get; set; }
        public DbSet<GeneralPurposeGloveType> GeneralPurposeGloveType { get; set; }
        public DbSet<PostGloveHandHygiene> PostGloveHandHygiene { get; set; }
        public DbSet<Clinic> Clinic { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<FourIndicationsObservation> FourIndicationsObservation { get; set; }
        public DbSet<GloveObservation> GloveObservation { get; set; }
        public DbSet<HandJewelryObservation> HandJewelryObservation { get; set; }
        public DbSet<ProtectiveEquipmentObservation> ProtectiveEquipmentObservation { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<IndicationTypes> IndicationTypes { get; set; }
        public DbSet<MisuseType> MisuseType { get; set; }
        public DbSet<HealthcareProvider> HealthcareProvider { get; set; }
        public DbSet<RegionaltHealthcareProvider> RegionaltHealthcareProvider { get; set; }
        public DbSet<Municipality> Municipality { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Session>().Property(s => s.Comment).HasMaxLength(1000);
            mb.Entity<Session>().HasIndex(s => s.Discriminator);
            mb.Entity<Session>().HasIndex(s => s.CreatedTime);
            mb.Entity<Session>().HasIndex(s => s.StartTime);

            mb.Entity<FourIndicationsObservation>().Property(fio => fio.Comment).HasMaxLength(1000);
            mb.Entity<FourIndicationsObservation>().HasIndex(fio => fio.CreatedTime);
            mb.Entity<FourIndicationsObservation>().HasIndex(fio => fio.RegistrationTime);

            mb.Entity<ProtectiveEquipmentObservation>().HasIndex(bo => bo.CreatedTime);
            mb.Entity<ProtectiveEquipmentObservation>().HasIndex(bo => bo.RegistrationTime);

            mb.Entity<HandJewelryObservation>().HasIndex(ho => ho.CreatedTime);
            mb.Entity<HandJewelryObservation>().HasIndex(ho => ho.RegistrationTime);

            mb.Entity<GloveObservation>().HasIndex(ho => ho.CreatedTime);
            mb.Entity<GloveObservation>().HasIndex(ho => ho.RegistrationTime);

            mb.Entity<IndicationTypes>().Property(it => it.Code).HasMaxLength(50).IsRequired();
            mb.Entity<IndicationTypes>().HasIndex(it => it.Code).IsUnique();
            mb.Entity<IndicationTypes>().Property(it => it.Name).HasMaxLength(50);
            mb.Entity<IndicationTypes>().HasIndex(it => it.Name);
            mb.Entity<IndicationTypes>().Property(it => it.Number).HasMaxLength(2);

            mb.Entity<TransmissionStatusType>().Property(ost => ost.Name).HasMaxLength(50);
            mb.Entity<TransmissionStatusType>().HasIndex(ost => ost.Name);
            mb.Entity<TransmissionStatusType>().Property(ost => ost.Code).HasMaxLength(50).IsRequired();
            mb.Entity<TransmissionStatusType>().HasIndex(ost => ost.Code).IsUnique();

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

            mb.Entity<InstitutionType>().Property(it => it.Code).HasMaxLength(50).IsRequired();
            mb.Entity<InstitutionType>().HasIndex(it => it.Code).IsUnique();
            mb.Entity<InstitutionType>().Property(it => it.Name).HasMaxLength(100);
            mb.Entity<InstitutionType>().HasIndex(it => it.Name);

            mb.Entity<SectionType>().Property(at => at.Code).HasMaxLength(50).IsRequired();
            mb.Entity<SectionType>().HasIndex(at => at.Code).IsUnique();
            mb.Entity<SectionType>().Property(at => at.Name).HasMaxLength(100);
            mb.Entity<SectionType>().HasIndex(at => at.Name);

            mb.Entity<ProtectiveEquipment>().Property(i => i.Comment).HasMaxLength(1000);

            mb.Entity<ProtectiveEquipmentType>().Property(but => but.Code).HasMaxLength(50).IsRequired();
            mb.Entity<ProtectiveEquipmentType>().HasIndex(but => but.Code).IsUnique();
            mb.Entity<ProtectiveEquipmentType>().Property(but => but.Name).HasMaxLength(100);
            mb.Entity<ProtectiveEquipmentType>().HasIndex(but => but.Name);
            

            mb.Entity<ProtectiveEquipmentSettingType>().Property(bust => bust.Code).HasMaxLength(50).IsRequired();
            mb.Entity<ProtectiveEquipmentSettingType>().HasIndex(bust => bust.Code).IsUnique();
            mb.Entity<ProtectiveEquipmentSettingType>().Property(bust => bust.Name).HasMaxLength(100);
            mb.Entity<ProtectiveEquipmentSettingType>().HasIndex(bust => bust.Name);
            

            mb.Entity<IndicatedGloveType>().Property(h => h.Code).HasMaxLength(50).IsRequired();
            mb.Entity<IndicatedGloveType>().HasIndex(h => h.Code).IsUnique();
            mb.Entity<IndicatedGloveType>().Property(h => h.Name).HasMaxLength(100);
            mb.Entity<IndicatedGloveType>().HasIndex(h => h.Name);

            mb.Entity<GeneralPurposeGloveType>().Property(h => h.Code).HasMaxLength(50).IsRequired();
            mb.Entity<GeneralPurposeGloveType>().HasIndex(h => h.Code).IsUnique();
            mb.Entity<GeneralPurposeGloveType>().Property(h => h.Name).HasMaxLength(100);
            mb.Entity<GeneralPurposeGloveType>().HasIndex(h => h.Name);

            mb.Entity<PostGloveHandHygiene>().Property(h => h.Code).HasMaxLength(50).IsRequired();
            mb.Entity<PostGloveHandHygiene>().HasIndex(h => h.Code).IsUnique();
            mb.Entity<PostGloveHandHygiene>().Property(h => h.Name).HasMaxLength(100);
            mb.Entity<PostGloveHandHygiene>().HasIndex(h => h.Name);
            

            mb.Entity<Region>().Property(rt => rt.Code).HasMaxLength(50).IsRequired();
            mb.Entity<Region>().HasIndex(rt => rt.Code).IsUnique();
            mb.Entity<Region>().Property(rt => rt.Name).HasMaxLength(100);
            mb.Entity<Region>().HasIndex(rt => rt.Name);

            mb.Entity<MisuseType>().Property(i => i.Name).HasMaxLength(100);
            mb.Entity<MisuseType>().HasIndex(i => i.Name);

            mb.Entity<Institution>().Property(i => i.Name).HasMaxLength(250);
            mb.Entity<Institution>().HasIndex(i => i.Name);
            mb.Entity<Institution>().Property(i => i.HERId).HasMaxLength(50);
            mb.Entity<Institution>().Property(i => i.Abbreviation).HasMaxLength(250);
            mb.Entity<Institution>().HasIndex(i => i.Abbreviation);
            mb.Entity<Institution>().HasIndex(i => i.HERId);

            mb.Entity<PredefinedComments>().Property(pk => pk.Comment).HasMaxLength(1000);

            mb.Entity<Department>().Property(a => a.Name).HasMaxLength(250);
            mb.Entity<Department>().HasIndex(a => a.Name);

            mb.Entity<Clinic>().Property(k => k.Name).HasMaxLength(250);
            mb.Entity<Clinic>().HasIndex(k => k.Name);

            mb.Entity<User>().Property(b => b.FirstName).HasMaxLength(100).IsRequired();
            mb.Entity<User>().Property(b => b.Surname).HasMaxLength(100).IsRequired();
            mb.Entity<User>().Property(b => b.Email).HasMaxLength(500);
            mb.Entity<User>().Property(b => b.IdentityPseudonym).HasMaxLength(100);
            mb.Entity<User>().Property(b => b.HPRNumber).HasMaxLength(50);
            mb.Entity<User>().HasIndex(b => b.IdentityPseudonym);
            mb.Entity<User>().HasIndex(b => b.HPRNumber);

            mb.Entity<UserAccessRequest>().Property(b => b.UserFirstName).HasMaxLength(100).IsRequired();
            mb.Entity<UserAccessRequest>().Property(b => b.UserSurname).HasMaxLength(100).IsRequired();
            mb.Entity<UserAccessRequest>().Property(b => b.IdentPseudonym).HasMaxLength(100).IsRequired();
            mb.Entity<UserAccessRequest>().Property(b => b.HPRNummer).HasMaxLength(50);
            mb.Entity<UserAccessRequest>().HasIndex(b => b.Status);

            mb.Entity<PPEConfigurationType>()
                .HasKey(bu => new { bu.ProtectiveEquipmentTypeId, bu.ProtectiveEquipmentSettingTypeId });
            mb.Entity<PPEConfigurationType>()
                .HasOne(bu => bu.ProtectiveEquipmentType)
                .WithMany(b => b.PPEConfigurationTypes)
                .HasForeignKey(bu => bu.ProtectiveEquipmentTypeId);
            mb.Entity<PPEConfigurationType>()
                .HasOne(bc => bc.ProtectiveEquipmentSettingType)
                .WithMany(c => c.PPEConfigurationTypes)
                .HasForeignKey(bc => bc.ProtectiveEquipmentSettingTypeId);

            mb.Entity<HealthcareProvider>().Property(h => h.Name).HasMaxLength(250).IsRequired();

            mb.Entity<RegionaltHealthcareProvider>().Property(rh => rh.Name).HasMaxLength(50);

            mb.Entity<Municipality>().Property(k => k.Number).HasMaxLength(4);
            mb.Entity<Municipality>().Property(k => k.Name).HasMaxLength(100);
        }
    }
}