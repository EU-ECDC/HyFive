using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Handsmykke
{
    public class HentHandsmykkeType
    {
        public class Query : IRequest<Modeller.V1.Observasjon.HandsmykkeType>
        {
            public int Id = 0;
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Observasjon.HandsmykkeType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Observasjon.HandsmykkeType> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.HandsmykkeType
                    .AsNoTracking()
                    .ProjectTo<Modeller.V1.Observasjon.HandsmykkeType>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            }
        }
    }
}
