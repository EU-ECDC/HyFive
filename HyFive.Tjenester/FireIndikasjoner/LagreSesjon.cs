using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FourIndicationsSession = HyFive.Models.V1.Session.FourIndicationsSession;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.FireIndikasjoner.Helpers;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.FireIndikasjoner
{
    public class LagreSesjon
    {
        public class Command : IRequest<Guid>
        {
            public FourIndicationsSession Sesjon { get; set; }
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly IUserService _brukerService;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger, IUserService brukerService) 
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
                        $"Fant ikke en observatør med HPR-nummer {request.HPRNummer} på institusjon med ID {request.Sesjon.Department.InstitusjonId}");

                var indikasjonstyper = _context.IndicationTypes.ToList();
                var aktivitettyper = _context.ActivityType.ToList();

                var sesjon = _mapper.Map<Domene.Session.FourIndicationsSession>(request.Sesjon);
                sesjon.CreatedTime = DateTime.Now;
                sesjon.Department = await HentAvdeling(request, cancellationToken);

                // Dette er måten vi ønsker å håndtere feil hvis vi prøver å lagre en sesjon med en avdeling som lenger ikke eksisterer
                if (sesjon.Department == null)
                {
                    _logger.LogWarning($"Fant ikke avdeling med id: {request.Sesjon.Department.Id}");
                    return sesjon.Id;
                }

                sesjon.Observer = observator;
                foreach (var observasjon in sesjon.Observations)
                {
                    FourIndicatorsObservationValidator.ValidateObservasjon(observasjon);
                    observasjon.CreatedTime = DateTime.Now;
                    observasjon.Role = sesjon.Department.Roller.FirstOrDefault(r => r.Id == observasjon.Role.Id);
                    observasjon.IndicationTypes = indikasjonstyper
                        .Where(i => observasjon.IndicationTypes.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observasjon.Activity.ActivityType = observasjon.Activity.ActivityType != null
                        ? aktivitettyper.FirstOrDefault(a => a.Id == observasjon.Activity.ActivityType.Id)
                        : null;
                }

                var overforingsstatuser = _context.TransmissionStatusType.ToList();
                sesjon.TransmissionStatus = overforingsstatuser.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator);

                _context.Add(sesjon);
                _context.SaveChanges();
                return sesjon.Id;
            }

            private async Task<Domene.Place.Avdeling> HentAvdeling(Command request, CancellationToken cancellationToken)
            {
                return await _context.Department.Include(a => a.Roller)
                    .FirstOrDefaultAsync(a => a.Id == request.Sesjon.Department.Id, cancellationToken);
            }


            private async Task<Observator> HentObservator(Command request, CancellationToken cancellationToken)
            {
                var institusjon = await _context.Institution
                    .Include(i => i.Users)
                    .FirstOrDefaultAsync(i => i.Id == request.Sesjon.Department.InstitusjonId);

                if (institusjon == null)
                    throw new Exception(
                        $"Fant ikke oppgitt institusjon med id: {request.Sesjon.Department.InstitusjonId}");

                return institusjon.Users.OfType<Observator>()
                    .Where(_brukerService.HasHprOrPseudonymAndIsActive<Observator>(request.HPRNummer, request.Pseudonym).Compile())
                    .FirstOrDefault();

            }
        }
    }
}