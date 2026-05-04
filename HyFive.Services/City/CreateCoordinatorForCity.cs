using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.User;
using HyFive.Services.Helpers;
using HyFive.Services.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public class CreateCoordinatorForCity
    {
        public class Command : IRequest<Status>
        {
            public CityCoordinator Coordinator { get; set; }
            public string City { get; set; }
        }

        public class Handler : IRequestHandler<Command, Status>
        {
            private readonly HandHygieneContext _context;



            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<Status> Handle(Command command, CancellationToken cancellationToken)
            {
                if (!CoordinatorForCityValidator.CanBeUpdated(command.Coordinator, out var errorCode, out var args))
                    throw new ValidationException(errorCode, args);

                if (string.IsNullOrWhiteSpace(command.City))
                    throw new ValidationException("CityRequired");

                var city = command.City.Trim();

                // 1) Allow only facilities that belong to this city (safety)
                var allowedFacilityIds = await GetFacilityIdsInCity(city, cancellationToken);

                var requestedFacilityIds = command.Coordinator.Facilities?
                    .Select(x => x.Id)
                    .Distinct()
                    .ToHashSet() ?? new HashSet<int>();

                requestedFacilityIds.IntersectWith(allowedFacilityIds);

                if (requestedFacilityIds.Count == 0)
                    throw new ValidationException("FacilitiesForCityNotFound", city);

                // 2) Create or load ONE coordinator user (no more per-facility rows)
                var coordinator = await _context.User
                .WithPermission(PermissionLevelConstants.Coordinator)
                .FirstOrDefaultAsync(u => u.Email == command.Coordinator.Email, cancellationToken);

                if (coordinator == null)
                {
                    coordinator = new Coordinator
                    {
                        FirstName = command.Coordinator.FirstName,
                        LastName = command.Coordinator.LastName,
                        Email = command.Coordinator.Email,
                        IdentityPseudonym = command.Coordinator.IdentityPseudonym,
                        IsDeactivated = command.Coordinator.IsDeactivated
                    };
                    _context.Add(coordinator);
                    await _context.SaveChangesAsync(cancellationToken); // get Id
                }
                else
                {
                    // Optional: update profile + reactivate
                    coordinator.FirstName = command.Coordinator.FirstName;
                    coordinator.LastName = command.Coordinator.LastName;
                    coordinator.IdentityPseudonym = command.Coordinator.IdentityPseudonym;
                    coordinator.IsDeactivated = command.Coordinator.IsDeactivated;
                }

                // 3) Ensure permissions:
                // Coordinator must also be Observer => add BOTH levels for each facility
                await EnsurePermissions(
                    userId: coordinator.Id,
                    organisationUnitIds: requestedFacilityIds,
                    permissionLevels: new[] { "Coordinator", "Observer" },
                    cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new Status { Success = true };
            }
            private async Task<List<int>> GetFacilityIdsInCity(string city, CancellationToken ct)
            {
                return await
                    (from f in _context.OrganisationUnit.AsNoTracking()
                     join a in _context.Address.AsNoTracking() on f.AddressId equals a.Id
                     where f.ParentId == null
                           && a.City != null
                           && EF.Functions.ILike(a.City, city)
                     select f.Id)
                    .Distinct()
                    .ToListAsync(ct);
            }

            private async Task EnsurePermissions(
                int userId,
                HashSet<int> organisationUnitIds,
                IEnumerable<string> permissionLevels,
                CancellationToken ct)
            {
                var levels = permissionLevels.Distinct().ToList();

                var existing = await _context.UserPermission.AsNoTracking()
                    .Where(p => p.UserId == userId
                                && p.OrganisationUnitId.HasValue
                                && organisationUnitIds.Contains(p.OrganisationUnitId.Value)
                                && levels.Contains(p.PermissionLevel))
                    .Select(p => new { p.OrganisationUnitId, p.PermissionLevel })
                    .ToListAsync(ct);

                var existingSet = existing
                    .Select(x => (x.OrganisationUnitId, x.PermissionLevel))
                    .ToHashSet();

                foreach (var ouId in organisationUnitIds)
                {
                    foreach (var level in levels)
                    {
                        if (existingSet.Contains((ouId, level)))
                            continue;

                        _context.UserPermission.Add(new HyFive.Domain.User.UserPermission
                        {
                            UserId = userId,
                            OrganisationUnitId = ouId,
                            PermissionLevel = level
                        });
                    }
                }
            }
        }
    }
}
