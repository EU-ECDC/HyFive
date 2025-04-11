using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FireIndikasjonerSesjon = HyFive.Modeller.V1.Sesjon.FireIndikasjonerSesjon;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.FireIndikasjoner.Helpers;
using Microsoft.Extensions.Logging;

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class LagreSesjon
    {
        public class Command : IRequest<Guid>
        {
            public FireIndikasjonerSesjon Sesjon { get; set; }
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly IBrukerService _brukerService;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger, IBrukerService brukerService) 
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
                _brukerService = brukerService;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                // Verifisere at observatør er observatør på institusjon
                var observator = await HentObservator(request, cancellationToken);
                if (observator == null)
                    throw new Exception(
                        $"Fant ikke en observatør med HPR-nummer {request.HPRNummer} på institusjon med ID {request.Sesjon.Avdeling.InstitusjonId}");

                var indikasjonstyper = _context.IndicationTypes.ToList();
                var aktivitettyper = _context.ActivityType.ToList();

                var sesjon = _mapper.Map<Domene.Session.FourIndicationsSession>(request.Sesjon);
                sesjon.CreatedTime = DateTime.Now;
                sesjon.Department = await HentAvdeling(request, cancellationToken);

                // Dette er måten vi ønsker å håndtere feil hvis vi prøver å lagre en sesjon med en avdeling som lenger ikke eksisterer
                if (sesjon.Department == null)
                {
                    _logger.LogWarning($"Fant ikke avdeling med id: {request.Sesjon.Avdeling.Id}");
                    return sesjon.Id;
                }

                sesjon.Observer = observator;
                foreach (var observasjon in sesjon.Observations)
                {
                    FireIndikasjonerObservasjonValidator.ValidateObservasjon(observasjon);
                    observasjon.CreatedTime = DateTime.Now;
                    observasjon.Role = sesjon.Department.Roller.FirstOrDefault(r => r.Id == observasjon.Role.Id);
                    observasjon.IndicationTypes = indikasjonstyper
                        .Where(i => observasjon.IndicationTypes.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observasjon.Activity.ActivityType = observasjon.Activity.ActivityType != null
                        ? aktivitettyper.FirstOrDefault(a => a.Id == observasjon.Activity.ActivityType.Id)
                        : null;
                }

                var overforingsstatuser = _context.TransmissionStatusType.ToList();
                sesjon.TransmissionStatus = overforingsstatuser.First(o => o.Code == OverforingstatusTypeKonstanter.OverfortTilKoordinator);

                _context.Add(sesjon);
                _context.SaveChanges();
                return sesjon.Id;
            }

            private async Task<Domene.Place.Avdeling> HentAvdeling(Command request, CancellationToken cancellationToken)
            {
                return await _context.Department.Include(a => a.Roller)
                    .FirstOrDefaultAsync(a => a.Id == request.Sesjon.Avdeling.Id, cancellationToken);
            }


            private async Task<Observator> HentObservator(Command request, CancellationToken cancellationToken)
            {
                var institusjon = await _context.Institution
                    .Include(i => i.Users)
                    .FirstOrDefaultAsync(i => i.Id == request.Sesjon.Avdeling.InstitusjonId);

                if (institusjon == null)
                    throw new Exception(
                        $"Fant ikke oppgitt institusjon med id: {request.Sesjon.Avdeling.InstitusjonId}");

                return institusjon.Users.OfType<Observator>()
                    .Where(_brukerService.HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer, request.Pseudonym).Compile())
                    .FirstOrDefault();

            }
        }
    }
}