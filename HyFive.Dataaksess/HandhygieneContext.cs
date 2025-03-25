using HyFive.Domene.Bruker;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;
using HyFive.Domene.Observasjon.Hansker;
using HyFive.Domene.Sesjon;
using HyFive.Domene.Sted;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Dataaksess
{
    public class HandhygieneContext : DbContext
    {
        public HandhygieneContext(DbContextOptions<HandhygieneContext> options) : base(options)
        {
        }

        public DbSet<Sesjon> Sesjon { get; set; }
        public DbSet<FireIndikasjonerSesjon> FireIndikasjonerSesjon { get; set; }
        public DbSet<HandsmykkeSesjon> HandsmykkeSesjon { get; set; }
        public DbSet<BeskyttelsesutstyrSesjon> BeskyttelsesutstyrSesjon { get; set; }
        public DbSet<HanskeSesjon> HanskeSesjon { get; set; }
        public DbSet<Rolle> Rolle { get; set; }
        public DbSet<Bruker> Bruker { get; set; }
        public DbSet<Koordinator> Koordinator { get; set; }
        public DbSet<Observator> Observator { get; set; }
        public DbSet<FhiAdmin> FhiAdmin { get; set; }
        public DbSet<ForesporselOmBrukertilgang> ForesporselOmBrukertilgang { get; set; }
        public DbSet<OverforingstatusType> OverforingstatusType { get; set; }
        public DbSet<Institusjon> Institusjon { get; set; }
        public DbSet<InstitusjonType> InstitusjonType { get; set; }
        public DbSet<AvdelingType> AvdelingType { get; set; }
        public DbSet<PredefinertKommentar> PredefinertKommentar { get; set; }
        public DbSet<IndikasjonType> Indikasjon { get; set; }
        public DbSet<Avdeling> Avdeling { get; set; }
        public DbSet<AktivitetType> AktivitetType { get; set; }
        public DbSet<HandsmykkeType> HandsmykkeType { get; set; }
        public DbSet<BeskyttelsesutstyrsettingType> BeskyttelsesutstyrsettingType { get; set; }
        public DbSet<BeskyttelsesutstyrType> BeskyttelsesutstyrType { get; set; }
        public DbSet<HanskeMedIndikasjonType> HanskeMedIndikasjonType { get; set; }
        public DbSet<HanskeUtenIndikasjonType> HanskeUtenIndikasjonType { get; set; }
        public DbSet<HandhygieneEtterHanskebrukType> HandhygieneEtterHanskebrukType { get; set; }
        public DbSet<Klinikk> Klinikk { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<FireIndikasjonerObservasjon> FireIndikasjonerObservasjon { get; set; }
        public DbSet<HanskeObservasjon> HanskeObservasjon { get; set; }
        public DbSet<HandsmykkeObservasjon> HandsmykkeObservasjon { get; set; }
        public DbSet<BeskyttelsesutstyrObservasjon> BeskyttelsesutstyrObservasjon { get; set; }
        public DbSet<Aktivitet> Aktivitet { get; set; }
        public DbSet<IndikasjonType> IndikasjonType { get; set; }
        public DbSet<FeilbrukType> FeilbrukType { get; set; }
        public DbSet<Helseforetak> Helseforetak { get; set; }
        public DbSet<RegionaltHelseforetak> RegionaltHelseforetak { get; set; }
        public DbSet<Kommune> Kommune { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Sesjon>().Property(s => s.Kommentar).HasMaxLength(1000);
            mb.Entity<Sesjon>().HasIndex(s => s.Discriminator);
            mb.Entity<Sesjon>().HasIndex(s => s.Opprettettidspunkt);
            mb.Entity<Sesjon>().HasIndex(s => s.Starttidspunkt);

            mb.Entity<FireIndikasjonerObservasjon>().Property(fio => fio.Kommentar).HasMaxLength(1000);
            mb.Entity<FireIndikasjonerObservasjon>().HasIndex(fio => fio.Opprettettidspunkt);
            mb.Entity<FireIndikasjonerObservasjon>().HasIndex(fio => fio.Registrerttidspunkt);

            mb.Entity<BeskyttelsesutstyrObservasjon>().HasIndex(bo => bo.Opprettettidspunkt);
            mb.Entity<BeskyttelsesutstyrObservasjon>().HasIndex(bo => bo.Registrerttidspunkt);

            mb.Entity<HandsmykkeObservasjon>().HasIndex(ho => ho.Opprettettidspunkt);
            mb.Entity<HandsmykkeObservasjon>().HasIndex(ho => ho.Registrerttidspunkt);

            mb.Entity<HanskeObservasjon>().HasIndex(ho => ho.Opprettettidspunkt);
            mb.Entity<HanskeObservasjon>().HasIndex(ho => ho.Registrerttidspunkt);

            mb.Entity<IndikasjonType>().Property(it => it.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<IndikasjonType>().HasIndex(it => it.Kode).IsUnique();
            mb.Entity<IndikasjonType>().Property(it => it.Navn).HasMaxLength(50);
            mb.Entity<IndikasjonType>().HasIndex(it => it.Navn);
            mb.Entity<IndikasjonType>().Property(it => it.Nummer).HasMaxLength(2);

            mb.Entity<OverforingstatusType>().Property(ost => ost.Navn).HasMaxLength(50);
            mb.Entity<OverforingstatusType>().HasIndex(ost => ost.Navn);
            mb.Entity<OverforingstatusType>().Property(ost => ost.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<OverforingstatusType>().HasIndex(ost => ost.Kode).IsUnique();

            mb.Entity<Rolle>().Property(r => r.Navn).HasMaxLength(50);
            mb.Entity<Rolle>().Property(r => r.Beskrivelse).HasMaxLength(250);
            mb.Entity<Rolle>().HasIndex(r => r.Navn);

            mb.Entity<AktivitetType>().Property(at => at.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<AktivitetType>().HasIndex(at => at.Kode).IsUnique();
            mb.Entity<AktivitetType>().Property(at => at.Navn).HasMaxLength(100);
            mb.Entity<AktivitetType>().HasIndex(at => at.Navn);

            mb.Entity<HandsmykkeType>().Property(ht => ht.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<HandsmykkeType>().HasIndex(ht => ht.Kode).IsUnique();
            mb.Entity<HandsmykkeType>().Property(ht => ht.Navn).HasMaxLength(100);

            mb.Entity<InstitusjonType>().Property(it => it.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<InstitusjonType>().HasIndex(it => it.Kode).IsUnique();
            mb.Entity<InstitusjonType>().Property(it => it.Navn).HasMaxLength(100);
            mb.Entity<InstitusjonType>().HasIndex(it => it.Navn);

            mb.Entity<AvdelingType>().Property(at => at.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<AvdelingType>().HasIndex(at => at.Kode).IsUnique();
            mb.Entity<AvdelingType>().Property(at => at.Navn).HasMaxLength(100);
            mb.Entity<AvdelingType>().HasIndex(at => at.Navn);

            mb.Entity<Beskyttelsesutstyr>().Property(i => i.Kommentar).HasMaxLength(1000);

            mb.Entity<BeskyttelsesutstyrType>().Property(but => but.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<BeskyttelsesutstyrType>().HasIndex(but => but.Kode).IsUnique();
            mb.Entity<BeskyttelsesutstyrType>().Property(but => but.Navn).HasMaxLength(100);
            mb.Entity<BeskyttelsesutstyrType>().HasIndex(but => but.Navn);
            

            mb.Entity<BeskyttelsesutstyrsettingType>().Property(bust => bust.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<BeskyttelsesutstyrsettingType>().HasIndex(bust => bust.Kode).IsUnique();
            mb.Entity<BeskyttelsesutstyrsettingType>().Property(bust => bust.Navn).HasMaxLength(100);
            mb.Entity<BeskyttelsesutstyrsettingType>().HasIndex(bust => bust.Navn);
            

            mb.Entity<HanskeMedIndikasjonType>().Property(h => h.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<HanskeMedIndikasjonType>().HasIndex(h => h.Kode).IsUnique();
            mb.Entity<HanskeMedIndikasjonType>().Property(h => h.Navn).HasMaxLength(100);
            mb.Entity<HanskeMedIndikasjonType>().HasIndex(h => h.Navn);

            mb.Entity<HanskeUtenIndikasjonType>().Property(h => h.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<HanskeUtenIndikasjonType>().HasIndex(h => h.Kode).IsUnique();
            mb.Entity<HanskeUtenIndikasjonType>().Property(h => h.Navn).HasMaxLength(100);
            mb.Entity<HanskeUtenIndikasjonType>().HasIndex(h => h.Navn);

            mb.Entity<HandhygieneEtterHanskebrukType>().Property(h => h.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<HandhygieneEtterHanskebrukType>().HasIndex(h => h.Kode).IsUnique();
            mb.Entity<HandhygieneEtterHanskebrukType>().Property(h => h.Navn).HasMaxLength(100);
            mb.Entity<HandhygieneEtterHanskebrukType>().HasIndex(h => h.Navn);
            

            mb.Entity<Region>().Property(rt => rt.Kode).HasMaxLength(50).IsRequired();
            mb.Entity<Region>().HasIndex(rt => rt.Kode).IsUnique();
            mb.Entity<Region>().Property(rt => rt.Navn).HasMaxLength(100);
            mb.Entity<Region>().HasIndex(rt => rt.Navn);

            mb.Entity<FeilbrukType>().Property(i => i.Navn).HasMaxLength(100);
            mb.Entity<FeilbrukType>().HasIndex(i => i.Navn);

            mb.Entity<Institusjon>().Property(i => i.Navn).HasMaxLength(250);
            mb.Entity<Institusjon>().HasIndex(i => i.Navn);
            mb.Entity<Institusjon>().Property(i => i.HERId).HasMaxLength(50);
            mb.Entity<Institusjon>().Property(i => i.Forkortelse).HasMaxLength(250);
            mb.Entity<Institusjon>().HasIndex(i => i.Forkortelse);
            mb.Entity<Institusjon>().HasIndex(i => i.HERId);

            mb.Entity<PredefinertKommentar>().Property(pk => pk.Kommentar).HasMaxLength(1000);

            mb.Entity<Avdeling>().Property(a => a.Navn).HasMaxLength(250);
            mb.Entity<Avdeling>().HasIndex(a => a.Navn);

            mb.Entity<Klinikk>().Property(k => k.Navn).HasMaxLength(250);
            mb.Entity<Klinikk>().HasIndex(k => k.Navn);

            mb.Entity<Bruker>().Property(b => b.Fornavn).HasMaxLength(100).IsRequired();
            mb.Entity<Bruker>().Property(b => b.Etternavn).HasMaxLength(100).IsRequired();
            mb.Entity<Bruker>().Property(b => b.Epost).HasMaxLength(500);
            mb.Entity<Bruker>().Property(b => b.IdentPseudonym).HasMaxLength(100);
            mb.Entity<Bruker>().Property(b => b.HPRNummer).HasMaxLength(50);
            mb.Entity<Bruker>().HasIndex(b => b.IdentPseudonym);
            mb.Entity<Bruker>().HasIndex(b => b.HPRNummer);

            mb.Entity<ForesporselOmBrukertilgang>().Property(b => b.BrukerFornavn).HasMaxLength(100).IsRequired();
            mb.Entity<ForesporselOmBrukertilgang>().Property(b => b.BrukerEtternavn).HasMaxLength(100).IsRequired();
            mb.Entity<ForesporselOmBrukertilgang>().Property(b => b.IdentPseudonym).HasMaxLength(100).IsRequired();
            mb.Entity<ForesporselOmBrukertilgang>().Property(b => b.HPRNummer).HasMaxLength(50);
            mb.Entity<ForesporselOmBrukertilgang>().HasIndex(b => b.Status);

            mb.Entity<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType>()
                .HasKey(bu => new { bu.BeskyttelsesutstyrTypeId, bu.BeskyttelsesutstyrsettingTypeId });
            mb.Entity<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType>()
                .HasOne(bu => bu.BeskyttelsesutstyrType)
                .WithMany(b => b.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper)
                .HasForeignKey(bu => bu.BeskyttelsesutstyrTypeId);
            mb.Entity<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType>()
                .HasOne(bc => bc.BeskyttelsesutstyrsettingType)
                .WithMany(c => c.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper)
                .HasForeignKey(bc => bc.BeskyttelsesutstyrsettingTypeId);

            mb.Entity<Helseforetak>().Property(h => h.Navn).HasMaxLength(250).IsRequired();

            mb.Entity<RegionaltHelseforetak>().Property(rh => rh.Navn).HasMaxLength(50);

            mb.Entity<Kommune>().Property(k => k.Nummer).HasMaxLength(4);
            mb.Entity<Kommune>().Property(k => k.Navn).HasMaxLength(100);
        }
    }
}