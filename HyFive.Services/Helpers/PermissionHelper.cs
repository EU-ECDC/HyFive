using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Helpers
{
    public static class PermissionHelper
    {
        public static IQueryable<Domain.User.User> WithPermission(
            this IQueryable<Domain.User.User> query,
            string permission)
        {
            return query.Where(u =>
                u.UserPermissions.Any(p => p.PermissionLevel == permission));
        }
        public static async Task<bool> HasObserverPermissionForFacilityAsync(
            HandHygieneContext context,
            int userId,
            int facilityId,
            CancellationToken cancellationToken)
        {
            return await context.UserPermission
                .AsNoTracking()
                .AnyAsync(p =>
                    p.UserId == userId &&
                    p.PermissionLevel == PermissionLevelConstants.Observer &&
                    p.OrganisationUnitId == facilityId,
                    cancellationToken);
        }

        public static async Task<int?> GetFacilityIdForUnitAsync(
           HandHygieneContext context,
           int unitId,
           CancellationToken cancellationToken)
        {
            return await context.OrganisationUnit
                .AsNoTracking()
                .Where(u => u.Id == unitId)
                .Select(u => u.Parent.Parent.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
