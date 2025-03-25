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
    public class OppdaterHanskeMedIndikasjonType
    {
        public class Command : IRequest<HanskeMedIndikasjonType>
        {
            public HanskeMedIndikasjonType HanskeMedIndikasjonType { get; set; }
        }

        public class Handler : IRequestHandler<Command, HanskeMedIndikasjonType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<HanskeMedIndikasjonType> Handle(Command request, CancellationToken cancellationToken)
            {
                var hanskeMedIndikasjonType = await _context.HanskeMedIndikasjonType
                    .FirstOrDefaultAsync(x => x.Id == request.HanskeMedIndikasjonType.Id);

                if (hanskeMedIndikasjonType == null) throw new Exception($"Fant ikke hanskeMedIndikasjonType med id {request.HanskeMedIndikasjonType.Id}");

                hanskeMedIndikasjonType.Navn = request.HanskeMedIndikasjonType.Navn;

                _context.Update(hanskeMedIndikasjonType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<HanskeMedIndikasjonType>(hanskeMedIndikasjonType);
                return mapped;
            }
        }
    }
}
