using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class OppdaterAktivitetType
    {
        public class Command : IRequest<AktivitetType>
        {
            public AktivitetType Aktivitettype { get; set; }
        }

        public class Handler : IRequestHandler<Command, AktivitetType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<AktivitetType> Handle(Command command, CancellationToken cancellationToken)
            {
                var aktivitettype = await _context.AktivitetType
                    .FirstOrDefaultAsync(i => i.Id == command.Aktivitettype.Id, cancellationToken);

                if (aktivitettype == null) throw new Exception($"Fant ikke aktivitettype med id {command.Aktivitettype.Id}");

                aktivitettype.Navn = command.Aktivitettype.Navn;

                _context.AktivitetType.Update(aktivitettype);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<AktivitetType>(aktivitettype);
                return mapped;
            }
        }
    }
}