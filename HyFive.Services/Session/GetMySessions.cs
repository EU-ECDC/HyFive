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
                var sessions = await _context.Session
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Facility)
                    .Where(s => s.Observer.IsDeactivated == false &&
                            s.Observer.Email == request.Email)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                int count = sessions.Count();
                sessions = sessions.Skip(request.Search.Skip).Take(request.Search.Take).ToList(); 

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