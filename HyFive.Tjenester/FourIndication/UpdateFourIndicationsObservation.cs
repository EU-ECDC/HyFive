using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Services.FourIndication.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FourIndicatorsObservation = HyFive.Models.V1.Observation.FourIndicatorsObservation;

namespace HyFive.Services.FourIndication
{
    public class UpdateFourIndicationsObservation
    {
        public class Command : IRequest<bool>
        {
            public FourIndicatorsObservation Observation { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var observation = await _context.FourIndicationsObservation
                    .Include(o => o.FourIndicationsSession)
                    .ThenInclude(s => s.TransferStatus)
                    .Include(o => o.IndicationTypes)
                    .Include(o => o.Activity)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observation.Id), cancellationToken);
                
                if (observation == null)
                {
                    throw new Exception("O-FI-01: Could not find observation with ID: " + request.Observation.Id);
                }
                if (observation.FourIndicationsSession.TransferStatus?.Code == TransferStatusTypeConstants.TransferredToFhi)
                {
                    throw new Exception("O-FI-02: The observation has already been transferred to FHI and cannot be changed.");
                }

                FourIndicatorsObservationValidator.ValidateObservation(_mapper.Map<Domain.Observation.FourIndicationsObservation>(request.Observation));

                try
                {
                    var indicationTypesFromRequest = _context.IndicationTypes.Where(i => request.Observation.IndicationTypes.Select(oi => oi.Id).Contains(i.Id)).ToList();
                    observation.IndicationTypes = indicationTypesFromRequest;

                    var activityTypeFromRequest = _context.ActivityType.FirstOrDefault(a => a.Code == request.Observation.Activity.ActivityType.Code);
                    observation.Activity.ActivityType = activityTypeFromRequest;
                    observation.Activity.GloveUsed = request.Observation.Activity.GloveUsed;
                    observation.Activity.TimeSpent = request.Observation.Activity.TimeSpent;
                    observation.Activity.TimeRecordingWasDone = request.Observation.Activity.TimeRecordingWasDone;

                    observation.RegisteredTime = request.Observation.RegistrationTime;

                    var rolleFraRequest = _context.Role.FirstOrDefault(r => r.Id == request.Observation.Role.Id);
                    observation.Role = rolleFraRequest;

                    observation.Comment = request.Observation.Comment;

                    _context.Update(observation);

                    _context.SaveChanges();
                }
                catch (Exception e)
                {   
                    _logger.LogError(e, "O-FI-03: Error while updating Four Indication observation.");
                    throw;
                }

                return true;
            }
        }
    }
}
