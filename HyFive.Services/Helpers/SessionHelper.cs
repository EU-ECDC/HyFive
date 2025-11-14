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
        public static async Task<Domain.Place.Department> GetDepartmentAsync(
            HandHygieneContext context, int departmentId, CancellationToken cancellationToken)
        {
            return await context.Department
                .Include(d => d.Roles)
                .FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);
        }

        public static async Task<ObserverUser> GetObserverAsync(
            HandHygieneContext context, IUserService userService, string email, int facilityId, CancellationToken cancellationToken)
        {
            var facility = await context.Facility
                .Include(f => f.Users)
                .FirstOrDefaultAsync(f => f.Id == facilityId, cancellationToken);

            if (facility == null)
                throw new ArgumentException($"Did not find the specified facility with ID: {facilityId}");

            return facility.Users
                .FirstOrDefault(userService.HasEmailAndIsActive<ObserverUser>(email).Compile());
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
