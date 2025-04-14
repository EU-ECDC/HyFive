using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Helseforetak
{
    public class OppdaterHelseforetaket
    {
        public class Command : IRequest<bool>
        {
            public Models.V1.Institution.HealthcareEnterprise HealthcareProvider { get; set; }
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
                var helseforetak = _context.HealthcareProvider.Include(h => h.RegionaltHealthcareProvider)
                                                        .FirstOrDefault(h => h.Id == command.HealthcareProvider.Id);

                helseforetak.Name = command.HealthcareProvider.Name;
                helseforetak.RegionaltHealthcareProvider = _context.RegionaltHealthcareProvider.Find(command.HealthcareProvider.RegionaltHelseforetakId);

                _context.HealthcareProvider.Update(helseforetak);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
