using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Konstanter;
using HyFive.Modeller.V1.Oversikt;
using HyFive.Modeller.V1.Sesjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Sesjon
{
    public class OverforSesjonTilFHI
    {
        public class Query : IRequest<SesjonOversiktRapport>
        {
            public Guid SesjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, SesjonOversiktRapport>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<SesjonOversiktRapport> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjon = await _context.Sesjon
                    .Include(s => s.Avdeling)
                    .Include(s => s.Observator)
                    .Include(s => s.Overforingstatus)
                    .FirstOrDefaultAsync(x => x.Id == request.SesjonId);

                var overfortTilFHI = await _context.OverforingstatusType.FirstOrDefaultAsync(x => x.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi);

                sesjon.Overforingstatus = overfortTilFHI;

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Domene.Sesjon.Sesjon, SesjonOversiktRapport>(sesjon);
            }
        }
    }
}