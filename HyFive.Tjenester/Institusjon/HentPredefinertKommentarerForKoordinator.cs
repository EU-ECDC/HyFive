using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domene.Place;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PredefinedComment = HyFive.Models.V1.Institution.PredefinedComment;

namespace HyFive.Services.Institusjon
{
    public class HentPredefinertKommentarerForKoordinator
    {
        public class Query : IRequest<List<PredefinedComment>>
        {
            public int Institusjonid { get; set; }
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
                var kommentarer = await _context.PredefinedComments
                    .AsNoTracking()
                    .Where(pk => pk.InstitutionId == request.Institusjonid && pk.SessionType == SessionType.ProtectiveEquipment)
                    .ProjectTo<PredefinedComment>(_mapper.ConfigurationProvider)
                    .OrderBy(pk => pk.Comment)
                    .ToListAsync(cancellationToken);

                return kommentarer;
            }
        }
    }
}
