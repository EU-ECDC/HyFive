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

namespace HyFive.Services.Report.Observations
{
    public class GetGloveObservations
    {
        public class Query : IRequest<IEnumerable<GloveObservationReport>>
        {
            public List<int> DepartmentIds { get; set; }
            public int? DepartmentId { get; set; }
            public Guid? SessionId { get; set; }
            public int ObserverId { get; set; }
            public List<int> InstitutionIds { get; set; }
            public int? InstitutionId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public AuthorizedRole Role { get; set; }
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
                    .Include(fo => fo.GloveSession).ThenInclude(fo => fo.Department).ThenInclude(a => a.Institution).ThenInclude(i => i.Municipality)
                    .Include(fo => fo.PostGloveHandHygieneType)
                    .Include(fo => fo.GloveWithIndicationTypes)
                    .Include(fo => fo.GloveWithIndicationTypes)
                    .Include(fo => fo.Role)
                    .AsNoTracking();

                if (query.Role == AuthorizedRole.Observer)
                {
                    queryable = queryable.Where(p => p.GloveSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToCoordinator);
                }
                else if (query.Role == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.GloveSession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin);
                }

                if (query.DepartmentIds != null && query.DepartmentIds.Any())
                {
                    queryable = queryable.Where(o => query.DepartmentIds.Contains(o.GloveSession.Department.Id));
                }
                else if (query.DepartmentId > 0)
                {
                    queryable = queryable.Where(o => o.GloveSession.Department.Id == query.DepartmentId);
                }

                if (query.InstitutionIds != null && query.InstitutionIds.Any())
                {
                    queryable = queryable.Where(o => query.InstitutionIds.Contains(o.GloveSession.Department.InstitutionId));
                }

                if (query.ObserverId > 0)
                {
                    queryable = queryable.Where(o => o.GloveSession.Observer.Id == query.ObserverId);
                }

                if (query.SessionId != null)
                {
                    queryable = queryable.Where(o => o.GloveSession.Id == query.SessionId);
                }

                if (query.FromDate != null)
                {
                    queryable = queryable.Where(o => o.RegisteredTime.Date >= query.FromDate.Value.Date);
                }
                
                if (query.ToDate != null)
                {
                    queryable = queryable.Where(o => o.RegisteredTime.Date <= query.ToDate.Value.Date);
                }

                return await queryable
                                    .OrderBy(o => o.GloveSession.Id)
                                    .ThenBy(o => o.Id)
                                    .ProjectTo<GloveObservationReport>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
            }
        }
    }
}
