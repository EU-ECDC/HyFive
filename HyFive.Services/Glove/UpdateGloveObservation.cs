using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Services.Glove.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Glove
{
    public class UpdateGloveObservation
    {
        public class Command : IRequest<bool>
        {
            public GloveObservation Observation { get; set; }
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
                var observation = await _context.GloveObservation
                    .Include(o => o.GloveSession)
                    .ThenInclude(s => s.TransferStatus)
                    .Include(o => o.PostGloveHandHygieneType)
                    .Include(o => o.GloveWithIndicationTypes)
                    .Include(o => o.GloveWithoutIndicationTypes)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observation.Id), cancellationToken);

                if (observation == null)
                {
                    throw new DomainException("ObservationNotFound" + request.Observation.Id);
                }

                if (observation.GloveSession.TransferStatus?.Code == TransferStatusTypeConstants.TransferredToAdmin)
                {
                    throw new DomainException("ObservationAlreadyTransferred");
                }

                var observationFromRequest =
                    _mapper.Map<Domain.Observation.Gloves.GloveObservation>(request.Observation);
                GloveObservationValidator.ValidateObservation(observationFromRequest);

                var gloveWithIndicationTypes = await _context.GloveWithIndicationType.ToListAsync(cancellationToken);
                var gloveWithoutIndicationTypes = await _context.GloveWithoutIndicationType.ToListAsync(cancellationToken);
                var handHygieneAfterGloveUseTypes = await _context.HandHygieneAfterGloveUseType.ToListAsync(cancellationToken);
                
                
                observation.RegisteredTime = request.Observation.RegisteredTime;

                observation.GlovesUsed = observationFromRequest.GlovesUsed;
                observation.GloveWithIndicationTypes = gloveWithIndicationTypes
                    .Where(hmi => observationFromRequest.GloveWithIndicationTypes.Select(ohmi => ohmi.Id).Contains(hmi.Id))
                    .ToList();
                observation.GloveWithoutIndicationTypes = gloveWithoutIndicationTypes
                    .Where(hui => observationFromRequest.GloveWithoutIndicationTypes.Select(ohui => ohui.Id).Contains(hui.Id))
                    .ToList();
                observation.PostGloveHandHygieneType = observationFromRequest.PostGloveHandHygieneType != null
                    ? handHygieneAfterGloveUseTypes.FirstOrDefault(he => he.Id == observationFromRequest.PostGloveHandHygieneType.Id)
                    : null;
                    
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
