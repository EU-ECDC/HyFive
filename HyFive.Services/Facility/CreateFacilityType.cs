using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class CreateFacilityType
    {
        public class Command : IRequest<OrganisationUnitType>
        {
            public CreateOrganisationUnitTypeRequest FacilityType { get; set; }
        }

        public class Handler : IRequestHandler<Command, OrganisationUnitType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<OrganisationUnitType> Handle(Command request, CancellationToken cancellationToken)
            {
                var exists = await _context.OrganisationUnitType.AnyAsync(r => r.Code == request.FacilityType.Code);
                if (exists)
                    throw new ValidationException("CodeExists", request.FacilityType.Code);

                var facilityType = new Domain.Place.OrganisationUnitType
                {
                    Code = request.FacilityType.Code,
                    Name = request.FacilityType.Name
                };

                _context.OrganisationUnitType.Add(facilityType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<OrganisationUnitType>(facilityType);
                return mapped;
            }
        }
    }
}