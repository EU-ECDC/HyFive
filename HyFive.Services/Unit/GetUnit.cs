using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Unit
{
    public class GetUnit
    {
        public class Query : IRequest<UnitResponse>
        {
            public int Id { get; set; }
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, UnitResponse>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<UnitResponse> Handle(Query request, CancellationToken cancellationToken)
            {
                // Load unit OU (+ level, roles if you want them in response)
                var unit = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(x => x.LevelRef)
                    .Include(x => x.OrganisationUnitRoles).ThenInclude(r => r.Role) // optional
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (unit == null)
                    return null;

                if (unit.LevelRef?.Level != OrganisationUnitLevels.Unit)
                    throw new DomainException("OrganisationUnitIsNotUnit", request.Id);

                // Verify the facilityId is an ancestor (walk up ParentId)
                var currentParentId = unit.ParentId;
                var safety = 0;
                while (currentParentId.HasValue && safety++ < 50)
                {
                    if (currentParentId.Value == request.FacilityId)
                    {
                        return new UnitResponse
                        {
                            Id = unit.Id,
                            Name = unit.Name,
                            FacilityId = request.FacilityId,
                            Departments = []
                        };
                    }

                    currentParentId = await _context.OrganisationUnit
                        .AsNoTracking()
                        .Where(x => x.Id == currentParentId.Value)
                        .Select(x => x.ParentId)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                // Not under this facility
                return null;
            }
        }
    }
}
