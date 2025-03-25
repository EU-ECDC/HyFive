using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HyFive.Modeller.V1.Observasjon;
using System;

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class OppdaterIndikasjonstype
    {
        public class Command : IRequest<IndikasjonType>
        {
            public IndikasjonType Indikasjonstype { get; set; }
        }

        public class Handler : IRequestHandler<Command, IndikasjonType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IndikasjonType> Handle(Command request, CancellationToken cancellationToken)
            {
                var indikasjonstype = await _context.Indikasjon
                    .FirstOrDefaultAsync(i => i.Id == request.Indikasjonstype.Id, cancellationToken);

                if (indikasjonstype == null) throw new Exception($"Fant ikke indikasjontype med id {request.Indikasjonstype.Id}");

                indikasjonstype.Navn = request.Indikasjonstype.Navn;
                indikasjonstype.Nummer = request.Indikasjonstype.Nummer;

                _context.Indikasjon.Update(indikasjonstype);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedIndikasjonstype =
                    _mapper.Map<Domene.Observasjon.IndikasjonType, IndikasjonType>(indikasjonstype);

                return mappedIndikasjonstype;
            }
        }
    }
}