using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.Session;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Session
{
    public class GetMySessions
    {
        public class Query : IRequest<SearchSessionsResult>
        {
            public string Email { get; set; }
            public SearchSessions Search {  get; set; }
        }

        public class Handler : IRequestHandler<Query, SearchSessionsResult>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<SearchSessionsResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Session
                   .AsNoTracking()
                   .Where(s => !s.Observer.IsDeactivated && s.Observer.Email == request.Email)
                   .Include(s => s.Observer)
                   .Include(s => s.OrganisationUnit)
                       .ThenInclude(ou => ou.Parent)
                           .ThenInclude(parent => parent.Parent);


                var count = await query.CountAsync(cancellationToken);

                var sessions = await query
                    .OrderByDescending(s => s.StartDate) 
                    .Skip(request.Search.Skip)
                    .Take(request.Search.Take)
                    .ToListAsync(cancellationToken);

                var mapped = _mapper.Map<List<Domain.Session.Session>, List<SessionReport>>(sessions);

                return new SearchSessionsResult()
                {
                    sessionReports = mapped,
                    totalCount = count
                };
            }

        }
    }
}