using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Report.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransferStatusTypeConstants = HyFive.Models.V1.Constants.TransferStatusTypeConstants;

namespace HyFive.Services.Rapport.Observasjoner
{
    public class HentBeskyttelsesutstyrObservasjoner
    {
        public class Query : IRequest<IEnumerable<PPEObservationReport>>
        {
            public int AvdelingId { get; set; }
            public Guid? SesjonId { get; set; }
            public int ObservatorId { get; set; }
            public int InstitusjonId { get; set; }
            public DateTime? FraTid { get; set; }
            public DateTime? TilTid { get; set; }
            public AuthorizedRole Rolle { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<PPEObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;


            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<PPEObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var queryable = _context.ProtectiveEquipmentObservation
                    .Include(fo => fo.ProtectiveEquipmentSession).ThenInclude(fo => fo.Observer)
                    .Include(fo => fo.ProtectiveEquipmentSession).ThenInclude(fo => fo.Avdeling).ThenInclude(a => a.Institusjon).ThenInclude(i => i.Kommune)
                    .Include(fo => fo.BeskyttelsesutstyrSesjon).ThenInclude(fo => fo.Overforingstatus)
                    .Include(fo => fo.Settingtype)
                    .Include(fo => fo.Beskyttelsesutstyrliste).ThenInclude(bu => bu.Feilbruktyper)
                    .Include(fo => fo.Beskyttelsesutstyrliste).ThenInclude(bu => bu.Utstyrstype)
                    .Include(fo => fo.Rolle)
                    .AsNoTracking()
                    .SelectMany(s => s.Beskyttelsesutstyrliste);

                if (query.Rolle == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToFhi);
                }

                if (query.AvdelingId > 0)
                {
                    queryable = queryable.Where(o => o.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Id == query.AvdelingId);
                }

                if (query.InstitusjonId > 0)
                {
                    queryable = queryable.Where(o => o.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.InstitusjonId == query.InstitusjonId);
                }
                if (query.ObservatorId > 0)
                {
                    queryable = queryable.Where(o => o.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Observator.Id == query.ObservatorId);
                }

                if (query.SesjonId != null)
                {
                    queryable = queryable.Where(o => o.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Id == query.SesjonId);
                }
                if (query.FraTid != null)
                {                    
                    queryable = queryable.Where(o => o.BeskyttelsesutstyrObservasjon.Registrerttidspunkt.Date >= query.FraTid.Value.Date);
                }
                
                if (query.TilTid != null)
                {
                    queryable = queryable.Where(o => o.BeskyttelsesutstyrObservasjon.Registrerttidspunkt.Date <= query.TilTid.Value.Date);
                }
                return await queryable
                                    .OrderBy(o => o.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Id)
                                    .ThenBy(o => o.BeskyttelsesutstyrObservasjon.Id)
                                    .ProjectTo<PPEObservationReport>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
            }
        }
    }
}
