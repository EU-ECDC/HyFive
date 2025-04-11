using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon.Gloves;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Hanske
{
    public class OppdaterHanskeUtenIndikasjonType
    {
        public class Command : IRequest<GeneralPurposeGloveType>
        {
            public GeneralPurposeGloveType HanskeUtenIndikasjonType { get; set; }
        }

        public class Handler : IRequestHandler<Command, GeneralPurposeGloveType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<GeneralPurposeGloveType> Handle(Command request, CancellationToken cancellationToken)
            {
                var hanskeUtenIndikasjonType = await _context.GeneralPurposeGloveType
                    .FirstOrDefaultAsync(x => x.Id == request.HanskeUtenIndikasjonType.Id);

                if (hanskeUtenIndikasjonType == null) throw new Exception($"Fant ikke hanskeUtenIndikasjonType med id {request.HanskeUtenIndikasjonType.Id}");

                hanskeUtenIndikasjonType.Name = request.HanskeUtenIndikasjonType.Name;

                _context.Update(hanskeUtenIndikasjonType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<GeneralPurposeGloveType>(hanskeUtenIndikasjonType);
                return mapped;
            }
        }
    }
}
