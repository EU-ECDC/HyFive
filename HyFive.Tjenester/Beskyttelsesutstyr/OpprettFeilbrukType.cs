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
    public class OpprettFeilbrukType
    {
        public class Command : IRequest<FeilbrukType>
        {
            public int UtstyrTypeId { get; set; }
            public OpprettFeilbrukTypeRequest FeilbrukType { get; set; }
        }

        public class Handler : IRequestHandler<Command, FeilbrukType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<FeilbrukType> Handle(Command request, CancellationToken cancellationToken)
            {
                var utstyrType = await _context.BeskyttelsesutstyrType
                    .Include(but => but.Feilbruktyper)
                    .FirstOrDefaultAsync(but => but.Id == request.UtstyrTypeId, cancellationToken);
                if (utstyrType == null)
                {
                    throw new Exception("Kunne ikke finne utstyrType med ID " + request.UtstyrTypeId);
                }

                var feilbrukType = new Domene.Observasjon.Beskyttelsesutstyr.FeilbrukType()
                {
                    Navn = request.FeilbrukType.Navn
                };

                utstyrType.Feilbruktyper.Add(feilbrukType);

                _context.BeskyttelsesutstyrType.Update(utstyrType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<FeilbrukType>(feilbrukType);
                return mapped;
            }
        }
    }
}