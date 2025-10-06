using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public class UpdateCity
    {
        public class Command : IRequest<bool>
        {
            public Models.V1.Facility.City City { get; set; }
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
                var city = _context.City.FirstOrDefault(h => h.Id == command.City.Id);

                city.Name = command.City.Name;

                _context.City.Update(city);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}
