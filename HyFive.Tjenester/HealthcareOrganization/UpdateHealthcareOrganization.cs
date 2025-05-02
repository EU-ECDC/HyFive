using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HealthcareOrganization
{
    public class UpdateHealthcareOrganization
    {
        public class Command : IRequest<bool>
        {
            public Models.V1.Institution.HealthcareOrganization HealthcareOrganization { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var healthcareOrganization = _context.HealthcareOrganization.Include(h => h.RegionalHealthcareOrganization)
                                                        .FirstOrDefault(h => h.Id == command.HealthcareOrganization.Id);

                healthcareOrganization.Name = command.HealthcareOrganization.Name;
                healthcareOrganization.RegionalHealthcareOrganization = _context.RegionalHealthcareOrganization.Find(command.HealthcareOrganization.RegionaltHealthcareOrganizationId);

                _context.HealthcareOrganization.Update(healthcareOrganization);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
