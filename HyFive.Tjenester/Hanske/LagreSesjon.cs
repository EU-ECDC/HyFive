using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.Glove.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GloveSession = HyFive.Models.V1.Session.GloveSession;

namespace HyFive.Services.Glove
{
    public class LagreSesjon
    {
        public class Command : IRequest<Guid>
        {
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
            public GloveSession Sesjon { get; set; }
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
                var observator = await HentObservator(request, cancellationToken);
                if (observator == null)
                    throw new Exception(
                        $"Fant ikke en observatør med HPR-nummer { request.HPRNummer } // pseudonym {request.Pseudonym} på institusjon med ID {request.Sesjon.Department.InstitusjonId}");

                var hanskeMedIndikasjonTyper = _context.IndicatedGloveType.ToList();
                var hanskeUtenIndikasjonTyper = _context.GeneralPurposeGloveType.ToList();
                var handhygieneEtterHanskebrukTyper = _context.PostGloveHandHygiene.ToList();

                var sesjon = _mapper.Map<Domene.Session.GloveSession>(request.Sesjon);
                sesjon.CreatedTime = DateTime.Now;
                sesjon.Department = await HentAvdeling(request, cancellationToken);
                sesjon.Observer = observator;

                // Dette er måten vi ønsker å håndtere feil hvis vi prøver å lagre en sesjon med en avdeling som lenger ikke eksisterer
                if(sesjon.Department == null)
                {
                    _logger.LogWarning($"Fant ikke avdeling med id: {request.Sesjon.Department.Id}");
                    return sesjon.Id;
                }

                foreach (var observajon in sesjon.Observations)
                {
                    observajon.CreatedTime = DateTime.Now;
                    observajon.Role = sesjon.Department.Roller.FirstOrDefault(r => r.Id == observajon.Role.Id);
                    observajon.IndicatedGloveTypes = hanskeMedIndikasjonTyper
                                                            .Where(hmi => observajon.IndicatedGloveTypes.Select(ohmi => ohmi.Id).Contains(hmi.Id))
                                                            .ToList();
                    observajon.GeneralPurposeGloveTypes = hanskeUtenIndikasjonTyper
                                                            .Where(hui => observajon.GeneralPurposeGloveTypes.Select(ohui => ohui.Id).Contains(hui.Id))
                                                            .ToList();
                    observajon.HandhygieneEtterHanskebrukType = observajon.HandhygieneEtterHanskebrukType != null
                                                                ? handhygieneEtterHanskebrukTyper.FirstOrDefault(he => he.Id == observajon.HandhygieneEtterHanskebrukType.Id)
                                                                : null;
                    HanskeObservasjonValidator.ValidateObservasjon(observajon);
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

                return institusjon.Users.OfType<Observator>().Where(
                    _brukerService
                        .HasHprOrPseudonymAndIsActive<Observator>(request.HPRNummer,
                            request.Pseudonym).Compile()).FirstOrDefault();
            }
        }
    }
}