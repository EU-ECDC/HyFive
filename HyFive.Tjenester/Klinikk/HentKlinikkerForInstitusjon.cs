using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Klinikk
{
    public class HentKlinikkerForInstitusjon
    {
        public class Query : IRequest<IEnumerable<Modeller.V1.Institution.Clinic>>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<Modeller.V1.Institution.Clinic>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Modeller.V1.Institution.Clinic>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Clinic
                    .AsNoTracking()
                    .Include(k => k.Institution)
                    .Include(k => k.Departments)
                    .Where(k => k.Institusjon.Id == request.InstitusjonId)
                    .OrderBy(k => k.Navn)
                    .ProjectTo<Modeller.V1.Institution.Clinic>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
