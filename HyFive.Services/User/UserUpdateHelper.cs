using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.User
{
    public static class UserUpdateHelper
    {
        public static async Task<Domain.User.User> UpdateUserBaseFields(
            HandHygieneContext context,
            CreateUpdateUserRequest request,
            string requiredPermissionLevel,
            CancellationToken cancellationToken
            )
        {
            if (!UserValidator.HasNameAndEmail(request))
                throw new ValidationException("ObserverMissingDetails");

            var user = await context.User
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(
                    u => u.Id == request.Id &&
                         u.UserPermissions.Any(p => p.PermissionLevel == requiredPermissionLevel),
                    cancellationToken);

            if (user == null)
                throw new DomainException("UserNotFound", request.Id);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.IsDeactivated = request.IsDeactivated;

            return user;
        }
    }
}
