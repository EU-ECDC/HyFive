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
    public class UpdateAddress
    {
        public class Command : IRequest<bool>
        {
            public int Id { get; set; }
            public UpdateAddressRequest Address { get; set; }
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
                
                var address = await _context.Address
                    .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

                if (address == null)
                    throw new ValidationException("AddressNotFound");

                // Normalize
                var city = command.Address.City?.Trim();
                var street = command.Address.Street?.Trim();
                var postal = command.Address.PostalCode?.Trim();

                if (string.IsNullOrWhiteSpace(city))
                    throw new ValidationException("CityRequired");

                address.City = city!;
                address.Street = street;
                address.PostalCode = postal;

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }
}
