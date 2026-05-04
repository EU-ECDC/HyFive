using HyFive.Models.V1.Authentication;
using HyFive.Models.V1.Constants;
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

        Task<bool> IsCoordinator(string email);

        Task<bool> IsObserver(string email);

        Task<bool> IsCoordinatorForFacility(int facilityOrgUnitId);

        Task<bool> IsCoordinatorForFacilitiesOrAdmin(List<int> facilityOrgUnitIds);

        Task<bool> IsCoordinatorForDepartment(int departmentOrgUnitId);
        Task<bool> IsCoordinatorForUnit(int unitOrgUnitId);

        Task<bool> IsAdminOrCoordinator(string email);

        Task<bool> IsAdmin(string email);

        Task<bool> IsAdmin();

        Task<bool> IsCoordinatorForDepartmentOrAdmin(int departmentOrgUnitId);

        Task<bool> IsObserverForFacility(int facilityOrgUnitId);

        Task<bool> IsCoordinatorForFacilityOrAdmin(int facilityOrgUnitId);

        Task<bool> IsCoordinatorForCityOrAdmin(string city);

        string GetEmail();

        Task<bool> IsCoordinatorForSession(string sessionId);

        Task<int> GetObserverIdIfHasAccessToFacility(int facilityOrgUnitId);

        string GetPseudonym();

        Expression<Func<Domain.User.User, bool>> HasEmailAndIsActive(string email);
    }
}
