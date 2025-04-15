using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Report.Glove;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransferStatusTypeConstants = HyFive.Models.V1.Constants.TransferStatusTypeConstants;

namespace HyFive.Services.Rapport.Observasjoner
{
    public class HentHanskeObservasjoner
    {
        public class Query : IRequest<IEnumerable<GloveObservationReport>>
        {
            public int AvdelingId { get; set; }
            public Guid? SesjonId { get; set; }
            public int ObservatorId { get; set; }
            public int InstitusjonId { get; set; }
            public DateTime? FraTid { get; set; }
            public DateTime? TilTid { get; set; }
            public AuthorizedRole Rolle { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<GloveObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<GloveObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var queryable = _context.GloveObservation
                    .Include(fo => fo.GloveSession).ThenInclude(fo => fo.Observer)
                    .Include(fo => fo.GloveSession).ThenInclude(fo => fo.Avdeling).ThenInclude(a => a.Institusjon).ThenInclude(i => i.Kommune)
                    .Include(fo => fo.HandhygieneEtterHanskebrukType)
                    .Include(fo => fo.HanskeUtenIndikasjonTyper)
                    .Include(fo => fo.HanskeMedIndikasjonTyper)
                    .Include(fo => fo.Rolle)
                    .AsNoTracking();

                if (query.Rolle == AuthorizedRole.Observer)
                {
                    queryable = queryable.Where(p => p.HanskeSesjon.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToCoordinator);
                }
                else if (query.Rolle == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.HanskeSesjon.Overforingstatus.Kode == TransferStatusTypeConstants.TransferredToFhi);
                }

                if (query.AvdelingId > 0)
                {
                    queryable = queryable.Where(o => o.HanskeSesjon.Avdeling.Id == query.AvdelingId);
                }

                if (query.InstitusjonId > 0)
                {
                    queryable = queryable.Where(o => o.HanskeSesjon.Avdeling.InstitusjonId == query.InstitusjonId);
                }

                if (query.ObservatorId > 0)
                {
                    queryable = queryable.Where(o => o.HanskeSesjon.Observator.Id == query.ObservatorId);
                }

                if (query.SesjonId != null)
                {
                    queryable = queryable.Where(o => o.HanskeSesjon.Id == query.SesjonId);
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
                                    .OrderBy(o => o.HanskeSesjon.Id)
                                    .ThenBy(o => o.Id)
                                    .ProjectTo<GloveObservationReport>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
            }
        }
    }
}
