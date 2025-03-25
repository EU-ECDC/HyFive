using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Region
{
    public class OpprettRegion
    {
        public class Command : IRequest<Modeller.V1.Institusjon.Region>
        {
            public OpprettRegionRequest NyRegion { get; set; }
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
                var exists = await _context.Region.AnyAsync(r => r.Kode == request.NyRegion.Kode);
                if (exists)
                    throw new InvalidOperationException(
                        $"Kode {request.NyRegion.Kode} er allerede i bruk. Vennligst prøv med en annen kode.");

                var region = new Domene.Sted.Region
                {
                    Kode = request.NyRegion.Kode,
                    Navn = request.NyRegion.Navn
                };

                _context.Region.Add(region);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Modeller.V1.Institusjon.Region>(region);
            }
        }
    }
}
