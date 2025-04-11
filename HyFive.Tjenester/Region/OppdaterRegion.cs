using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Institution;
using MediatR;

namespace HyFive.Tjenester.Region
{
    public class OppdaterRegion
    {
        public class Command : IRequest<Modeller.V1.Institution.Region>
        {
            public Modeller.V1.Institution.Region RegionType { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Institution.Region>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institution.Region> Handle(Command request, CancellationToken cancellationToken)
            {
                var regionType = _context.Region.SingleOrDefault(r => r.Id == request.RegionType.Id);

                if (regionType == null) throw new Exception($"Fant ikke region med id {request.RegionType.Id}");

                regionType.Name = request.RegionType.Name;

                _context.Update(regionType);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Modeller.V1.Institution.Region>(regionType);
            }
        }
    }
}
