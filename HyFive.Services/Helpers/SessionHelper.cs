using HyFive.DataAccess;
using HyFive.Services.Authentication.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.Helpers
{
    public static class SessionHelper
    {
        public static async Task<Domain.Place.OrganisationUnit> GetOrganisationUnitAsync(
            HandHygieneContext context, int unitId, CancellationToken cancellationToken)
        {
                return await context.OrganisationUnit
                    .Include(ou => ou.LevelRef)
                    .Include(ou => ou.Parent)
                        .ThenInclude(parent => parent.OrganisationUnitRoles)
                            .ThenInclude(our => our.Role)
                    .Include(ou => ou.Parent)
                        .ThenInclude(parent => parent.Parent)
                    .Include(ou => ou.OrganisationUnitRoles)
                        .ThenInclude(our => our.Role)
                    .FirstOrDefaultAsync(ou => ou.Id == unitId, cancellationToken);
        }

        public static async Task<ObserverUser> GetObserverAsync(
            HandHygieneContext context, IUserService userService, string email, int organisationUnitId, CancellationToken cancellationToken)
        {
            var ou = await context.OrganisationUnit
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == organisationUnitId, cancellationToken);

            if (ou == null)
                throw new ArgumentException($"Did not find the specified organisation unit with ID: {organisationUnitId}");

            // Find observer user (active)
            var observer = await context.User
                .OfType<ObserverUser>()
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeactivated, cancellationToken);

            if (observer == null)
                return null;

            // Check permissions: allow either direct OU or facility(parent) OU
            var facilityId = ou.ParentId;

            var hasAccess = await context.UserPermission.AnyAsync(p =>
                p.UserId == observer.Id &&
                (p.OrganisationUnitId == ou.Id || (facilityId != null && p.OrganisationUnitId == facilityId.Value)),
                cancellationToken);

            return hasAccess ? observer : null;
        }

        public static async Task<Domain.Session.TransferStatusType> GetDefaultTransferStatusAsync(
            HandHygieneContext context, string code, CancellationToken cancellationToken)
        {
            return await context.TransferStatusType
                .FirstOrDefaultAsync(t => t.Code == code, cancellationToken)
                ?? throw new ArgumentException($"Did not find transfer status with code '{code}'");
        }
    }
}
