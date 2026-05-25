using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class UpdateFacility
    {
        public class Command : IRequest<OrganisationUnit>
        {
            public UpdateOrganizationUnitRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, OrganisationUnit>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<OrganisationUnit> Handle(Command command, CancellationToken cancellationToken)
            {
                var req = command.Request;

                // 1) Validate requested type
                var type = await _context.OrganisationUnitType
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == req.OrganisationUnitTypeId, cancellationToken);

                if (type == null)
                    throw new DomainException("FacilityTypeNotFound", req.OrganisationUnitTypeId);

                var orgUnit = await _context.OrganisationUnit
                .Include(x => x.Address)
                .Include(x => x.LevelRef)
                .FirstOrDefaultAsync(x => x.Id == req.Id, cancellationToken);

                if (orgUnit == null)
                    throw new DomainException("FacilityNotFound", req.Id);

                var nameExists = await _context.OrganisationUnit
                    .AnyAsync(x => x.ParentId == null && x.Id != orgUnit.Id && x.Name == req.Name, cancellationToken);

                if (nameExists)
                    throw new ValidationException("FacilityNameExists");

                orgUnit.Name = req.Name?.Trim() ?? orgUnit.Name;
                orgUnit.Abbreviation = req.Abbreviation?.Trim() ?? orgUnit.Abbreviation;
                orgUnit.Description = req.Description?.Trim() ?? orgUnit.Description;
                orgUnit.TypeId = type.Id;
                orgUnit.Type = type;

                if (req.Address != null)
                {
                    if (orgUnit.Address == null)
                    {
                        orgUnit.Address = new Domain.Place.Address();
                    }

                    orgUnit.Address.CityId = req.Address.CityId > 0 ? req.Address.CityId : orgUnit.Address.CityId;
                    orgUnit.Address.Street = req.Address.Street?.Trim() ?? orgUnit.Address.Street;
                    orgUnit.Address.PostalCode = req.Address.PostalCode?.Trim() ?? orgUnit.Address.PostalCode;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<OrganisationUnit>(orgUnit);
            }
        }
    }
}
