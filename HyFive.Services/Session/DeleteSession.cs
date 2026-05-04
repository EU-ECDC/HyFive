using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Session;
using HyFive.Services.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Session
{
    public class DeleteSession
    {
        public class Command : IRequest<DeleteSessionResponse>
        {
            public Guid SessionId { get; set; }
            public int FacilityId { get; set; }
            public string TransferStatusCode { get; set; }
        }

        public class Handler : BaseHandler, IRequestHandler<Command, DeleteSessionResponse>
        {
            private readonly HandHygieneContext _databaseContext;

            public Handler(HandHygieneContext databaseContext, IMapper mapper) : base(databaseContext, mapper)
            {
                _databaseContext = databaseContext;
            }
            
            public async Task<DeleteSessionResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new DeleteSessionResponse();
                
                // 1) Load minimal info needed for validation + branching
                var sessionInfo = await _databaseContext.Session
                    .AsNoTracking()
                    .Where(s => s.Id == request.SessionId)
                    .Select(s => new
                    {
                        s.Id,
                        s.Discriminator,
                        s.OrganisationUnitId,
                        TransferStatusCode = s.TransferStatus.Code
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (sessionInfo == null)
                    throw new DomainException("SessionNotFound", request.SessionId);

                if (sessionInfo.TransferStatusCode != request.TransferStatusCode)
                    throw new DomainException("SessionWithTransferStatusNotFound", request.SessionId, request.TransferStatusCode);

                // 2) Verify the session OU belongs to the facility (facility is ancestor of the session's OU)
                var belongsToFacility = await IsAncestorAsync(_databaseContext, request.FacilityId, sessionInfo.OrganisationUnitId, cancellationToken);
                if (!belongsToFacility)
                    throw new DomainException("SessionNotLinkedToFacility", request.SessionId, request.FacilityId);

                var sessionType = SessionHelper.GetSessionType(sessionInfo.Discriminator);

                switch (sessionType)
                {
                    case SessionType.FiveIndications:
                        response.Success = DeleteSessionFiveIndicators(request.SessionId);
                        break;
                    case SessionType.HandJewelry:
                        response.Success = DeleteSessionHandJewelry(request.SessionId);
                        break;
                    case SessionType.Gloves:
                        response.Success = DeleteSessionGloves(request.SessionId);
                        break;
                    case SessionType.ProtectiveEquipment:
                        response.Success = DeleteSessionProtectiveEquipment(request.SessionId);
                        break;
                    default:
                        throw new DomainException("SessionTypeDeletionNotSupported", sessionType);
                }

                return response;
            }
            
            private bool DeleteSessionFiveIndicators(Guid sessionIdToDelete)
            {
                return DeleteSessionWithObservations<HyFive.Domain.Session.FiveIndicationsSession>(
                sessionIdToDelete,
                query => query.Include(s => s.Observations),
                session => _databaseContext.RemoveRange(session.Observations));
            }

            private bool DeleteSessionHandJewelry(Guid sessionIdToDelete)
            {
                return DeleteSessionWithObservations<HyFive.Domain.Session.HandJewelrySession>(
                sessionIdToDelete,
                query => query.Include(s => s.Observations),
                session => _databaseContext.RemoveRange(session.Observations));
            }

            private bool DeleteSessionGloves(Guid sessionIdToDelete)
            {
                return DeleteSessionWithObservations<HyFive.Domain.Session.GloveSession>(
                sessionIdToDelete,
                query => query.Include(s => s.Observations),
                session => _databaseContext.RemoveRange(session.Observations));
            }

            private bool DeleteSessionProtectiveEquipment(Guid sessionIdToDelete)
            {
                return DeleteSessionWithObservations<HyFive.Domain.Session.ProtectiveEquipmentSession>(
                sessionIdToDelete,
                query => query.Include(s => s.Observations)
                              .ThenInclude(o => o.ProtectiveEquipmentList),
                session =>
                {
                    if (session.Observations.Any())
                    {
                        var equipment = session.Observations.SelectMany(o => o.ProtectiveEquipmentList).ToList();
                        if (equipment.Any())
                            _databaseContext.RemoveRange(equipment);

                        _databaseContext.RemoveRange(session.Observations);
                    }
                });
            }

            private bool DeleteSessionWithObservations<TSession>(
                Guid sessionIdToDelete,
                Func<IQueryable<TSession>, IQueryable<TSession>> includeQuery,
                Action<TSession> deleteChildren)
                where TSession : class
            {
                var session = includeQuery(_databaseContext.Set<TSession>())
                    .FirstOrDefault(s => EF.Property<Guid>(s, "Id") == sessionIdToDelete);

                if (session == null)
                    return false;

                deleteChildren(session);

                _databaseContext.Remove(session);
                _databaseContext.SaveChanges();
                return true;
            }
        }

        public class DeleteSessionResponse
        {
            public bool Success { get; set; }
        }
    }
}
