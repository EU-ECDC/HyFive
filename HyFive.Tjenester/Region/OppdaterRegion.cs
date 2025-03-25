using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;

namespace HyFive.Tjenester.Region
{
    public class OppdaterRegion
    {
        public class Command : IRequest<Modeller.V1.Institusjon.Region>
        {
            public Modeller.V1.Institusjon.Region RegionType { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Institusjon.Region>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Modeller.V1.Institusjon.Region> Handle(Command request, CancellationToken cancellationToken)
            {
                var regionType = _context.Region.SingleOrDefault(r => r.Id == request.RegionType.Id);

                if (regionType == null) throw new Exception($"Fant ikke region med id {request.RegionType.Id}");

                regionType.Navn = request.RegionType.Navn;

                _context.Update(regionType);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Modeller.V1.Institusjon.Region>(regionType);
            }
        }
    }
}
