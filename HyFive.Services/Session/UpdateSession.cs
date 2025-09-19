using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;

namespace HyFive.Services.Session
{
    public class UpdateSession
    {
        public class Command : IRequest<UpdateSessionResponse>
        {
            public Guid SessionId { get; set; }
            public int FacilityId { get; set; }
            public string Comment { get; set; }
            public DateTime? StartDate { get; set; }
        }

        public class Handler : IRequestHandler<Command, UpdateSessionResponse>
        {
            private readonly HandHygieneContext _databaseContext;

            public Handler(HandHygieneContext databaseContext)
            {
                _databaseContext = databaseContext;
            }
            
            public async Task<UpdateSessionResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new UpdateSessionResponse();

                var session = _databaseContext.Session.FirstOrDefault(s => s.Id == request.SessionId && s.Department.FacilityId == request.FacilityId);

                if (session == null)
                {
                    throw new ArgumentException(
                        $"Did not find session with ID:  {request.SessionId}");
                }

                if (request.Comment != null)
                {
                    session.Comment = request.Comment;
                }

                await _databaseContext.SaveChangesAsync();
                response.Success = true;
                return response;
            }
        }

        public class UpdateSessionResponse
        {
            public bool Success { get; set; }
        }
    }
}