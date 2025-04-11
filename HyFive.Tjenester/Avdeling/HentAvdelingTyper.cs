using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Avdeling
{
    public class HentAvdelingTyper
    {
        public class Query : IRequest<IEnumerable<Modeller.V1.Institution.DepartmentType>>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<HentAvdelingTyper.Query, IEnumerable<Modeller.V1.Institution.DepartmentType>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Modeller.V1.Institution.DepartmentType>> Handle(HentAvdelingTyper.Query request, CancellationToken cancellationToken)
            {
                return await _context.SectionType
                    .AsNoTracking()
                    .OrderBy(a => a.Name)
                    .ProjectTo<Modeller.V1.Institution.DepartmentType>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
