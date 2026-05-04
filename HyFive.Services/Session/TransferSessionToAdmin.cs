using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Session
{
    public class TransferSessionToAdmin
    {
        public class Query : IRequest<SessionOverviewReport>
        {
            public Guid SessionId { get; set; }
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, SessionOverviewReport>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<SessionOverviewReport> Handle(Query request, CancellationToken cancellationToken)
            {
                var session = await _context.Session
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .FirstOrDefaultAsync(x => x.Id == request.SessionId, cancellationToken);

                if (session == null)
                    throw new DomainException("SessionNotFound", request.SessionId);

                var allowed = await IsAncestorAsync(
                    ancestorId: request.FacilityId,
                    nodeId: session.OrganisationUnitId,
                    cancellationToken);

                if (!allowed)
                    throw new DomainException("SessionNotLinkedToFacility", request.SessionId, request.FacilityId);

                var transferredToAdmin = await _context.TransferStatusType.FirstOrDefaultAsync(x => x.Code == TransferStatusTypeConstants.TransferredToAdmin, cancellationToken);

                session.TransferStatusId = transferredToAdmin.Id;

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Domain.Session.Session, SessionOverviewReport>(session);
            }

            private async Task<bool> IsAncestorAsync(int ancestorId, int nodeId, CancellationToken cancellationToken)
            {
                // Walk up ParentId chain: node -> parent -> ... until null
                if (ancestorId == nodeId)
                    return true;

                var currentId = nodeId;
                var safety = 0;

                while (safety++ < 100)
                {
                    var parentId = await _context.OrganisationUnit
                        .AsNoTracking()
                        .Where(o => o.Id == currentId)
                        .Select(o => o.ParentId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (!parentId.HasValue)
                        return false;

                    if (parentId.Value == ancestorId)
                        return true;

                    currentId = parentId.Value;
                }

                // cycle protection / corrupt data
                return false;
            }
        }
    }
}