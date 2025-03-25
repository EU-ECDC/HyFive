using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1;
using HyFive.Modeller.V1.Bruker;
using HyFive.Tjenester.Bruker;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class OppdaterKoordinatorForHelseforetak
    {
        public class Command : IRequest<Status>
        {
            public KoordinatorForHelseforetak Koordinator { get; set; }
            public int HelseforetakId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Status>
        {
            private readonly HandhygieneContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(HandhygieneContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }
            public async Task<Status> Handle(Command command, CancellationToken cancellationToken)
            {
                try
                {
                    if(!KanKoordinatorOppdateres(command.Koordinator, out var feilmelding))
                        return new Status { Suksess = false, Feilmelding = feilmelding };

                    var institusjonIdListe = command.Koordinator.Institusjoner.Select(x => x.Id);
                    List<Koordinator> koordinatorer = FinnKoordinatorForInstitusjonIHelseforetak(command);
                    OppdaterKoordinatorerForInstitusjonIHelseForetak(command.Koordinator, koordinatorer);

                    OppdaterInstitusjonForKoordinator(command.Koordinator, institusjonIdListe, koordinatorer);

                    _context.SaveChanges();
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Feil under oppdatering av koordinator");
                    return new Status { Suksess = false, Feilmelding = e.Message };
                }

                return new Status { Suksess = true };
            }

            private bool KanKoordinatorOppdateres(KoordinatorForHelseforetak koordinator, out string feilmelding)
            {
                feilmelding = "";

                if(string.IsNullOrWhiteSpace(koordinator.Fornavn))
                {
                    feilmelding = "Fornavn må fylles ut";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(koordinator.Etternavn))
                {
                    feilmelding = "Etternavn må fylles ut";
                    return false;
                }

                if(string.IsNullOrWhiteSpace(koordinator.EndretHPRNummer) && string.IsNullOrWhiteSpace(koordinator.EndretIdentPseudonym))
                {
                    feilmelding = "Hpr nummer eller identpseudonym må fylles ut";
                    return false;
                }

                if(!string.IsNullOrWhiteSpace(koordinator.EndretIdentPseudonym) && !BrukerValidator.ErGyldigIdentPseudonym(koordinator.EndretIdentPseudonym))
                {
                    feilmelding = "Identpseudonym er ikke gyldig";
                    return false;
                }

                return true;
            }

            private void OppdaterInstitusjonForKoordinator(KoordinatorForHelseforetak koordinatorForHelseForetak, IEnumerable<int> institusjonIdListe, List<Koordinator> koordinatorer)
            {
                if (koordinatorForHelseForetak.ErDeaktivert)
                    return;

                DeaktiverKooordinatorForInstitusjonSomIkkeErILista(koordinatorer, institusjonIdListe);

                foreach (var institusjonId in institusjonIdListe)
                {
                    var koordinator = HentKoordinator(institusjonId, koordinatorForHelseForetak.HPRNummer, koordinatorForHelseForetak.IdentPseudonym);
                    if (koordinator != null)
                    {
                        if (koordinator.ErDeaktivert)
                            koordinator.ErDeaktivert = false;
                    }
                    else
                    {
                        var nyKoordinator = LagKoordinatorForInstitusjon(koordinatorForHelseForetak, institusjonId);
                        _context.Add(nyKoordinator);
                    }
                }
            }

            private static void OppdaterKoordinatorerForInstitusjonIHelseForetak(KoordinatorForHelseforetak koordinatorForHelseforetak, List<Koordinator> koordinatorer)
            {
                foreach (var koordinator in koordinatorer)
                {
                    koordinator.Fornavn = koordinatorForHelseforetak.Fornavn;
                    koordinator.Etternavn = koordinatorForHelseforetak.Etternavn;
                    koordinator.Epost = koordinatorForHelseforetak.Epost;
                    koordinator.HPRNummer = koordinatorForHelseforetak.EndretHPRNummer;
                    koordinator.IdentPseudonym = koordinatorForHelseforetak.EndretIdentPseudonym;
                    koordinator.ErDeaktivert = koordinatorForHelseforetak.ErDeaktivert;
                }
            }

            private List<Koordinator> FinnKoordinatorForInstitusjonIHelseforetak(Command request)
            {
                return _context.Koordinator.Where(k => k.Institusjon.Helseforetak.Id == request.HelseforetakId &&
                                                    ((!string.IsNullOrEmpty(k.HPRNummer) &&
                                                    k.HPRNummer == request.Koordinator.HPRNummer) ||
                                                    (!string.IsNullOrEmpty(k.IdentPseudonym) &&
                                                    k.IdentPseudonym == request.Koordinator.IdentPseudonym))).ToList();
            }

            private Koordinator LagKoordinatorForInstitusjon(KoordinatorForHelseforetak koordinator, int institusjonId)
            {
                var institusjon = _context.Institusjon.FirstOrDefault(i => i.Id == institusjonId);
                var nyKoordinator = new Koordinator
                {
                    Fornavn = koordinator.Fornavn,
                    Etternavn = koordinator.Etternavn,
                    HPRNummer = koordinator.HPRNummer,
                    IdentPseudonym = koordinator.IdentPseudonym,
                    Institusjon = institusjon
                };
                return nyKoordinator;
            }

            private void DeaktiverKooordinatorForInstitusjonSomIkkeErILista(List<Koordinator> koordinatorer, IEnumerable<int> institusjonIdListe)
            {
                var koordinatorerSomIkkeErIListe = koordinatorer.Where(k => !institusjonIdListe.Contains(k.Id));

                koordinatorerSomIkkeErIListe.All(k => k.ErDeaktivert = true);
            }

            private Koordinator HentKoordinator(int institusjonId, string hprNummer, string identPseudonym)
            {
                var koordinator = _context.Koordinator.FirstOrDefault(k => k.Institusjon.Id == institusjonId &&
                                                                        ((!string.IsNullOrEmpty(k.HPRNummer) &&
                                                                        k.HPRNummer == hprNummer) ||
                                                                        (!string.IsNullOrEmpty(k.IdentPseudonym) &&
                                                                        k.IdentPseudonym == identPseudonym)));
                return koordinator;
            }
        }
    }
}
