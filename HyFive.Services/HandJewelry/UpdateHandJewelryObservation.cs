using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Services.HandJewelry.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HandJewelryObservation = HyFive.Models.V1.Observation.HandJewelryObservation;

namespace HyFive.Services.HandJewelry
{
    public class UpdateHandJewelryObservation
    {
        public class Command : IRequest<bool>
        {
            public HandJewelryObservation Observation { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var observation = await _context.HandJewelryObservation
                    .Include(o => o.HandJewelrySession)
                    .ThenInclude(s => s.TransferStatus)
                    .Include(o => o.HandJewelries)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observation.Id), cancellationToken);

                if (observation == null)
                {
                    throw new DomainException("ObservationNotFound", request.Observation.Id);
                }
                
                if (observation.HandJewelrySession.TransferStatus?.Code == TransferStatusTypeConstants.TransferredToAdmin)
                {
                    throw new DomainException("ObservationAlreadyTransferred");
                }

                HandJewelryObservationValidator.ValidateObservation(_mapper.Map<Domain.Observation.HandJewelryObservation>(request.Observation));

                
                var handJewelryTypeIds = request.Observation.HandJewelries.Select(h => h.Id);
                var handJewelryFromDatabase = await _context.HandJewelryType.Where(h => handJewelryTypeIds.Contains(h.Id)).ToListAsync(cancellationToken);

                var handJewelryIdsFromRequest = string.Join(',', handJewelryTypeIds);
                if (!handJewelryFromDatabase.Any())
                {
                    throw new DomainException("HandJewelryNotFoundByIds", handJewelryIdsFromRequest);
                }

                if (handJewelryFromDatabase.Count != request.Observation.HandJewelries.Count)
                {
                    throw new DomainException("HandJewelryCountMismatch", string.Join(',', handJewelryFromDatabase.Select(h => h.Id)), handJewelryIdsFromRequest);
                }
                    
                    
                observation.HandJewelries = handJewelryFromDatabase;
                    
                observation.RegisteredTime = request.Observation.RegisteredTime;

                    var roleFromRequest = await _context.Role.FirstOrDefaultAsync(r => r.Id == request.Observation.Role.Id, cancellationToken);
                    observation.Role = roleFromRequest;
                    observation.Comment = request.Observation.Comment;

                _context.Update(observation);

                await _context.SaveChangesAsync(cancellationToken);
                

                return true;
            }
        }
    }
}
