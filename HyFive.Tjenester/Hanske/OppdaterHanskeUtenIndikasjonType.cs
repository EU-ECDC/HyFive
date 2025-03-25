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
    public class OppdaterHanskeUtenIndikasjonType
    {
        public class Command : IRequest<HanskeUtenIndikasjonType>
        {
            public HanskeUtenIndikasjonType HanskeUtenIndikasjonType { get; set; }
        }

        public class Handler : IRequestHandler<Command, HanskeUtenIndikasjonType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<HanskeUtenIndikasjonType> Handle(Command request, CancellationToken cancellationToken)
            {
                var hanskeUtenIndikasjonType = await _context.HanskeUtenIndikasjonType
                    .FirstOrDefaultAsync(x => x.Id == request.HanskeUtenIndikasjonType.Id);

                if (hanskeUtenIndikasjonType == null) throw new Exception($"Fant ikke hanskeUtenIndikasjonType med id {request.HanskeUtenIndikasjonType.Id}");

                hanskeUtenIndikasjonType.Navn = request.HanskeUtenIndikasjonType.Navn;

                _context.Update(hanskeUtenIndikasjonType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<HanskeUtenIndikasjonType>(hanskeUtenIndikasjonType);
                return mapped;
            }
        }
    }
}
