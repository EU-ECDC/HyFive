using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Sesjon
{
    public class HentMineSesjoner
    {
        public class Query : IRequest<List<SessionReport>>
        {
            public string HPRNummer { get; set; }
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
                    .Where(s => s.Observer.ErDeaktivert == false
                                && ((HarHprNummer(request.HPRNummer) && s.Observer.HPRNummer == request.HPRNummer) ||
                                    (HarIdentPseudonym(request.Pseudonym) && s.Observer.IdentityPseudonym == request.Pseudonym)))
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                var mapped = _mapper.Map<List<Domene.Session.Session>, List<SessionReport>>(sessions);
                return mapped;
            }

            private static bool HarIdentPseudonym(string identPseudonym)
            {
                return !string.IsNullOrEmpty(identPseudonym);
            }

            private static bool HarHprNummer(string hprnummer)
            {
                if (string.IsNullOrEmpty(hprnummer))
                    return false;
                
                return true;
            }
        }
    }
}