using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Rapport.FireIndikasjoner;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OverforingstatusTypeKonstanter = HyFive.Modeller.V1.Konstanter.OverforingstatusTypeKonstanter;

namespace HyFive.Tjenester.Rapport.Observasjoner
{
    public class HentFireIndikasjonerObservasjoner
    {
        public class Query : IRequest<IEnumerable<FireIndikasjonerObservasjonRapport>>
        {
            public int AvdelingId { get; set; }
            public Guid? SesjonId { get; set; }
            public int ObservatorId { get; set; }
            public int InstitusjonId { get; set; }
            public DateTime? FraTid { get; set; }
            public DateTime? TilTid { get; set; }
            public AuthorizedRole Rolle { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<FireIndikasjonerObservasjonRapport>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;


            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<FireIndikasjonerObservasjonRapport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var queryable = _context.FireIndikasjonerObservasjon
                    .Include(fo => fo.FireIndikasjonerSesjon).ThenInclude(fo => fo.Observator)
                    .Include(fo => fo.FireIndikasjonerSesjon).ThenInclude(fo => fo.Avdeling).ThenInclude(a => a.Institusjon).ThenInclude(i => i.Kommune)
                    .Include(fo => fo.FireIndikasjonerSesjon).ThenInclude(fo => fo.Overforingstatus)
                    .Include(fo => fo.Aktivitet)
                    .Include(fo => fo.Indikasjonstyper)
                    .Include(fo => fo.Rolle)
                    .AsNoTracking();

                if (query.Rolle == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.FireIndikasjonerSesjon.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi);
                }

                if (query.AvdelingId > 0)
                {
                    queryable = queryable.Where(o => o.FireIndikasjonerSesjon.Avdeling.Id == query.AvdelingId);
                }

                if (query.InstitusjonId > 0)
                {
                    queryable = queryable.Where(o => o.FireIndikasjonerSesjon.Avdeling.InstitusjonId == query.InstitusjonId);
                }
                if (query.ObservatorId > 0)
                {
                    queryable = queryable.Where(o => o.FireIndikasjonerSesjon.Observator.Id == query.ObservatorId);
                }

                if (query.SesjonId != null)
                {
                    queryable = queryable.Where(o => o.FireIndikasjonerSesjon.Id == query.SesjonId);
                }
                if (query.FraTid != null)
                {
                    queryable = queryable.Where(o => o.Registrerttidspunkt.Date >= query.FraTid.Value.Date);
                }
                
                if (query.TilTid != null)
                {
                    queryable = queryable.Where(o => o.Registrerttidspunkt.Date <= query.TilTid.Value.Date);
                }
                return await queryable
                                      .OrderBy(o => o.FireIndikasjonerSesjon.Id)
                                      .ThenBy(o => o.Id)
                                      .ProjectTo<FireIndikasjonerObservasjonRapport>(_mapper.ConfigurationProvider)
                                      .ToListAsync();
            }
        }
    }
}
