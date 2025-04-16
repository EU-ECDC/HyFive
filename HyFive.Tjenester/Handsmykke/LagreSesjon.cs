using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Bruker;
using Microsoft.EntityFrameworkCore;
using HandJewelrySession = HyFive.Models.V1.Session.HandJewelrySession;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using Microsoft.Extensions.Logging;

namespace HyFive.Services.Handsmykke
{
    public class LagreSesjon
    {
        public class Command : IRequest<Guid>
        {
            public HandJewelrySession Sesjon { get; set; }
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
                    throw new Exception($"Fant ikke en observatør med HPR-nummer {request.HPRNummer} på institusjon med ID {request.Sesjon.Department.InstitusjonId}");

                var handsmykketyper = _context.HandJewelryType.ToList();
                var sesjon = _mapper.Map<Domene.Session.HandJewelrySession>(request.Sesjon);
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
                    observasjon.CreatedTime = DateTime.Now;
                    observasjon.Role = sesjon.Department.Roller.FirstOrDefault(r => r.Id == observasjon.Role.Id);
                    observasjon.HandJewelry = handsmykketyper.Where(ht => observasjon.HandJewelry.Select(oh => oh.Id).Contains(ht.Id)).ToList();
                    observasjon.Comment = string.IsNullOrEmpty(observasjon.Comment) ? null : observasjon.Comment;
                }

                var overforingsstatuser = _context.TransmissionStatusType.ToList();
                sesjon.TransmissionStatus = overforingsstatuser.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator);

                _context.Add(sesjon);
                _context.SaveChanges();
                return sesjon.Id;
            }

            private async Task<Domene.Place.Avdeling> HentAvdeling(Command request, CancellationToken cancellationToken)
            {
                return await _context.Department.Include(a => a.Roller).FirstOrDefaultAsync(a => a.Id == request.Sesjon.Department.Id, cancellationToken);
            }

            private async Task<Observator> HentObservator(Command request, CancellationToken cancellationToken)
            {
                var institusjon = await _context.Institution
                    .Include(i => i.Users)
                    .FirstOrDefaultAsync(i => i.Id == request.Sesjon.Department.InstitusjonId);

                if (institusjon == null)
                    throw new Exception($"Fant ikke oppgitt institusjon med id: {request.Sesjon.Department.InstitusjonId}");

                return institusjon
                    .Users
                    .OfType<Observator>()
                    .FirstOrDefault(_brukerService.HasHprOrPseudonymAndIsActive<Observator>(request.HPRNummer, request.Pseudonym).Compile());
            }
        }
    }
}
