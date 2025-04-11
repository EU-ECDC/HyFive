using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class OppdaterHelseforetaket
    {
        public class Command : IRequest<bool>
        {
            public Modeller.V1.Institution.HealthcareEnterprise Helseforetak { get; set; }
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
                                                        .FirstOrDefault(h => h.Id == command.Helseforetak.Id);

                helseforetak.Name = command.Helseforetak.Name;
                helseforetak.RegionaltHealthcareProvider = _context.RegionaltHealthcareProvider.Find(command.Helseforetak.RegionaltHelseforetakId);

                _context.HealthcareProvider.Update(helseforetak);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
