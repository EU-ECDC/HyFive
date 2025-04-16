using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Services.User;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Helseforetak
{
    public class OpprettKoordinatorForHelseforetak
    {
        public class Command : IRequest<Status>
        {
            public HealthcareInstitutionCoordinator Koordinator { get; set; }
            public int HelseforetakId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Status>
        {
            private readonly HandHygieneContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(HandHygieneContext context, ILogger<Handler> logger)
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

                    var institusjonIdListe = command.Koordinator.Institutions.Select(x => x.Id);

                    foreach (var institusjonId in institusjonIdListe)
                    {
                        var koordinator = HentKoordinator(institusjonId, command.Koordinator.HPRNumber, command.Koordinator.IdentityPseudonym);
                        if (koordinator != null)
                        {
                            if (koordinator.ErDeaktivert)
                                koordinator.ErDeaktivert = false;
                        }
                        else
                        {
                            var nyKoordinator = LagKoordinatorForInstitusjon(command.Koordinator, institusjonId);

                            // Coordinator skal også være observatør for samme institusjon
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

            private Koordinator LagKoordinatorForInstitusjon(HealthcareInstitutionCoordinator koordinator, int institusjonId)
            {
                var institusjon = _context.Institution.FirstOrDefault(i => i.Id == institusjonId);
                var nyKoordinator = new Koordinator
                {
                    Fornavn = koordinator.FirstName,
                    Etternavn = koordinator.Surname,
                    Epost = koordinator.Email,
                    HPRNummer = koordinator.HPRNumber,
                    IdentPseudonym = koordinator.IdentityPseudonym,
                    Institusjon = institusjon
                };
                return nyKoordinator;
            }

            private Observator LagObservatorForInstitusjon(HealthcareInstitutionCoordinator koordinator, int institusjonId)
            { 
                var institusjon = _context.Institution.FirstOrDefault(i => i.Id == institusjonId);

                var observator = new Observator
                {
                    Fornavn = koordinator.FirstName,
                    Etternavn = koordinator.Surname,
                    Epost = koordinator.Email,
                    HPRNummer = koordinator.HPRNumber,
                    IdentPseudonym = koordinator.IdentityPseudonym,
                    Institusjon = institusjon
                };

                return observator;
            }

            private Koordinator HentKoordinator(int institusjonId, string hprNummer, string identPseudonym)
            {
                var koordinator = _context.Coordinator.FirstOrDefault(k => k.Institusjon.Id == institusjonId &&
                                                                        ((!string.IsNullOrEmpty(k.HPRNumber) &&
                                                                        k.HPRNumber == hprNummer) ||
                                                                        (!string.IsNullOrEmpty(k.IdentityPseudonym) &&
                                                                        k.IdentityPseudonym == identPseudonym)));
                return koordinator;
            }

            private bool KanKoordinatorOppdateres(HealthcareInstitutionCoordinator koordinator, out string feilmelding)
            {
                feilmelding = "";

                if (string.IsNullOrWhiteSpace(koordinator.FirstName))
                {
                    feilmelding = "Fornavn må fylles ut";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(koordinator.Surname))
                {
                    feilmelding = "Etternavn må fylles ut";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(koordinator.HPRNumber) && string.IsNullOrWhiteSpace(koordinator.IdentityPseudonym))
                {
                    feilmelding = "Hpr nummer eller identpseudonym må fylles ut";
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(koordinator.IdentityPseudonym) && !UserValidator.IsValidIdentityPseudonym(koordinator.IdentityPseudonym))
                {
                    feilmelding = "Identpseudonym er ikke gyldig";
                    return false;
                }

                return true;
            }
        }
    }
}
