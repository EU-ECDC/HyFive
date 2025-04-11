using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Handsmykke
{
    public class HentHandsmykkeType
    {
        public class Query : IRequest<Modeller.V1.Observasjon.HandJewelryType>
        {
            public int Id = 0;
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Observasjon.HandJewelryType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Observasjon.HandJewelryType> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.HandJewelryType
                    .AsNoTracking()
                    .ProjectTo<Modeller.V1.Observasjon.HandJewelryType>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            }
        }
    }
}
