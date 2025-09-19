using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;

namespace HyFive.Services.Region
{
    public class UpdateRegion
    {
        public class Command : IRequest<Models.V1.Facility.Region>
        {
            public Models.V1.Facility.Region RegionType { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Facility.Region>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Facility.Region> Handle(Command request, CancellationToken cancellationToken)
            {
                var regionType = _context.Region.SingleOrDefault(r => r.Id == request.RegionType.Id);

                if (regionType == null) throw new Exception($"Did not find region with id: {request.RegionType.Id}");

                regionType.Name = request.RegionType.Name;

                _context.Update(regionType);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Models.V1.Facility.Region>(regionType);
            }
        }
    }
}
