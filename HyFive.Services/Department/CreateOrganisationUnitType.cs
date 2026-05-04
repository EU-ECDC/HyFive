using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Department
{
    public class CreateOrganisationUnitType
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
                if (string.IsNullOrWhiteSpace(request.Type?.Code))
                    throw new ValidationException("CodeRequired");

                var code = request.Type.Code.Trim();

                var exists = await _context.OrganisationUnitType
                    .AnyAsync(r => r.Code == code, cancellationToken);

                if (exists)
                    throw new ValidationException("CodeExists", code);

                var typeEntity = new Domain.Place.OrganisationUnitType
                {
                    Code = code,
                    Name = request.Type.Name?.Trim(),
                    Description = request.Type.Description?.Trim()
                };

                _context.OrganisationUnitType.Add(typeEntity);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<OrganisationUnitType>(typeEntity);
            }
        }
    }
}