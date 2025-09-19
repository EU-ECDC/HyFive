using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Region
{
    public class CreateRegion
    {
        public class Command : IRequest<Models.V1.Facility.Region>
        {
            public CreateRegionRequest NewRegion { get; set; }
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
                var exists = await _context.Region.AnyAsync(r => r.Code == request.NewRegion.Code);
                if (exists)
                    throw new InvalidOperationException(
                        $"Code {request.NewRegion.Code} is already in use. Please try with another code.");

                var region = new Domain.Place.Region
                {
                    Code = request.NewRegion.Code,
                    Name = request.NewRegion.Name
                };

                _context.Region.Add(region);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Models.V1.Facility.Region>(region);
            }
        }
    }
}
