using HyFive.Models.V1.Authentication;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HyFive.Services.Authentication.User
{
    public interface IUserService
    {
        Task<LoggedInUser> GetUser();

        bool IsUserLoggedIn();

        bool IsCoordinator(string email);

        bool IsObserver(string email);

        bool IsCoordinatorForFacility(int facilityId);

        bool IsCoordinatorForFacilitiesOrAdmin(List<int> facilityIds);

        bool IsCoordinatorForDepartment(int departmentId);

        bool IsAdminOrCoordinator(string email);

        bool IsAdmin(string email);

        bool IsAdmin();

        bool IsCoordinatorForDepartmentOrAdmin(int departmentId);

        bool IsObserverForFacility(int facilityId);

        bool IsCoordinatorForFacilityOrAdmin(int facilityId);

        bool IsCoordinatorForCityOrAdmin(int cityId);

        string GetHprNumber();
        string GetEmail();

        bool IsCoordinatorForSession(string sessionId);

        int GetObserverIdForFacility(int facilityId);

        string GetPseudonym();

        Expression<Func<TUser, bool>> HasEmailAndIsActive<TUser>(string email) where TUser : Domain.User.User;
    }
}
