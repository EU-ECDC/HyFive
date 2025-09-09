using AutoMapper;
using HyFive.DataAccess;
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
    public class TransferSessionToFhi
    {
        public class Query : IRequest<SessionOverviewReport>
        {
            public Guid SessionId { get; set; }
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
                    .Include(s => s.Department)
                    .Include(s => s.Observer)
                    .Include(s => s.TransferStatus)
                    .FirstOrDefaultAsync(x => x.Id == request.SessionId);

                var transferredToFHI = await _context.TransferStatusType.FirstOrDefaultAsync(x => x.Code == TransferStatusTypeConstants.TransferredToAdmin);

                session.TransferStatus = transferredToFHI;

                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<Domain.Session.Session, SessionOverviewReport>(session);
            }
        }
    }
}