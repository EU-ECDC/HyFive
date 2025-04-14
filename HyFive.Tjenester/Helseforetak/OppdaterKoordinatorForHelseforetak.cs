using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Services.Bruker;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Helseforetak
{
    public class OppdaterKoordinatorForHelseforetak
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
                    if(!KanKoordinatorOppdateres(command.Koordinator, out var feilmelding))
                        return new Status { Suksess = false, Feilmelding = feilmelding };

                    var institusjonIdListe = command.Koordinator.Institutions.Select(x => x.Id);
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

            private bool KanKoordinatorOppdateres(HealthcareInstitutionCoordinator koordinator, out string feilmelding)
            {
                feilmelding = "";

                if(string.IsNullOrWhiteSpace(koordinator.FirstName))
                {
                    feilmelding = "Fornavn må fylles ut";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(koordinator.Surname))
                {
                    feilmelding = "Etternavn må fylles ut";
                    return false;
                }

                if(string.IsNullOrWhiteSpace(koordinator.ModifiedHPRNumber) && string.IsNullOrWhiteSpace(koordinator.ModifiedPseudonym))
                {
                    feilmelding = "Hpr nummer eller identpseudonym må fylles ut";
                    return false;
                }

                if(!string.IsNullOrWhiteSpace(koordinator.ModifiedPseudonym) && !BrukerValidator.ErGyldigIdentPseudonym(koordinator.ModifiedPseudonym))
                {
                    feilmelding = "Identpseudonym er ikke gyldig";
                    return false;
                }

                return true;
            }

            private void OppdaterInstitusjonForKoordinator(HealthcareInstitutionCoordinator koordinatorForHelseForetak, IEnumerable<int> institusjonIdListe, List<Koordinator> koordinatorer)
            {
                if (koordinatorForHelseForetak.IsDisabled)
                    return;

                DeaktiverKooordinatorForInstitusjonSomIkkeErILista(koordinatorer, institusjonIdListe);

                foreach (var institusjonId in institusjonIdListe)
                {
                    var koordinator = HentKoordinator(institusjonId, koordinatorForHelseForetak.HPRNumber, koordinatorForHelseForetak.IdentityPseudonym);
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

            private static void OppdaterKoordinatorerForInstitusjonIHelseForetak(HealthcareInstitutionCoordinator koordinatorForHelseforetak, List<Koordinator> koordinatorer)
            {
                foreach (var koordinator in koordinatorer)
                {
                    koordinator.Fornavn = koordinatorForHelseforetak.FirstName;
                    koordinator.Etternavn = koordinatorForHelseforetak.Surname;
                    koordinator.Epost = koordinatorForHelseforetak.Email;
                    koordinator.HPRNummer = koordinatorForHelseforetak.ModifiedHPRNumber;
                    koordinator.IdentPseudonym = koordinatorForHelseforetak.ModifiedPseudonym;
                    koordinator.ErDeaktivert = koordinatorForHelseforetak.IsDisabled;
                }
            }

            private List<Koordinator> FinnKoordinatorForInstitusjonIHelseforetak(Command request)
            {
                return _context.Coordinator.Where(k => k.Institusjon.Helseforetak.Id == request.HelseforetakId &&
                                                    ((!string.IsNullOrEmpty(k.HPRNumber) &&
                                                    k.HPRNumber == request.Koordinator.HPRNumber) ||
                                                    (!string.IsNullOrEmpty(k.IdentityPseudonym) &&
                                                    k.IdentityPseudonym == request.Koordinator.IdentityPseudonym))).ToList();
            }

            private Koordinator LagKoordinatorForInstitusjon(HealthcareInstitutionCoordinator koordinator, int institusjonId)
            {
                var institusjon = _context.Institution.FirstOrDefault(i => i.Id == institusjonId);
                var nyKoordinator = new Koordinator
                {
                    Fornavn = koordinator.FirstName,
                    Etternavn = koordinator.Surname,
                    HPRNummer = koordinator.HPRNumber,
                    IdentPseudonym = koordinator.IdentityPseudonym,
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
                var koordinator = _context.Coordinator.FirstOrDefault(k => k.Institusjon.Id == institusjonId &&
                                                                        ((!string.IsNullOrEmpty(k.HPRNumber) &&
                                                                        k.HPRNumber == hprNummer) ||
                                                                        (!string.IsNullOrEmpty(k.IdentityPseudonym) &&
                                                                        k.IdentityPseudonym == identPseudonym)));
                return koordinator;
            }
        }
    }
}
