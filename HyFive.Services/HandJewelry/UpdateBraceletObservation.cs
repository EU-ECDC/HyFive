using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using HyFive.Services.HandJewelry.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HandJewelryObservation = HyFive.Models.V1.Observation.HandJewelryObservation;

namespace HyFive.Services.HandJewelry
{
    public class UpdateBraceletObservation
    {
        public class Command : IRequest<bool>
        {
            public HandJewelryObservation Observation { get; set; }
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
                var observation = await _context.HandJewelryObservation
                    .Include(o => o.HandJewelrySession)
                    .ThenInclude(s => s.TransferStatus)
                    .Include(o => o.HandJewelries)
                    .Include(o => o.Role)
                    .FirstOrDefaultAsync(o => o.Id == new Guid(request.Observation.Id), cancellationToken);

                if (observation == null)
                {
                    throw new Exception("O-HS-01: Did not find observation with ID " + request.Observation.Id);
                }
                
                if (observation.HandJewelrySession.TransferStatus?.Code == TransferStatusTypeConstants.TransferredToFhi)
                {
                    throw new Exception("O-HS-02: The observation has already been transferred to FHI and cannot be changed");
                }

                HandJewelryObservationValidator.ValidateObservation(_mapper.Map<Domain.Observation.HandJewelryObservation>(request.Observation));

                try
                {
                    var handJewelryTypeIds = request.Observation.HandJewelries.Select(h => h.Id);
                    var handJewelryFromDatabase = _context.HandJewelryType.Where(h => handJewelryTypeIds.Contains(h.Id)).ToList();

                    var handJewelryIdsFromRequest = string.Join(',', handJewelryTypeIds);
                    if (handJewelryFromDatabase.Any() == false)
                    {
                        throw new Exception($"O-HS-03:Did not find any hand jewelry with IDs {handJewelryIdsFromRequest}");
                    }

                    if (handJewelryFromDatabase.Count() != request.Observation.HandJewelries.Count())
                    {
                        throw new Exception($"O-HS-04: The number of bracelets in the observation does not match the number of bracelet types found in the database. " +
                                            $"Hand jewelry in the database: {string.Join(',',handJewelryFromDatabase.Select(h => h.Id))} / " + 
                                            $"Hand jewelry in request: {handJewelryIdsFromRequest} ");
                    }
                    
                    
                    observation.HandJewelries = handJewelryFromDatabase;
                    
                    observation.RegisteredTime = request.Observation.RegisteredTime;

                    var roleFromRequest = _context.Role.FirstOrDefault(r => r.Id == request.Observation.Role.Id);
                    observation.Role = roleFromRequest;
                    observation.Comment = request.Observation.Comment;

                    _context.Update(observation);

                    _context.SaveChanges();
                }
                catch (Exception e)
                {   
                    _logger.LogError(e, "O-HS-05: Error while updating Bracelet observation");
                    throw;
                }

                return true;
            }
        }
    }
}
