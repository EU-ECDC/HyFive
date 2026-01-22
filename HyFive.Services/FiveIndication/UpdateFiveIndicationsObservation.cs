using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Services.FiveIndication.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FiveIndicatorsObservation = HyFive.Models.V1.Observation.FiveIndicatorsObservation;

namespace HyFive.Services.FiveIndication
{
    public class UpdateFiveIndicationsObservation
    {
        public class Command : IRequest<bool>
        {
            public FiveIndicatorsObservation Observation { get; set; }
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
                var observation = await _context.FiveIndicationsObservation
                    .Include(o => o.FiveIndicationsSession)
                    .ThenInclude(s => s.TransferStatus)
                    .Include(o => o.IndicationTypes)
                    .Include(o => o.Activity)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observation.Id), cancellationToken);
                
                if (observation == null)
                {
                    throw new DomainException("ObservationNotFound", request.Observation.Id);
                }
                if (observation.FiveIndicationsSession.TransferStatus?.Code == TransferStatusTypeConstants.TransferredToAdmin)
                {
                    throw new DomainException("ObservationAlreadyTransferred");
                }

                FiveIndicatorsObservationValidator.ValidateObservation(_mapper.Map<Domain.Observation.FiveIndicationsObservation>(request.Observation));

                
                    var indicationTypesFromRequest = await _context.IndicationTypes.Where(i => request.Observation.IndicationTypes.Select(oi => oi.Id).Contains(i.Id)).ToListAsync(cancellationToken);
                    observation.IndicationTypes = indicationTypesFromRequest;

                    var activityTypeFromRequest = await _context.ActivityType.FirstOrDefaultAsync(a => a.Code == request.Observation.Activity.ActivityType.Code, cancellationToken);
                    observation.Activity.ActivityType = activityTypeFromRequest;
                    observation.Activity.GlovesUsed = request.Observation.Activity.GlovesUsed;
                    observation.Activity.SecondsUsed = request.Observation.Activity.SecondsUsed;
                    observation.Activity.TimingWasPerformed = request.Observation.Activity.TimingWasPerformed;

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
