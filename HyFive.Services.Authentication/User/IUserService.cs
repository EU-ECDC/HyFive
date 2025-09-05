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

        bool IsCoordinatorForInstitution(int institutionId);

        bool IsCoordinatorForInstitutionsOrAdmin(List<int> institutionIds);

        bool IsCoordinatorForDepartment(int departmentId);

        bool IsFhiAdminOrCoordinator(string email);

        bool IsFhiAdmin(string email);

        bool IsFhiAdmin();

        bool IsCoordinatorForDepartmentOrFhiAdmin(int departmentId);

        bool IsObserverForInstitution(int institutionId);

        bool IsCoordinatorForInstitutionOrFhiAdmin(int institutionId);

        bool IsCoordinatorForHealthcareProviderOrFhiAdmin(int healthcareProviderId);

        string GetHprNumber();
        string GetEmail();

        bool IsCoordinatorForSession(string sessionId);

        int GetObserverIdForInstitution(int institutionId);

        string GetPseudonym();

        Expression<Func<TUser, bool>> HasEmailAndIsActive<TUser>(string email) where TUser : Domain.User.User;
    }
}
