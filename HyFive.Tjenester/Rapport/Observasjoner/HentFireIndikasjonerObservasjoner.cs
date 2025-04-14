using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Report.FourIndications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransferStatusTypeConstants = HyFive.Models.V1.Constants.TransferStatusTypeConstants;

namespace HyFive.Services.Rapport.Observasjoner
{
    public class HentFireIndikasjonerObservasjoner
    {
        public class Query : IRequest<IEnumerable<FourIndicationsObservationReport>>
        {
            public int AvdelingId { get; set; }
            public Guid? SesjonId { get; set; }
            public int ObservatorId { get; set; }
            public int InstitusjonId { get; set; }
            public DateTime? FraTid { get; set; }
            public DateTime? TilTid { get; set; }
            public AuthorizedRole Rolle { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<FourIndicationsObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;


            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<FourIndicationsObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var queryable = _context.FourIndicationsObservation
                    .Include(fo => fo.FourIndicationsSession).ThenInclude(fo => fo.Observer)
                    .Include(fo => fo.FourIndicationsSession).ThenInclude(fo => fo.Avdeling).ThenInclude(a => a.Institusjon).ThenInclude(i => i.Kommune)
                    .Include(fo => fo.FireIndikasjonerSesjon).ThenInclude(fo => fo.Overforingstatus)
                    .Include(fo => fo.Aktivitet)
                    .Include(fo => fo.Indikasjonstyper)
                    .Include(fo => fo.Rolle)
                    .AsNoTracking();

                if (query.Rolle == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.FireIndikasjonerSesjon.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToFhi);
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
                                      .ProjectTo<FourIndicationsObservationReport>(_mapper.ConfigurationProvider)
                                      .ToListAsync();
            }
        }
    }
}
