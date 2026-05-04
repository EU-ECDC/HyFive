using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Services.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public class Handler : BaseHandler, IRequestHandler<Command, UpdateSessionResponse>
        {
            private readonly HandHygieneContext _databaseContext;

            public Handler(HandHygieneContext databaseContext, IMapper mapper) : base(databaseContext, mapper)
            {
                _databaseContext = databaseContext;
            }
            
            public async Task<UpdateSessionResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                var response = new UpdateSessionResponse();

                // 1) Load session + its OU id (no Department anymore)
                var session = await _databaseContext.Session
                    .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

                if (session == null)
                    throw new DomainException("SessionNotFound", request.SessionId);

                // 2) Verify session belongs to facility via OU tree (facility is ancestor of session OU)
                var belongsToFacility = await IsAncestorAsync( _databaseContext,request.FacilityId, session.OrganisationUnitId,cancellationToken);
                if (!belongsToFacility)
                    throw new DomainException("SessionNotLinkedToFacility", request.SessionId, request.FacilityId);

                // 3) Update fields
                if (request.Comment != null)
                    session.Comment = request.Comment;

                await _databaseContext.SaveChangesAsync(cancellationToken);

                response.Success = true;
                return response;
            }
        }
    }

    public class UpdateSessionResponse
    {
        public bool Success { get; set; }
    }
    
}