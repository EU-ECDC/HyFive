using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Rapport.Handsmykke;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OverforingstatusTypeKonstanter = HyFive.Modeller.V1.Konstanter.OverforingstatusTypeKonstanter;

namespace HyFive.Tjenester.Rapport.Observasjoner
{
    public class HentHandsmykkeObservasjoner
    {
        public class Query : IRequest<IEnumerable<HandsmykkeObservasjonRapport>>
        {
            public int AvdelingId { get; set; }
            public Guid? SesjonId { get; set; }
            public int ObservatorId { get; set; }
            public int InstitusjonId { get; set; }
            public DateTime? FraTid { get; set; }
            public DateTime? TilTid { get; set; }
            public AuthorizedRole Rolle { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<HandsmykkeObservasjonRapport>>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<HandsmykkeObservasjonRapport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var queryable = _context.HandsmykkeObservasjon
                    .Include(fo => fo.HandsmykkeSesjon).ThenInclude(fo => fo.Observator)
                    .Include(fo => fo.HandsmykkeSesjon).ThenInclude(fo => fo.Avdeling).ThenInclude(a => a.Institusjon).ThenInclude(i => i.Kommune)
                    .Include(fo => fo.Handsmykker)
                    .Include(fo => fo.Rolle)
                    .AsNoTracking();

                if (query.Rolle == AuthorizedRole.Observator)
                {
                    queryable = queryable.Where(p => p.HandsmykkeSesjon.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilKoordinator);
                }
                else if (query.Rolle == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.HandsmykkeSesjon.Overforingstatus.Kode == OverforingstatusTypeKonstanter.OverfortTilFhi);
                }

                if (query.AvdelingId > 0)
                {
                    queryable = queryable.Where(o => o.HandsmykkeSesjon.Avdeling.Id == query.AvdelingId);
                }

                if (query.InstitusjonId > 0)
                {
                    queryable = queryable.Where(o => o.HandsmykkeSesjon.Avdeling.InstitusjonId == query.InstitusjonId);
                }

                if (query.ObservatorId > 0)
                {
                    queryable = queryable.Where(o => o.HandsmykkeSesjon.Observator.Id == query.ObservatorId);
                }

                if (query.SesjonId != null)
                {
                    queryable = queryable.Where(o => o.HandsmykkeSesjon.Id == query.SesjonId);
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
                                    .OrderBy(o => o.HandsmykkeSesjon.Id)
                                    .ThenBy(o => o.Id)
                                    .ProjectTo<HandsmykkeObservasjonRapport>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
            }
        }
    }
}
