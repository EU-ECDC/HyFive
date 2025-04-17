using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HealthcareOrganization
{
    public class CreateHealthcareOrganization
    {
        public class Command : IRequest<bool>
        {
            public CreateHealthcareOrganizationRequest HealthcareOrganization { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var healthcareOrganization = _context.HealthcareOrganization.Include(h => h.RegionaltHealthcareOrganization)
                                                        .FirstOrDefault(h => h.Name.ToLower() == request.HealthcareOrganization.Name.ToLower());

                if (healthcareOrganization != null)
                    return false;

                healthcareOrganization = new Domain.Place.HealthcareOrganization
                {
                    Name = request.HealthcareOrganization.Name,
                    RegionaltHealthcareOrganization = _context.RegionaltHealthcareOrganization.Find(request.HealthcareOrganization.RegionaltHealthcareOrganizationId)
                };

                _context.HealthcareOrganization.Add(healthcareOrganization);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
