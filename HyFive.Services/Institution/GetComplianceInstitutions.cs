using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Services.Institution
{
    public class GetComplianceInstitution
    {
        public class Query : IRequest<List<Models.V1.Institution.Institution>>
        {
            public List<int> InstitutionIds { get; set; } = new();
        }

        public class Handler : IRequestHandler<Query, List<Models.V1.Institution.Institution>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }



            public async Task<List<Models.V1.Institution.Institution>> Handle(Query request, CancellationToken cancellationToken)
            {
                var institutions = await _context.Institution
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Roles)
                    .Include(i => i.PredefinedComment)
                    .Include(i => i.InstitutionType)
                    .Where(i => request.InstitutionIds.Contains(i.Id))
                    .ProjectTo<Models.V1.Institution.Institution>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                //if (institution == null)
                //{
                //    throw new Exception($"Could not find institution with ID: {request.InstitutionId}");
                //}

                foreach (var institution in institutions)
                {
                    institution.HasObservations =
                        institution.Departments != null &&
                        institution.Departments.Any() &&
                        _context.Session.Include(s => s.Department)
                            .Any(s => institution.Departments.Select(d => d.Id).Contains(s.Department.Id));

                    institution.Departments = institution.Departments.OrderBy(d => d.Name).ToList();
                }

                return institutions;
            }
        }
    }
}
