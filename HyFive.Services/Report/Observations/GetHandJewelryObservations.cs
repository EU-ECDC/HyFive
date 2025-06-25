using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.Report.HandJewelry;
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
    public class GetHandJewelryObservations
    {
        public class Query : IRequest<IEnumerable<HandJewelryObservationReport>>
        {
            public int DepartmentId { get; set; }
            public Guid? SessionId { get; set; }
            public int ObserverId { get; set; }
            public int InstitutionId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToTime { get; set; }
            public AuthorizedRole Role { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<HandJewelryObservationReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IEnumerable<HandJewelryObservationReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var queryable = _context.HandJewelryObservation
                    .Include(fo => fo.HandJewelrySession).ThenInclude(fo => fo.Observer)
                    .Include(fo => fo.HandJewelrySession).ThenInclude(fo => fo.Department).ThenInclude(a => a.Institution).ThenInclude(i => i.Municipality)
                    .Include(fo => fo.HandJewelries)
                    .Include(fo => fo.Role)
                    .AsNoTracking();

                if (query.Role == AuthorizedRole.Observer)
                {
                    queryable = queryable.Where(p => p.HandJewelrySession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToCoordinator);
                }
                else if (query.Role == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.HandJewelrySession.TransferStatus.Code == TransferStatusTypeConstants.TransferredToFhi);
                }

                if (query.DepartmentId > 0)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.Department.Id == query.DepartmentId);
                }

                if (query.InstitutionId > 0)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.Department.InstitutionId == query.InstitutionId);
                }

                if (query.ObserverId > 0)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.Observer.Id == query.ObserverId);
                }

                if (query.SessionId != null)
                {
                    queryable = queryable.Where(o => o.HandJewelrySession.Id == query.SessionId);
                }

                if (query.FromDate != null)
                {
                    queryable = queryable.Where(o => o.RegisteredTime.Date >= query.FromDate.Value.Date);
                }
                
                if (query.ToTime != null)
                {
                    queryable = queryable.Where(o => o.RegisteredTime.Date <= query.ToTime.Value.Date);
                }

                return await queryable
                                    .OrderBy(o => o.HandJewelrySession.Id)
                                    .ThenBy(o => o.Id)
                                    .ProjectTo<HandJewelryObservationReport>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
            }
        }
    }
}
