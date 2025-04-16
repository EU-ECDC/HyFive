using HyFive.DataAccess;
using HyFive.Models.V1.UserAccessRequest;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.ForesporselOmBrukertilgang
{
    public class HentInstitusjon
    {
        public class Query : IRequest<InstitutionForUserAccessRequest>
        {
            public int InstitusjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, InstitutionForUserAccessRequest>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<InstitutionForUserAccessRequest> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjon = _context.Institution.AsNoTracking()
                    .FirstOrDefault(i => i.Id == request.InstitusjonId);

                var institusjonForForesporselOmBrukertilgang = new InstitutionForUserAccessRequest
                {
                    Id = institusjon.Id,
                    Name = institusjon.Name
                };

                return institusjonForForesporselOmBrukertilgang;
            }
        }
    }
}