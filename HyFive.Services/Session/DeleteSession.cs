using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Models.V1.Session;
using Microsoft.EntityFrameworkCore;

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

        public class Handler : IRequestHandler<Command, DeleteSessionResponse>
        {
            private readonly HandHygieneContext _databaseContext;

            public Handler(HandHygieneContext databaseContext)
            {
                _databaseContext = databaseContext;
            }
            
            public async Task<DeleteSessionResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new DeleteSessionResponse();
                var sessionAndType = await _databaseContext.Session
                    .AsNoTracking()
                    .Include(s => s.Department).ThenInclude(a => a.Facility)
                    .Select(s => new {s.Id, s.Discriminator, TransferStatusCode = s.TransferStatus.Code, FacilityId = s.Department.Facility.Id})
                    .FirstOrDefaultAsync(s => 
                        s.Id == request.SessionId
                        && s.TransferStatusCode == request.TransferStatusCode
                        && s.FacilityId == request.FacilityId
                    );

                if (sessionAndType == null)
                {
                    throw new ArgumentException(
                        $"Did not find session with ID {request.SessionId} and transfer status code {request.TransferStatusCode}");
                }

                var sessionType = SessionHelper.GetSessionType(sessionAndType.Discriminator);

                switch (sessionType)
                {
                    case SessionType.FiveIndications:
                        response.Success = DeleteSessionFourIndicators(request.SessionId);
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
                        throw new ArgumentException(
                            $"Deletion of session type {sessionType} is not supported.");
                }

                return response;
            }

            private bool DeleteSessionFourIndicators(Guid sessionIdToDelete)
            {
                var session = _databaseContext.FiveIndicationsSession
                    .Include(s => s.Observations)
                    .FirstOrDefault(s => s.Id == sessionIdToDelete);
                _databaseContext.RemoveRange(session.Observations);
                _databaseContext.Remove(session);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool DeleteSessionHandJewelry(Guid sessionIdToDelete)
            {
                var session = _databaseContext.HandJewelrySession
                    .Include(s => s.Observations)
                    .FirstOrDefault(s => s.Id == sessionIdToDelete);
                _databaseContext.RemoveRange(session.Observations);
                _databaseContext.Remove(session);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool DeleteSessionGloves(Guid sessionIdToDelete)
            {
                var session = _databaseContext.GloveSession
                    .Include(s => s.Observations)
                    .FirstOrDefault(s => s.Id == sessionIdToDelete);
                _databaseContext.RemoveRange(session.Observations);
                _databaseContext.Remove(session);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool DeleteSessionProtectiveEquipment(Guid sessionIdToDelete)
            {
                var session = _databaseContext.ProtectiveEquipmentSession
                    .Include(s => s.Observations).ThenInclude(b => b.ProtectiveEquipmentList)
                    .FirstOrDefault(s => s.Id == sessionIdToDelete);
                if (session.Observations.Any())
                {
                    var equipment = session.Observations.SelectMany(o => o.ProtectiveEquipmentList);
                    if (equipment.Any())
                    {
                        _databaseContext.RemoveRange(equipment);
                    }
                    _databaseContext.RemoveRange(session.Observations);
                }
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
