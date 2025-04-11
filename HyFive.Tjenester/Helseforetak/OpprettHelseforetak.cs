using HyFive.DataAccess;
using HyFive.Modeller.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Helseforetak
{
    public class OpprettHelseforetak
    {
        public class Command : IRequest<bool>
        {
            public CreateHealthEnterpriseRequest Helseforetak { get; set; }
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
                var helseforetak = _context.HealthcareProvider.Include(h => h.RegionaltHealthcareProvider)
                                                        .FirstOrDefault(h => h.Name.ToLower() == request.Helseforetak.Name.ToLower());

                if (helseforetak != null)
                    return false;

                helseforetak = new Domene.Place.HealthcareProvider
                {
                    Name = request.Helseforetak.Name,
                    RegionaltHealthcareProvider = _context.RegionaltHealthcareProvider.Find(request.Helseforetak.RegionaltHelseforetakId)
                };

                _context.HealthcareProvider.Add(helseforetak);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
