using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon.Hansker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Hanske
{
    public class OppdaterHandhygieneEtterHanskebrukType
    {
        public class Command : IRequest<HandhygieneEtterHanskebrukType>
        {
            public HandhygieneEtterHanskebrukType HandhygieneEtterHanskebrukType { get; set; }
        }

        public class Handler : IRequestHandler<Command, HandhygieneEtterHanskebrukType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<HandhygieneEtterHanskebrukType> Handle(Command request, CancellationToken cancellationToken)
            {
                var handhygieneEtterHanskebrukType = await _context.HandhygieneEtterHanskebrukType
                    .FirstOrDefaultAsync(x => x.Id == request.HandhygieneEtterHanskebrukType.Id);

                if (handhygieneEtterHanskebrukType == null) throw new Exception($"Fant ikke handhygieneEtterHanskebrukType med id {request.HandhygieneEtterHanskebrukType.Id}");

                handhygieneEtterHanskebrukType.Navn = request.HandhygieneEtterHanskebrukType.Navn;

                _context.Update(handhygieneEtterHanskebrukType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<HandhygieneEtterHanskebrukType>(handhygieneEtterHanskebrukType);
                return mapped;
            }
        }
    }
}
