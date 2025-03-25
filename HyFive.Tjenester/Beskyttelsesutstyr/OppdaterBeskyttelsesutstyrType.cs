using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class OppdaterBeskyttelsesutstyrType
    {
        public class Command : IRequest<BeskyttelsesutstyrType>
        {
            public BeskyttelsesutstyrType UtstyrType { get; set; }
        }

        public class Handler : IRequestHandler<Command, BeskyttelsesutstyrType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<BeskyttelsesutstyrType> Handle(Command request, CancellationToken cancellationToken)
            {
                var beskyttelsesutstyrType = await _context.BeskyttelsesutstyrType
                    .FirstOrDefaultAsync(x => x.Id == request.UtstyrType.Id, cancellationToken);

                if (beskyttelsesutstyrType == null) throw new Exception($"Fant ikke beskyttelsesutstyrType med id {request.UtstyrType.Id}");

                beskyttelsesutstyrType.Navn = request.UtstyrType.Navn;

                _context.BeskyttelsesutstyrType.Update(beskyttelsesutstyrType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<BeskyttelsesutstyrType>(beskyttelsesutstyrType);
                return mapped;
            }
        }
    }
}
