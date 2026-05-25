using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1;
using HyFive.Models.V1.User;
using HyFive.Services.Localization;
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
    public class UpdateCoordinatorForCity
    {
        public class Command : IRequest<Status>
        {
            public CityCoordinator Coordinator { get; set; }
            public int CityId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Status>
        {
            private readonly HandHygieneContext _context;

            private static readonly string[] LevelsToManage = { "Coordinator", "Observer" };

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<Status> Handle(Command command, CancellationToken cancellationToken)
            {

                if (!CoordinatorForCityValidator.CanBeUpdated(command.Coordinator, out var errorCode, out var args))
                    throw new ValidationException(errorCode, args);

                var cityId = command.CityId;
                var dto = command.Coordinator;

                // 1) load coordinator user (single row)
                var coordinator = await LoadCoordinator(dto, cancellationToken);
                if (coordinator == null)
                    throw new DomainException("CoordinatorNotFound");

                // 2) update coordinator profile
                UpdateCoordinatorProfile(coordinator, dto);

                // 3) facilities allowed for this city (safety: prevent cross-city assignments)
                var allowedFacilityIds = await GetFacilityIdsInCity(cityId, cancellationToken);

                // 4) requested facility ids (intersect allowed)
                var requestedFacilityIds = GetRequestedFacilityIds(dto, allowedFacilityIds);

                // 5) sync permissions (add/remove)
                await SyncCoordinatorFacilityPermissions(
                    coordinatorUserId: coordinator.Id,
                    requestedFacilityIds: requestedFacilityIds,
                    cancellationToken: cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return new Status { Success = true };
            }

            private async Task<Coordinator?> LoadCoordinator(CityCoordinator dto, CancellationToken ct)
            {
                return await _context.User
                    .OfType<Coordinator>()
                    .FirstOrDefaultAsync(u => u.Email == dto.Email, ct);
            }

            private static void UpdateCoordinatorProfile(Coordinator coordinator, CityCoordinator dto)
            {
                coordinator.FirstName = dto.FirstName;
                coordinator.LastName = dto.LastName;
                coordinator.Email = dto.Email;
                coordinator.IdentityPseudonym = dto.ModifiedPseudonym ?? dto.IdentityPseudonym;
                coordinator.IsDeactivated = dto.IsDeactivated;
            }

            private async Task<List<int>> GetFacilityIdsInCity(int cityId, CancellationToken ct)
            {
                return await
                    (from f in _context.OrganisationUnit.AsNoTracking()
                     join a in _context.Address.AsNoTracking() on f.AddressId equals a.Id
                     where f.ParentId == null
                           && a.City != null
                           && a.CityId == cityId
                     select f.Id)
                    .Distinct()
                    .ToListAsync(ct);
            }

            private static HashSet<int> GetRequestedFacilityIds(CityCoordinator dto, List<int> allowedFacilityIds)
            {
                var requested = dto.Facilities?
                    .Select(f => f.Id)
                    .Distinct()
                    .ToHashSet() ?? new HashSet<int>();

                // Keep only facilities that belong to the requested city
                requested.IntersectWith(allowedFacilityIds);
                return requested;
            }

            private async Task SyncCoordinatorFacilityPermissions(
            int coordinatorUserId,
            HashSet<int> requestedFacilityIds,
            CancellationToken cancellationToken)
            {
                // Load only facility-level permissions we manage for this user
                var existing = await _context.UserPermission
                    .Where(p => p.UserId == coordinatorUserId
                                && LevelsToManage.Contains(p.PermissionLevel))
                    .ToListAsync(cancellationToken);

                var requestedPairs = requestedFacilityIds
                .SelectMany(fid => LevelsToManage.Select(level => new { fid, level }))
                .ToList();

                // Remove permissions not requested anymore
                var toRemove = existing
                .Where(p => !requestedPairs.Any(r => r.fid == p.OrganisationUnitId && r.level == p.PermissionLevel))
                .ToList();

                if (toRemove.Count > 0)
                    _context.UserPermission.RemoveRange(toRemove);

                // Add new permissions
                var existingSet = existing
                .Select(p => (p.OrganisationUnitId, p.PermissionLevel))
                .ToHashSet();

                foreach (var facilityId in requestedPairs)
                {
                    if (existingSet.Contains((facilityId.fid, facilityId.level)))
                        continue;

                    _context.UserPermission.Add(new Domain.User.UserPermission
                    {
                        UserId = coordinatorUserId,
                        OrganisationUnitId = facilityId.fid,
                        PermissionLevel = facilityId.level
                    });
                }
            }
        }
    }
}
