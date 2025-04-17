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

namespace HyFive.Services.Rapport.Observations
{
    public class GetProtectiveEquipmentObservations
    {
        public class Query : IRequest<IEnumerable<PPEObservationReport>>
        {
            public int DepartmentId { get; set; }
            public Guid? SessionId { get; set; }
            public int ObserverId { get; set; }
            public int InstitutionId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToTime { get; set; }
            public AuthorizedRole Role { get; set; }
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
                    .Include(fo => fo.ProtectiveEquipmentSession).ThenInclude(fo => fo.Department).ThenInclude(a => a.Institution).ThenInclude(i => i.Municipality)
                    .Include(fo => fo.ProtectiveEquipmentSession).ThenInclude(fo => fo.TransmissionStatus)
                    .Include(fo => fo.SettingType)
                    .Include(fo => fo.ProtectiveEquipmentList).ThenInclude(bu => bu.MisuseTypes)
                    .Include(fo => fo.ProtectiveEquipmentList).ThenInclude(bu => bu.EquipmentType)
                    .Include(fo => fo.Role)
                    .AsNoTracking()
                    .SelectMany(s => s.ProtectiveEquipmentList);

                if (query.Role == AuthorizedRole.Administrator)
                {
                    queryable = queryable.Where(p => p.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi);
                }

                if (query.DepartmentId > 0)
                {
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Id == query.DepartmentId);
                }

                if (query.InstitutionId > 0)
                {
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.InstitutionId == query.InstitutionId);
                }
                if (query.ObserverId > 0)
                {
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Observer.Id == query.ObserverId);
                }

                if (query.SessionId != null)
                {
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Id == query.SessionId);
                }
                if (query.FromDate != null)
                {                    
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.RegistrationTime.Date >= query.FromDate.Value.Date);
                }
                
                if (query.ToTime != null)
                {
                    queryable = queryable.Where(o => o.ProtectiveEquipmentObservation.RegistrationTime.Date <= query.ToTime.Value.Date);
                }
                return await queryable
                                    .OrderBy(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Id)
                                    .ThenBy(o => o.ProtectiveEquipmentObservation.Id)
                                    .ProjectTo<PPEObservationReport>(_mapper.ConfigurationProvider)
                                    .ToListAsync();
            }
        }
    }
}
