using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.Gloves;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Hanske
{
    public class OppdaterHanskeMedIndikasjonType
    {
        public class Command : IRequest<IndicatedGloveType>
        {
            public IndicatedGloveType HanskeMedIndikasjonType { get; set; }
        }

        public class Handler : IRequestHandler<Command, IndicatedGloveType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IndicatedGloveType> Handle(Command request, CancellationToken cancellationToken)
            {
                var hanskeMedIndikasjonType = await _context.IndicatedGloveType
                    .FirstOrDefaultAsync(x => x.Id == request.HanskeMedIndikasjonType.Id);

                if (hanskeMedIndikasjonType == null) throw new Exception($"Fant ikke hanskeMedIndikasjonType med id {request.HanskeMedIndikasjonType.Id}");

                hanskeMedIndikasjonType.Name = request.HanskeMedIndikasjonType.Name;

                _context.Update(hanskeMedIndikasjonType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<IndicatedGloveType>(hanskeMedIndikasjonType);
                return mapped;
            }
        }
    }
}
