using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Report.FiveIndications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TransferStatusTypeConstants = HyFive.Models.V1.Constants.TransferStatusTypeConstants;

namespace HyFive.Services.Report.Observations
{
    public class GetFiveIndicationsObservations
    {
        public class Query : IRequest<IEnumerable<FiveIndicationsObservationReport>>
        {
            public List<int> DepartmentIds { get; set; }
            public int? DepartmentId { get; set; }
            public Guid? SessionId { get; set; }
            public int ObserverId { get; set; }
            public List<int> FacilityIds { get; set; }
            public int? FacilityId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<FiveIndicationsObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;


            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<FiveIndicationsObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var queryable = _context.FiveIndicationsObservation
                    .Include(fo => fo.FiveIndicationsSession).ThenInclude(fo => fo.Observer)
                    .Include(fo => fo.FiveIndicationsSession).ThenInclude(fo => fo.Department).ThenInclude(a => a.Facility).ThenInclude(i => i.Municipality)
                    .Include(fo => fo.FiveIndicationsSession).ThenInclude(fo => fo.TransferStatus)
                    .Include(fo => fo.Activity)
                    .Include(fo => fo.IndicationTypes)
                    .Include(fo => fo.Role)
                    .AsNoTracking();

                if (query.Role == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.FiveIndicationsSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);
                }

                if (query.DepartmentIds != null && query.DepartmentIds.Count > 0)
                {
                    queryable = queryable.Where(o => query.DepartmentIds.Contains(o.FiveIndicationsSession.Department.Id));
                }
                else if (query.DepartmentId > 0)
                {
                    queryable = queryable.Where(o => o.FiveIndicationsSession.Department.Id == query.DepartmentId);
                }

                if (query.FacilityIds != null && query.FacilityIds.Count > 0)
                {
                    queryable = queryable.Where(o => query.FacilityIds.Contains(o.FiveIndicationsSession.Department.FacilityId));
                }
                else if (query.FacilityId > 0)
                {
                    queryable = queryable.Where(o => o.FiveIndicationsSession.Department.FacilityId == query.FacilityId);
                }

                if (query.ObserverId > 0)
                {
                    queryable = queryable.Where(o => o.FiveIndicationsSession.Observer.Id == query.ObserverId);
                }

                if (query.SessionId != null)
                {
                    queryable = queryable.Where(o => o.FiveIndicationsSession.Id == query.SessionId);
                }

                if (query.FromDate != null)
                {
                    var fromDateUtc = DateTime.SpecifyKind(query.FromDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.RegisteredTime.Date >= fromDateUtc);
                }

                if (query.ToDate != null)
                {
                    var toDateUtc = DateTime.SpecifyKind(query.ToDate.Value.Date, DateTimeKind.Utc);
                    queryable = queryable.Where(o => o.RegisteredTime.Date <= toDateUtc);
                }

                return await queryable
                                      .OrderBy(o => o.FiveIndicationsSession.Id)
                                      .ThenBy(o => o.Id)
                                      .ProjectTo<FiveIndicationsObservationReport>(_mapper.ConfigurationProvider)
                                      .ToListAsync();
            }
        }
    }
}
