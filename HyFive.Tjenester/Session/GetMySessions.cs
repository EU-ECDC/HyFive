using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Session
{
    public class GetMySessions
    {
        public class Query : IRequest<List<SessionReport>>
        {
            public string HPRNumber { get; set; }
            public string Pseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<SessionReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<SessionReport>> Handle(Query request, CancellationToken cancellationToken)
            {
                var sessions = await _context.Session
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institution)
                    .Where(s => s.Observer.IsDeactivated == false
                                && ((HasHprNumber(request.HPRNumber) && s.Observer.HPRNumber == request.HPRNumber) ||
                                    (HasIdentityPseudonym(request.Pseudonym) && s.Observer.IdentityPseudonym == request.Pseudonym)))
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                var mapped = _mapper.Map<List<Domain.Session.Session>, List<SessionReport>>(sessions);
                return mapped;
            }

            private static bool HasIdentityPseudonym(string identityPseudonym)
            {
                return !string.IsNullOrEmpty(identityPseudonym);
            }

            private static bool HasHprNumber(string hprNumber)
            {
                if (string.IsNullOrEmpty(hprNumber))
                    return false;
                
                return true;
            }
        }
    }
}