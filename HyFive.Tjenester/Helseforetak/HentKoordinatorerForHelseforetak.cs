using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.User;
using HyFive.Modeller.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class HentKoordinatorerForHelseforetak
    {
        public class Query : IRequest<HealthcareInstitutionCoordinator[]>
        {
            public int HelseforetakId { get; set; }
        }

        public class Handler : IRequestHandler<Query, HealthcareInstitutionCoordinator[]>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }


            public async Task<HealthcareInstitutionCoordinator[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var koordinatorerForInstitusjonerIHelseforetak = HentKoordinatorerForInstitusjonerIHelseforetak(request.HelseforetakId);

                List<HealthcareInstitutionCoordinator> koordinatorForHelseforetakListe = LagKoordinatorerForHelseForetakListe(koordinatorerForInstitusjonerIHelseforetak);

                return koordinatorForHelseforetakListe.ToArray();
            }

            private static List<HealthcareInstitutionCoordinator> LagKoordinatorerForHelseForetakListe(List<Koordinator> koordinatorerForInstitusjonerIHelseforetak)
            {
                var koordinatorForHelseforetakListe = new List<HealthcareInstitutionCoordinator>();

                foreach (var koordinator in koordinatorerForInstitusjonerIHelseforetak)
                {
                    var koordinatorForHelseforetak = koordinatorForHelseforetakListe.FirstOrDefault(k => (!string.IsNullOrEmpty(k.HPRNumber) && k.HPRNumber == koordinator.HPRNummer) ||
                                                                                                         (!string.IsNullOrEmpty(k.IdentityPseudonym) && k.IdentityPseudonym == koordinator.IdentPseudonym));
                    if (koordinatorForHelseforetak == null)
                    {
                        koordinatorForHelseforetak = LagKoordinatorForHelseforetak(koordinatorForHelseforetakListe, koordinator);
                    }

                    LeggTilInstitusjon(koordinator, koordinatorForHelseforetak);
                }

                return koordinatorForHelseforetakListe;
            }

            private static void LeggTilInstitusjon(Koordinator koordinator, HealthcareInstitutionCoordinator koordinatorForHelseforetak)
            {
                var institusjonRapport = new InstitutionReport
                {
                    Abbreviation = koordinator.Institusjon.Forkortelse,
                    HERId = koordinator.Institusjon.HERId,
                    Id = koordinator.Institusjon.Id,
                    Name = koordinator.Institusjon.Navn,
                    InstitutionType = new InstitutionType
                    {
                        Id = koordinator.Institusjon.Institusjontype.Id,
                        Code = koordinator.Institusjon.Institusjontype.Kode,
                        Name = koordinator.Institusjon.Institusjontype.Navn
                    }
                };

                koordinatorForHelseforetak.Institutions.Add(institusjonRapport);
            }

            private static HealthcareInstitutionCoordinator LagKoordinatorForHelseforetak(List<HealthcareInstitutionCoordinator> koordinatorForHelseforetakListe, Koordinator koordinator)
            {
                var koordinatorForHelseforetak = new HealthcareInstitutionCoordinator
                {
                    FirstName = koordinator.Fornavn,
                    Surname = koordinator.Etternavn,
                    Email = koordinator.Epost,
                    HPRNumber = koordinator.HPRNummer,
                    IdentityPseudonym = koordinator.IdentPseudonym,
                    CreatedTime = koordinator.Opprettettidspunkt,
                    Institutions = new List<InstitutionReport>()
                };
                koordinatorForHelseforetakListe.Add(koordinatorForHelseforetak);
                return koordinatorForHelseforetak;
            }

            private List<Koordinator> HentKoordinatorerForInstitusjonerIHelseforetak(int helseforetakId)
            {
                return _context.User.OfType<Koordinator>()
                    .AsNoTracking()
                    .Include(b => b.Institusjon)
                        .ThenInclude(i => i.Helseforetak)
                    .Include(b => b.Institusjon)
                        .ThenInclude(i => i.Institusjontype)
                    .Where(b => b.Institusjon.Helseforetak.Id == helseforetakId &&
                                b.ErDeaktivert == false)
                    .OrderBy(b => b.Etternavn)
                    .ToList();
            }
        }
    }
}
