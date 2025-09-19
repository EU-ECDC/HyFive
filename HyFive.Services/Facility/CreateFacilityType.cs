using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class CreateFacilityType
    {
        public class Command : IRequest<FacilityType>
        {
            public CreateFacilityTypeRequest FacilityType { get; set; }
        }

        public class Handler : IRequestHandler<Command, FacilityType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<FacilityType> Handle(Command request, CancellationToken cancellationToken)
            {
                var exists = await _context.FacilityType.AnyAsync(r => r.Code == request.FacilityType.Code);
                if (exists)
                    throw new InvalidOperationException(
                        $"\"Code {{request.FacilityType.Code}} is already in use. Please try with a different code.");

                var facilityType = new Domain.Place.FacilityType
                {
                    Code = request.FacilityType.Code,
                    Name = request.FacilityType.Name
                };

                _context.FacilityType.Add(facilityType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<FacilityType>(facilityType);
                return mapped;
            }
        }
    }
}