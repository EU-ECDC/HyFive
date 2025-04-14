using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Institusjon
{
    public class HentInstitusjonerForKoordinator
    {
        public class Query : IRequest<Models.V1.Institution.InstitutionReport[]>
        {
            public string KoordinatorHprNummer { get; set; }
            public string KoordinatorPseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Institution.InstitutionReport[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.Institution.InstitutionReport[]> Handle(Query request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrEmpty(request.KoordinatorHprNummer) && string.IsNullOrEmpty(request.KoordinatorPseudonym))
                {
                    throw new ArgumentException(
                        $"Koordinators HPR-nummer må være større enn 0, eller KoordinatorPseudonym må være utfylt. HPR-nummer var: {request.KoordinatorHprNummer}. ");
                }
                
                var query = _context.Institution
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Roller)
                    .Include(i => i.Users)
                    .Include(i => i.PredefinerteKommmentarer)
                    .Include(i => i.Institusjontype)
                    .Where(i => i.Brukere
                        .Where(b => b.ErDeaktivert == false
                                    && ((HarHprNummer(request.KoordinatorHprNummer) && b.HPRNummer == request.KoordinatorHprNummer) ||
                                        (HarIdentPseudonym(request.KoordinatorPseudonym) && b.IdentPseudonym == request.KoordinatorPseudonym)))
                        .Any(b => b.Discriminator == nameof(Koordinator))
                    );

                var result = await query
                    .ProjectTo<Models.V1.Institution.InstitutionReport>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
                return result;
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
