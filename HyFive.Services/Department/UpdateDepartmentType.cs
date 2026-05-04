using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class UpdateDepartmentType
    {
        public class Command : IRequest<OrganisationUnitType>
        {
            public OrganisationUnitType Type { get; set; }
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
                var organisationUnitTypeEntity = await _context.OrganisationUnitType
                .FirstOrDefaultAsync(x => x.Id == request.Type.Id, cancellationToken);

                if (organisationUnitTypeEntity == null)
                    throw new DomainException("OrganisationUnitTypeNotFound");

                organisationUnitTypeEntity.Code = request.Type.Code;
                organisationUnitTypeEntity.Name = request.Type.Name?.Trim();
                organisationUnitTypeEntity.Description = request.Type.Description?.Trim();

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<OrganisationUnitType>(organisationUnitTypeEntity);
            }
        }
    }
}