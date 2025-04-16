using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.ForesporselOmBrukertilgang
{
    public class HentInstitusjoner
    {
        public class Query : IRequest<List<Models.V1.UserAccessRequest.InstitutionForUserAccessRequest>>
        {
        }

        public class Handler : IRequestHandler<Query, List<Models.V1.UserAccessRequest.InstitutionForUserAccessRequest>>
        {
            private readonly int InstitusjonIdForFHI = 1;

            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<List<Models.V1.UserAccessRequest.InstitutionForUserAccessRequest>> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjoner = _context.Institution.AsNoTracking()
                             .Where(i=> i.Id != InstitusjonIdForFHI)
                             .Select(i => new Models.V1.UserAccessRequest.InstitutionForUserAccessRequest()
                             {
                                 Id = i.Id, 
                                 Name = i.Name
                             })
                             .OrderBy(i=>i.Name)
                             .ToList();

                return institusjoner;
            }
        }
    }
}
