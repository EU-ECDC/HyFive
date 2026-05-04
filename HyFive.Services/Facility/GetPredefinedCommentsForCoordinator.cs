using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.Place;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PredefinedComment = HyFive.Models.V1.OrganisationUnit.PredefinedComment;

namespace HyFive.Services.Facility
{
    public class GetPredefinedCommentsForCoordinator
    {
        public class Query : IRequest<List<PredefinedComment>>
        {
            public int OrganisationUnitId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<PredefinedComment>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<List<PredefinedComment>> Handle(Query request, CancellationToken cancellationToken)
            {
                var comments = await _context.PredefinedComment
                    .AsNoTracking()
                    .Where(pk => pk.OrganisationUnitId == request.OrganisationUnitId && pk.SessionType == SessionType.ProtectiveEquipment)
                    .ProjectTo<PredefinedComment>(_mapper.ConfigurationProvider)
                    .OrderBy(pk => pk.Comment)
                    .ToListAsync(cancellationToken);

                return comments;
            }
        }
    }
}
