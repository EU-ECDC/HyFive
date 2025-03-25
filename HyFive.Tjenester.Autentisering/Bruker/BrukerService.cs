using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Autentisering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Autentisering.Bruker
{
    public class BrukerService : IBrukerService
    {
        private const string HashSalt = "handhygiene";
        private const string FornavnIClaims = "given_name";
        private const string EtternavnIClaims = "family_name";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BrukerService> _logger;
        private readonly HandhygieneContext _context;


        public BrukerService(IHttpContextAccessor httpContextAccessor,
            ILogger<BrukerService> logger,
            HandhygieneContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _context = context;
        }

        public async Task<InnloggetBruker> HentBruker()
        {
            var bruker = new InnloggetBruker()
            {
                Navn = "Grønn Vits",
            };
            
            var brukerNameForLog = bruker?.Navn;
            var hprnummer = HentHprnummer();
            var pseudonym = HentPseudonym();

            bruker.Id = CreateHash(pseudonym + bruker.Navn + HashSalt);
            bruker.ErObservator = ErObservator(hprnummer, pseudonym);
            bruker.ErKoordinator = ErKoordinator(hprnummer, pseudonym);
            bruker.ErFhiAdmin = ErFhiAdmin(pseudonym, hprnummer);
            bruker.HPRNummer = hprnummer;
            bruker.IdentPseudonym = pseudonym;
            bruker.InstitusjonsIder = await _context.Bruker.AsNoTracking().Include(k => k.Institusjon)
                .Where(HarHprEllerPseudonymOgErAktiv<Domene.Bruker.Bruker>(hprnummer, pseudonym))
                .Where(k => k.Institusjon != null)
                .Select(k => k.Institusjon.Id)
                .ToListAsync();
            bruker.Fornavn = HentFornavn();
            bruker.Etternavn = HentEtternavn();

            if (string.IsNullOrEmpty(bruker?.HPRNummer))
            {
                _logger.LogInformation("TI02: Bruker: {BrukerNameForLog} har ikke hprnummer (i HelseId)", brukerNameForLog);
            }
            return bruker;
        }

        public bool ErBrukerLoggetInn()
        {
            return true;
        }


        public bool ErKoordinator(string hprnummer, string pseudonym)
            => ErRolle<Koordinator>(hprnummer, pseudonym);

        public bool ErObservator(string hprnummer, string identPseudonym)
            => ErRolle<Observator>(hprnummer, identPseudonym);

        public bool ErKoordinatorForInstitusjon(int institusjonId, string identPseudonym, string hprnummer)
            => ErRolleForInstitusjon<Koordinator>(hprnummer, identPseudonym, institusjonId);

        public bool ErKoordinatorForInstitusjon(int institusjonId)
            => ErRolleForInstitusjon<Koordinator>(HentHprnummer(), HentPseudonym(), institusjonId);

        public bool ErKoordinatorForHelseforetak(int helseforetakId)
            => ErKoordinatorForHelseforetak(HentHprnummer(), HentPseudonym(), helseforetakId);

        public bool ErKoordinatorForInstitusjoner(int[] institusjonIder)
            => ErRolleForInstitusjoner<Koordinator>(institusjonIder);

        public bool ErKoordinatorForAvdeling(int avdelingId)
            => ErRolleForAvdeling<Koordinator>(avdelingId);

        public bool ErKoordinatorForBruker(int brukerId)
            => ErRolleForBruker<Koordinator>(HentHprnummer(), HentPseudonym(), brukerId);

        public bool ErObservatorForInstitusjon(string hprnummer, string identPseudonym, int institusjonId)
            => ErRolleForInstitusjon<Observator>(hprnummer, identPseudonym, institusjonId);

        public bool ErFhiAdminEllerKoordinator(string pseudonym, string hprnummer)
        {
            return ErFhiAdmin(pseudonym, hprnummer) || ErKoordinator(hprnummer, pseudonym);
        }

        public bool ErFhiAdmin(string identPseudonym, string hprnummer)
        {
            if (string.IsNullOrEmpty(hprnummer) && string.IsNullOrEmpty(identPseudonym))
                return false;

            bool erFhiAdmin = _context.Bruker.AsNoTracking().OfType<FhiAdmin>().AsNoTracking()
                    .Where(HarHprEllerPseudonymOgErAktiv<FhiAdmin>(hprnummer, identPseudonym)).Any();
            return erFhiAdmin;
        }

        public bool ErFhiAdmin()
            => ErFhiAdmin(HentPseudonym(), HentHprnummer());

        public bool ErKoordinatorForAvdelingEllerFhiAdmin(int avdelingId)
        {
            return ErKoordinatorForAvdeling(avdelingId) || ErFhiAdmin();
        }

        public bool ErObservatorForInstitusjon(int institusjonId)
        {
            return ErObservatorForInstitusjon(HentHprnummer(), HentPseudonym(), institusjonId);
        }

        public bool ErKoordinatorForInstitusjonEllerFhiAdmin(int institusjonId)
        {
            return ErKoordinatorForInstitusjon(institusjonId) || ErFhiAdmin();
        }

        public bool ErKoordinatorForHelseforetakEllerFhiAdmin(int helseforetakId)
        {
            return ErKoordinatorForHelseforetak(helseforetakId) || ErFhiAdmin();
        }

        public string HentHprnummer()
        {
            return "";
        }

        public bool ErKoordinatorForSesjon(string sesjonId)
        {
            var isGuid = Guid.TryParse(sesjonId, out Guid guidSesjonId);

            if (!isGuid)
            {
                throw new ArgumentException($"HelseIdBrukerService: Feil ved parsing av ID: {sesjonId}. {sesjonId} må være av typen Guid");
            }

            var institusjonId = _context.Sesjon.AsNoTracking().Include(s => s.Avdeling).ThenInclude(a => a.Institusjon)
                .FirstOrDefault(s => s.Id == guidSesjonId).Avdeling?.InstitusjonId;
            if (institusjonId != null)
            {
                return ErRolleForInstitusjon<Koordinator>(HentHprnummer(), HentPseudonym(), (int)institusjonId);
            }

            return false;
        }

        public int HentObservatorIdForInstitusjon(int institusjonId)
        {
            var hprnummer = HentHprnummer();
            var pseudonym = HentPseudonym();
            return _context.Bruker.AsNoTracking()
                .OfType<Observator>()
                .Include(o => o.Institusjon)
                .Where(HarHprEllerPseudonymOgErAktiv<Domene.Bruker.Observator>(hprnummer, pseudonym)).First(o => o.Institusjon.Id == institusjonId)?.Id ?? 0;
        }

        public Expression<Func<TBruker, bool>> HarHprEllerPseudonymOgErAktiv<TBruker>(string hprnummer, string identPseudonym) where TBruker : Domene.Bruker.Bruker
        {
            return b => ((!string.IsNullOrEmpty(hprnummer) && b.HPRNummer == hprnummer) || (!string.IsNullOrEmpty(b.IdentPseudonym) && b.IdentPseudonym == identPseudonym)) && b.ErDeaktivert == false;
        }

        private bool ErRolle<TRolle>(string hprnummer, string pseudonym) where TRolle : Domene.Bruker.Bruker
        {
            var erRolle = _context.Bruker.AsNoTracking().OfType<TRolle>()
                .Include(r => r.Institusjon)
                .Any(HarHprEllerPseudonymOgErAktiv<TRolle>(hprnummer, pseudonym));

            if (erRolle)
            {
                // Oppdater alle brukere med  med IdentPseudonym hvis de ikke har det. 
                OppdaterBrukerMedPseudonym<TRolle>(hprnummer).GetAwaiter().GetResult();
            }
            return erRolle;
        }

        private bool ErRolleForInstitusjon<TRolle>(string hprnummer, string identPseudonym, int institusjonId) where TRolle : Domene.Bruker.Bruker
        {
            return _context.Bruker.OfType<TRolle>().AsNoTracking().Include(b => b.Institusjon)
                .Where(HarHprEllerPseudonymOgErAktiv<TRolle>(hprnummer, identPseudonym))
                .Any(b => b.Institusjon.Id == institusjonId && b.Discriminator == GetDiscriminator<TRolle>());
        }

        private bool ErKoordinatorForHelseforetak(string hprnummer, string identPseudonym, int helseforetakId)
        {
            return _context.Bruker.OfType<Koordinator>().AsNoTracking().Include(b => b.Institusjon).ThenInclude(i => i.Helseforetak)
                .Where(HarHprEllerPseudonymOgErAktiv<Koordinator>(hprnummer, identPseudonym))
                .Any(b => b.Institusjon.Helseforetak.Id == helseforetakId);
        }

        private bool ErRolleForInstitusjoner<TRolle>(int[] institusjonIder) where TRolle : Domene.Bruker.Bruker
        {
            var hprnummer = HentHprnummer();
            var pseudonym = HentPseudonym();
            var koordinatorForInstitusjonIder = _context
                .Bruker
                .OfType<TRolle>()
                .AsNoTracking()
                .Include(k => k.Institusjon)
                .Where(HarHprEllerPseudonymOgErAktiv<TRolle>(hprnummer, pseudonym))
                .Where(k => k.Discriminator == GetDiscriminator<TRolle>())
                .Select(k => k.Institusjon.Id).ToList();

            if (institusjonIder == null)
            {
                return koordinatorForInstitusjonIder.Any();
            }

            return koordinatorForInstitusjonIder.Any() && institusjonIder.ToList().TrueForAll(iid => koordinatorForInstitusjonIder.Contains(iid));
        }

        private bool ErRolleForBruker<TRolle>(string hprnummer, string pseudonym, int brukerId) where TRolle : Domene.Bruker.Bruker
        {
            var institusjonId = _context
                .Bruker
                .AsNoTracking()
                .Include(b => b.Institusjon)
                .Where(b => b.Id == brukerId)
                .Select(b => b.Institusjon.Id).First();
            return ErRolleForInstitusjon<TRolle>(hprnummer, pseudonym, institusjonId);
        }

        private bool ErRolleForSesjon<TRolle>(string hprnummer, string pseudonym, Guid sesjonId) where TRolle : Domene.Bruker.Bruker
        {
            var institusjonId = _context.Sesjon
                .AsNoTracking()
                .Include(s => s.Avdeling)
                .ThenInclude(a => a.Institusjon)
                .Where(s => s.Id == sesjonId)
                .Select(b => b.Avdeling.Institusjon.Id).First();
            return ErRolleForInstitusjon<TRolle>(hprnummer, pseudonym, institusjonId);
        }

        private bool ErRolleForAvdeling<TRolle>(int avdelingId) where TRolle : Domene.Bruker.Bruker
        {
            var institusjonId = _context.Avdeling.Include(a => a.Institusjon).First(a => a.Id == avdelingId)
                .InstitusjonId;

            return ErRolleForInstitusjon<TRolle>(HentHprnummer(), HentPseudonym(), institusjonId);
        }

        /// <summary>
        /// Stjålet fra https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha512?view=net-5.0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string CreateHash(string str)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }


        public string HentPseudonym()
        {
            return "OCW6BpVN57vnbxBUE8WOOTM9FrkCaBixlD2y8FgYCag=";
        }

        private string GetDiscriminator<T>() where T : class
            => typeof(T).Name;

        private static Expression<Func<Domene.Bruker.Bruker, bool>> BrukerMedHprnummerUtenIdentpseudonym(string hprnummer)
            => b => (b.IdentPseudonym == null || b.IdentPseudonym == "") && b.HPRNummer == hprnummer;

        private async Task OppdaterBrukerMedPseudonym<TRolle>(string hprnummer) where TRolle : Domene.Bruker.Bruker
        {
            try
            {
                var brukerIderSomSkalOppdateres = _context
                    .Bruker
                    .OfType<TRolle>()
                    .AsNoTracking()
                    .Where(BrukerMedHprnummerUtenIdentpseudonym(hprnummer))
                    .ToList();

                if (brukerIderSomSkalOppdateres.Any())
                {
                    _logger.LogInformation(
                        $"TI01: {nameof(OppdaterBrukerMedPseudonym)}: Oppdaterer brukere! IDer: {string.Join(',', brukerIderSomSkalOppdateres.Select(b => b.Id))}");

                    var pseudonym = HentPseudonym();
                    foreach (var bruker in brukerIderSomSkalOppdateres)
                    {
                        bruker.IdentPseudonym = pseudonym;
                        _context.Entry(bruker).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "TE01: Feil under oppdatering av bruker som ikke hadde satt Pseudomym");
            }

        }

        public string HentFornavn()
        {
            var fornavn = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == FornavnIClaims)?.Value;
            return fornavn;
        }

        public string HentEtternavn()
        {
            var fornavn = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == EtternavnIClaims)?.Value;
            return fornavn;
        }
    }
}
