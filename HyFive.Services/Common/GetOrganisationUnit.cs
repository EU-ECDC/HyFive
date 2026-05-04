using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Common
{
    public class GetOrganisationUnit
    {
        public class Query : IRequest<Models.V1.OrganisationUnit.OrganisationUnit>
        {
            public int Id { get; set; }
            public bool IncludeChildren { get; set; } = false;
        }

        public class Handler : IRequestHandler<Query, Models.V1.OrganisationUnit.OrganisationUnit>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.OrganisationUnit.OrganisationUnit> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.OrganisationUnit
                .AsNoTracking()
                .Include(o => o.Type)
                .Include(o => o.LevelRef)
                .Include(o => o.Address)
                .Include(o => o.OrganisationUnitRoles)
                    .ThenInclude(our => our.Role)
                .Include(o => o.Children)
                    .ThenInclude(c => c.Type)
                .Include(o => o.Children)
                    .ThenInclude(c => c.OrganisationUnitRoles)
                        .ThenInclude(our => our.Role)
                .Where(o => o.Id == request.Id);

                var entity = await query.FirstOrDefaultAsync(cancellationToken);

                return entity == null
                    ? null
                    : _mapper.Map<Models.V1.OrganisationUnit.OrganisationUnit>(entity);
            }
        }
    }
}
