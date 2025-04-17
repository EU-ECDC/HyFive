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
    public class GetInstitution
    {
        public class Query : IRequest<Models.V1.Institution.Institution>
        {
            public int InstitutionId = 0;
        }

        public class Handler : IRequestHandler<Query, Models.V1.Institution.Institution>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }



            public async Task<Models.V1.Institution.Institution> Handle(Query request, CancellationToken cancellationToken)
            {
                var institution = await _context.Institution
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Role)
                    .Include(i => i.PredefinedComments)
                    .Include(i => i.InstitutionType)
                    .ProjectTo<Models.V1.Institution.Institution>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(i => i.Id == request.InstitutionId, cancellationToken);
                
                if (institution == null)
                {
                    throw new Exception($"Could not find institution with ID: {request.InstitutionId}");
                }
                
                institution.HasObservations = 
                    institution.Departments != null 
                    && institution.Departments.Any() 
                    && _context.Session.Include(s => s.Department)
                        .Any(s => institution.Departments.Select(a => a.Id).Contains(s.Department.Id));

                institution.Departments = institution.Departments.OrderBy(a => a.Name).ToList();

                return institution;
            }
        }
    }
}
