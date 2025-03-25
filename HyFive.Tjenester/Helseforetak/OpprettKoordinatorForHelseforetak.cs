using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1;
using HyFive.Modeller.V1.Bruker;
using HyFive.Tjenester.Bruker;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class OpprettKoordinatorForHelseforetak
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
                    if (!KanKoordinatorOppdateres(command.Koordinator, out var feilmelding))
                        return new Status { Suksess = false, Feilmelding = feilmelding };

                    var institusjonIdListe = command.Koordinator.Institusjoner.Select(x => x.Id);

                    foreach (var institusjonId in institusjonIdListe)
                    {
                        var koordinator = HentKoordinator(institusjonId, command.Koordinator.HPRNummer, command.Koordinator.IdentPseudonym);
                        if (koordinator != null)
                        {
                            if (koordinator.ErDeaktivert)
                                koordinator.ErDeaktivert = false;
                        }
                        else
                        {
                            var nyKoordinator = LagKoordinatorForInstitusjon(command.Koordinator, institusjonId);

                            // Koordinator skal også være observatør for samme institusjon
                            var nyObservator = LagObservatorForInstitusjon(command.Koordinator, institusjonId);

                            _context.Add(nyKoordinator);
                            _context.Add(nyObservator);
                        }
                    }

                _context.SaveChanges();

                
                }
                catch(Exception e)
                {
                    _logger.LogError(e, "Feil under oppdatering av koordinator");
                    return new Status { Suksess = false, Feilmelding = e.Message
        };
    }

                return new Status { Suksess = true };
            }

            private Koordinator LagKoordinatorForInstitusjon(KoordinatorForHelseforetak koordinator, int institusjonId)
            {
                var institusjon = _context.Institusjon.FirstOrDefault(i => i.Id == institusjonId);
                var nyKoordinator = new Koordinator
                {
                    Fornavn = koordinator.Fornavn,
                    Etternavn = koordinator.Etternavn,
                    Epost = koordinator.Epost,
                    HPRNummer = koordinator.HPRNummer,
                    IdentPseudonym = koordinator.IdentPseudonym,
                    Institusjon = institusjon
                };
                return nyKoordinator;
            }

            private Observator LagObservatorForInstitusjon(KoordinatorForHelseforetak koordinator, int institusjonId)
            { 
                var institusjon = _context.Institusjon.FirstOrDefault(i => i.Id == institusjonId);

                var observator = new Observator
                {
                    Fornavn = koordinator.Fornavn,
                    Etternavn = koordinator.Etternavn,
                    Epost = koordinator.Epost,
                    HPRNummer = koordinator.HPRNummer,
                    IdentPseudonym = koordinator.IdentPseudonym,
                    Institusjon = institusjon
                };

                return observator;
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

            private bool KanKoordinatorOppdateres(KoordinatorForHelseforetak koordinator, out string feilmelding)
            {
                feilmelding = "";

                if (string.IsNullOrWhiteSpace(koordinator.Fornavn))
                {
                    feilmelding = "Fornavn må fylles ut";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(koordinator.Etternavn))
                {
                    feilmelding = "Etternavn må fylles ut";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(koordinator.HPRNummer) && string.IsNullOrWhiteSpace(koordinator.IdentPseudonym))
                {
                    feilmelding = "Hpr nummer eller identpseudonym må fylles ut";
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(koordinator.IdentPseudonym) && !BrukerValidator.ErGyldigIdentPseudonym(koordinator.IdentPseudonym))
                {
                    feilmelding = "Identpseudonym er ikke gyldig";
                    return false;
                }

                return true;
            }
        }
    }
}
