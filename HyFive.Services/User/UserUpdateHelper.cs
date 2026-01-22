using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
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
        public static async Task UpdateUserBaseFields<TEntity>(
            HandHygieneContext context,
            Models.V1.User.User model,
            CancellationToken cancellationToken
        ) where TEntity : Domain.User.User
        {
            if (!UserValidator.HasNameAndEmail(model))
                throw new ValidationException("ObserverMissingDetails");

            // Load entity
            var user = await context.User.OfType<TEntity>()
                .FirstOrDefaultAsync(u => u.Id == model.Id, cancellationToken);

            if (user == null)
                throw new DomainException("UserNotFound", model.Id);

            // Update fields common to all user types
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.IsDeactivated = model.IsDisabled;

            context.User.Update(user);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
