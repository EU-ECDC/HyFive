using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Region
{
    public class OpprettRegion
    {
        public class Command : IRequest<Modeller.V1.Institution.Region>
        {
            public CreateRegionRequest NyRegion { get; set; }
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
                var exists = await _context.Region.AnyAsync(r => r.Code == request.NyRegion.Code);
                if (exists)
                    throw new InvalidOperationException(
                        $"Kode {request.NyRegion.Code} er allerede i bruk. Vennligst prøv med en annen kode.");

                var region = new Domene.Place.Region
                {
                    Code = request.NyRegion.Code,
                    Name = request.NyRegion.Name
                };

                _context.Region.Add(region);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Modeller.V1.Institution.Region>(region);
            }
        }
    }
}
