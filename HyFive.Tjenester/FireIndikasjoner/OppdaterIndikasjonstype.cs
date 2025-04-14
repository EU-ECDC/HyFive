using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HyFive.Models.V1.Observation;
using System;

namespace HyFive.Services.FireIndikasjoner
{
    public class OppdaterIndikasjonstype
    {
        public class Command : IRequest<IndicationType>
        {
            public IndicationType Indikasjonstype { get; set; }
        }

        public class Handler : IRequestHandler<Command, IndicationType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IndicationType> Handle(Command request, CancellationToken cancellationToken)
            {
                var indikasjonstype = await _context.IndicationTypes
                    .FirstOrDefaultAsync(i => i.Id == request.Indikasjonstype.Id, cancellationToken);

                if (indikasjonstype == null) throw new Exception($"Fant ikke indikasjontype med id {request.Indikasjonstype.Id}");

                indikasjonstype.Name = request.Indikasjonstype.Name;
                indikasjonstype.Number = request.Indikasjonstype.Number;

                _context.IndicationTypes.Update(indikasjonstype);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedIndikasjonstype =
                    _mapper.Map<Domene.Observation.IndicationTypes, IndicationType>((Domene.Observation.IndicationTypes)indikasjonstype);

                return mappedIndikasjonstype;
            }
        }
    }
}