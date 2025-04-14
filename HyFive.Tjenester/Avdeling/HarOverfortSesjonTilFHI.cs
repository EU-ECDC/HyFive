using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Avdeling
{
    public class HarOverfortSesjonTilFHI
    {
        public class Command : IRequest<bool>
        {
            public int AvdelingId { get; set; }
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
                var sesjonerOverfortTilFHI = await _context.Sesjon.Where(s => s.Department.Id == request.AvdelingId && s.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi).AnyAsync();

                return sesjonerOverfortTilFHI;
            }
        }
    }

}
