using HyFive.Models.V1.Authentication;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HyFive.Services.Authentication.User
{
    public interface IUserService
    {
        Task<LoggedInUser> GetUser();

        bool IsUserLoggedIn();

        bool IsCoordinator(string hprNumber, string pseudonym);

        bool IsObserver(string hprNumber, string identityPseudonym);

        bool IsFhiAdminOrCoordinator(int institutionId);

        bool IsCoordinatorForDepartment(int departmentId);

        bool IsFhiAdminOrCoordinator(string pseudonym, string hprNumber);

        bool IsFhiAdmin(string identityPseudonym, string hprNumber);

        bool IsFhiAdmin();

        bool IsCoordinatorForDepartmentOrFhiAdmin(int departmentId);

        bool IsObserverForInstitution(int institutionId);

        bool IsCoordinatorForInstitutionOrFhiAdmin(int institutionId);

        bool IsCoordinatorForHealthcareProviderOrFhiAdmin(int healthcareProviderId);

        string GetHprNumber();

        bool IsCoordinatorForSession(string sessionId);

        int GetObserverIdForInstitution(int institutionId);

        string GetPseudonym();

        Expression<Func<TUser, bool>> HasHprOrPseudonymAndIsActive<TUser>(string hprnummer, string identPseudonym) where TUser : Domain.User.User;
    }
}
