using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Services.Glove.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            private readonly ILogger<Handler> _logger;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var observation = await _context.GloveObservation
                    .Include(o => o.GloveSession)
                    .ThenInclude(s => s.TransferStatus)
                    .Include(o => o.PostGloveHandHygieneType)
                    .Include(o => o.IndicatedGloveTypes)
                    .Include(o => o.GloveWithoutIndicationTypes)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observation.Id), cancellationToken);

                if (observation == null)
                {
                    throw new Exception("O-H-01: Did not find observation with ID: " + request.Observation.Id);
                }

                if (observation.GloveSession.TransferStatus?.Code == TransferStatusTypeConstants.TransferredToFhi)
                {
                    throw new Exception("O-H-02: The observation has already been transferred to FHI and cannot be modified.");
                }

                var observationFromRequest =
                    _mapper.Map<Domain.Observation.Gloves.GloveObservation>(request.Observation);
                GloveObservationValidator.ValidateObservation(observationFromRequest);

                var gloveWithIndicationTypes = _context.GloveWithIndicationType.ToList();
                var gloveWithoutIndicationTypes = _context.GloveWithoutIndicationType.ToList();
                var handHygieneAfterGloveUseTypes = _context.HandHygieneAfterGloveUseType.ToList();
                
                try
                {
                    observation.RegisteredTime = request.Observation.RegisteredTime;

                    observation.GloveUsed = observationFromRequest.GloveUsed;
                    observation.IndicatedGloveTypes = gloveWithIndicationTypes
                        .Where(hmi => observationFromRequest.IndicatedGloveTypes.Select(ohmi => ohmi.Id).Contains(hmi.Id))
                        .ToList();
                    observation.GloveWithoutIndicationTypes = gloveWithoutIndicationTypes
                        .Where(hui => observationFromRequest.GloveWithoutIndicationTypes.Select(ohui => ohui.Id).Contains(hui.Id))
                        .ToList();
                    observation.PostGloveHandHygieneType = observationFromRequest.PostGloveHandHygieneType != null
                        ? handHygieneAfterGloveUseTypes.FirstOrDefault(he => he.Id == observationFromRequest.PostGloveHandHygieneType.Id)
                        : null;
                    
                    var roleFromRequest = _context.Role.FirstOrDefault(r => r.Id == request.Observation.Role.Id);
                    observation.Role = roleFromRequest;
                    observation.Comment = request.Observation.Comment;

                    _context.Update(observation);

                    _context.SaveChanges();
                }
                catch (Exception e)
                {   
                    _logger.LogError(e, "O-H-03: Error while updating Glove observation.");
                    throw;
                }

                return true;
            }
        }
    }
}
