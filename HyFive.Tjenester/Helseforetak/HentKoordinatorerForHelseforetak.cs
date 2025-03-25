using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Bruker;
using HyFive.Modeller.V1.Institusjon;
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
        public class Query : IRequest<KoordinatorForHelseforetak[]>
        {
            public int HelseforetakId { get; set; }
        }

        public class Handler : IRequestHandler<Query, KoordinatorForHelseforetak[]>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }


            public async Task<KoordinatorForHelseforetak[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var koordinatorerForInstitusjonerIHelseforetak = HentKoordinatorerForInstitusjonerIHelseforetak(request.HelseforetakId);

                List<KoordinatorForHelseforetak> koordinatorForHelseforetakListe = LagKoordinatorerForHelseForetakListe(koordinatorerForInstitusjonerIHelseforetak);

                return koordinatorForHelseforetakListe.ToArray();
            }

            private static List<KoordinatorForHelseforetak> LagKoordinatorerForHelseForetakListe(List<Koordinator> koordinatorerForInstitusjonerIHelseforetak)
            {
                var koordinatorForHelseforetakListe = new List<KoordinatorForHelseforetak>();

                foreach (var koordinator in koordinatorerForInstitusjonerIHelseforetak)
                {
                    var koordinatorForHelseforetak = koordinatorForHelseforetakListe.FirstOrDefault(k => (!string.IsNullOrEmpty(k.HPRNummer) && k.HPRNummer == koordinator.HPRNummer) ||
                                                                                                         (!string.IsNullOrEmpty(k.IdentPseudonym) && k.IdentPseudonym == koordinator.IdentPseudonym));
                    if (koordinatorForHelseforetak == null)
                    {
                        koordinatorForHelseforetak = LagKoordinatorForHelseforetak(koordinatorForHelseforetakListe, koordinator);
                    }

                    LeggTilInstitusjon(koordinator, koordinatorForHelseforetak);
                }

                return koordinatorForHelseforetakListe;
            }

            private static void LeggTilInstitusjon(Koordinator koordinator, KoordinatorForHelseforetak koordinatorForHelseforetak)
            {
                var institusjonRapport = new InstitusjonRapport
                {
                    Forkortelse = koordinator.Institusjon.Forkortelse,
                    HERId = koordinator.Institusjon.HERId,
                    Id = koordinator.Institusjon.Id,
                    Navn = koordinator.Institusjon.Navn,
                    Institusjontype = new InstitusjonType
                    {
                        Id = koordinator.Institusjon.Institusjontype.Id,
                        Kode = koordinator.Institusjon.Institusjontype.Kode,
                        Navn = koordinator.Institusjon.Institusjontype.Navn
                    }
                };

                koordinatorForHelseforetak.Institusjoner.Add(institusjonRapport);
            }

            private static KoordinatorForHelseforetak LagKoordinatorForHelseforetak(List<KoordinatorForHelseforetak> koordinatorForHelseforetakListe, Koordinator koordinator)
            {
                var koordinatorForHelseforetak = new KoordinatorForHelseforetak
                {
                    Fornavn = koordinator.Fornavn,
                    Etternavn = koordinator.Etternavn,
                    Epost = koordinator.Epost,
                    HPRNummer = koordinator.HPRNummer,
                    IdentPseudonym = koordinator.IdentPseudonym,
                    Opprettettidspunkt = koordinator.Opprettettidspunkt,
                    Institusjoner = new List<InstitusjonRapport>()
                };
                koordinatorForHelseforetakListe.Add(koordinatorForHelseforetak);
                return koordinatorForHelseforetak;
            }

            private List<Koordinator> HentKoordinatorerForInstitusjonerIHelseforetak(int helseforetakId)
            {
                return _context.Bruker.OfType<Koordinator>()
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
