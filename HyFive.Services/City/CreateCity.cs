using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public class CreateCity
    {
        public class Command : IRequest<bool>
        {
            public CreateCityRequest City { get; set; }
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
                var city = await _context.City.FirstOrDefaultAsync(h => h.Name.ToLower() == request.City.Name.ToLower(), cancellationToken);

                if (city != null)
                    return false;

                city = new Domain.Place.City
                {
                    Name = request.City.Name
                };

                _context.City.Add(city);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }
}
