using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public class CreateAddress
    {
        public class Command : IRequest<int>
        {
            public CreateAddressRequest Address { get; set; }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var city = request.Address.City?.Trim();
                var street = request.Address.Street?.Trim();
                var postal = request.Address.PostalCode?.Trim();

                if (string.IsNullOrWhiteSpace(city))
                    throw new ValidationException("CityRequired");

                var address = new Domain.Place.Address
                {
                    City = city!,
                    Street = street,
                    PostalCode = postal
                };

                _context.Address.Add(address);
                await _context.SaveChangesAsync(cancellationToken);

                return address.Id;
            }
        }
    }
}
