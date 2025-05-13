using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Authentication;
using Fhi.HelseId.Common.Identity;
using Fhi.HelseId.Web.ExtensionMethods;
using Fhi.HelseId.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Observer = HyFive.Domain.User.Observer;

namespace HyFive.Services.Authentication.User
{
    public class HealthIdUserService : IUserService
    {
        private const string HashSalt = "handhygiene";
        private const string FirstNameInClaims = "given_name";
        private const string LastNameInClaims = "family_name";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HealthIdUserService> _logger;
        private readonly HandHygieneContext _context;
        private ICurrentUser User { get; }



        public HealthIdUserService(ICurrentUser currentUser,
            IHttpContextAccessor httpContextAccessor,
            ILogger<HealthIdUserService> logger,
            HandHygieneContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _context = context;
            User = currentUser;
            if (User == null)
                _logger.LogInformation("TI03: User is null");
        }

        public async Task<LoggedInUser> GetUser()
        {
            var user = new LoggedInUser()
            {
                Name = User?.Name ?? "",
            };
            var logInName = User?.Name.ObfuscateName() ?? "(UserNull)";
            var hrpNumber = GetHprNumber();
            var pseudonym = GetPseudonym();

            user.Id = CreateHash(pseudonym + user.Name + HashSalt);
            user.IsObserver = IsObserver(hrpNumber, pseudonym);
            user.IsCoordinator = IsCoordinator(hrpNumber, pseudonym);
            user.IsFhiAdmin = IsFhiAdmin(pseudonym, hrpNumber);
            user.HPRNumber = hrpNumber;
            user.IdentityPseudonym = pseudonym;
            user.InstitutionIds = await _context.User.AsNoTracking().Include(k => k.Institution)
                .Where(HasHprOrPseudonymAndIsActive<Domain.User.User>(hrpNumber, pseudonym))
                .Where(k => k.Institution != null)
                .Select(k => k.Institution.Id)
                .ToListAsync();
            user.FirstName = GetFirstName();
            user.LastName = GetLastName();

            if (string.IsNullOrEmpty(User?.HprNummer))
            {
                _logger.LogInformation("TI02: User: {logInName} lacks hprNumber (i HealthId)", logInName);
            }
            return user;
        }

        public bool IsUserLoggedIn()
        {
            return this.User != null && (!string.IsNullOrEmpty(this.User.HprNummer) || !string.IsNullOrEmpty(this.User.PidPseudonym));
        }


        public bool IsCoordinator(string hrpNumber, string pseudonym)
            => IsRole<Coordinator>(hrpNumber, pseudonym);

        public bool IsObserver(string hrpNumber, string identPseudonym)
            => IsRole<Coordinator>(hrpNumber, identPseudonym);

        public bool IsCoordinatorForInstitution(int institutionId, string identPseudonym, string hrpNumber)
            => IsRoleForInstitution<Coordinator>(hrpNumber, identPseudonym, institutionId);

        public bool IsCoordinatorForInstitution(int institutionId)
            => IsRoleForInstitution<Coordinator>(GetHprNumber(), GetPseudonym(), institutionId);

        public bool IsCoordinatorForHealthcareProvider(int healthcareProvider)
            => IsCoordinatorForHealthcareProvider(GetHprNumber(), GetPseudonym(), healthcareProvider);

        public bool IsCoordinatorForInstitutions(int[] institutionIds)
            => IsRoleForInstitutions<Coordinator>(institutionIds);

        public bool IsCoordinatorForDepartment(int departmentId)
            => IsRoleForDepartment<Coordinator>(departmentId);

        public bool IsCoordinatorForUser(int userId)
            => IsRoleForUser<Coordinator>(GetHprNumber(), GetPseudonym(), userId);

        public bool IsObserverForInstitution(string hrpNumber, string identPseudonym, int institutionId)
            => IsRoleForInstitution<Observer>(hrpNumber, identPseudonym, institutionId);

        public bool IsFhiAdminOrCoordinator(string pseudonym, string hrpNumber)
        {
            return IsFhiAdmin(pseudonym, hrpNumber) || IsCoordinator(hrpNumber, pseudonym);
        }

        public bool IsFhiAdmin(string identPseudonym, string hrpNumber)
        {
            if (string.IsNullOrEmpty(hrpNumber) && string.IsNullOrEmpty(identPseudonym))
                return false;

            bool erFhiAdmin = _context.User.AsNoTracking().OfType<FhiAdmin>().AsNoTracking()
                    .Where(HasHprOrPseudonymAndIsActive<FhiAdmin>(hrpNumber, identPseudonym)).Any();
            return erFhiAdmin;
        }

        public bool IsFhiAdmin()
            => IsFhiAdmin(GetPseudonym(), GetHprNumber());

        public bool IsCoordinatorForDepartmentOrFhiAdmin(int avdelingId)
        {
            return IsCoordinatorForDepartment(avdelingId) || IsFhiAdmin();
        }

        public bool IsObserverForInstitution(int institutionId)
        {
            return IsObserverForInstitution(GetHprNumber(), GetPseudonym(), institutionId);
        }

        public bool IsCoordinatorForInstitutionOrFhiAdmin(int institutionId)
        {
            return IsCoordinatorForInstitution(institutionId) || IsFhiAdmin();
        }

        public bool IsCoordinatorForHealthcareProviderOrFhiAdmin(int healthcareProviderId)
        {
            return IsCoordinatorForHealthcareProvider(healthcareProviderId) || IsFhiAdmin();
        }

        public string GetHprNumber()
        {
            return _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimsPrincipalExtensions.HprNummer)?.Value;
        }

        public bool IsCoordinatorForSession(string sessionId)
        {
            var isGuid = Guid.TryParse(sessionId, out Guid guidSessionId);

            if (!isGuid)
            {
                throw new ArgumentException($"HealthIdUSerService: Error parsing ID: {sessionId}. {sessionId} must be of type Guid");
            }
            
            var institutionId = _context.Session.AsNoTracking().Include(s => s.Department).ThenInclude(a => a.Institution)
                .FirstOrDefault(s => s.Id == guidSessionId).Department?.InstitutionId;
            if (institutionId != null)
            {
                return IsRoleForInstitution<Coordinator>(GetHprNumber(), GetPseudonym(), (int)institutionId);
            }

            return false;
        }

        public int GetObserverIdForInstitution(int institutionId)
        {
            var hrpNumber = GetHprNumber();
            var pseudonym = GetPseudonym();
            return _context.User.AsNoTracking()
                .OfType<Observer>()
                .Include(o => o.Institution)
                .Where(HasHprOrPseudonymAndIsActive<Domain.User.Observer>(hrpNumber, pseudonym)).First(o => o.Institution.Id == institutionId)?.Id ?? 0;
        }

        public Expression<Func<TUser, bool>> HasHprOrPseudonymAndIsActive<TUser>(string hrpNumber, string identPseudonym) where TUser : Domain.User.User
        {
            return b => ((!string.IsNullOrEmpty(hrpNumber) && b.HPRNumber == hrpNumber) || (!string.IsNullOrEmpty(b.IdentityPseudonym) &&  b.IdentityPseudonym == identPseudonym)) && b.IsDeactivated == false;
        }

        private bool IsRole<TRole>(string hrpNumber, string pseudonym) where TRole : Domain.User.User
        {
            var erRolle = _context.User.AsNoTracking().OfType<TRole>()
                .Include(r => r.Institution)
                .Any(HasHprOrPseudonymAndIsActive<TRole>(hrpNumber, pseudonym));

            if (erRolle)
            {
                // Update all users with an IdentityPseudonym if they don't have one.
                UpdateUserWithPseudonym<TRole>(hrpNumber).GetAwaiter().GetResult();
            }
            return erRolle;
        }

        private bool IsRoleForInstitution<TRole>(string hrpNumber, string identPseudonym, int institutionId) where TRole : Domain.User.User
        {
            return _context.User.OfType<TRole>().AsNoTracking().Include(b => b.Institution)
                .Where(HasHprOrPseudonymAndIsActive<TRole>(hrpNumber, identPseudonym))
                .Any(b => b.Institution.Id == institutionId && b.Discriminator == GetDiscriminator<TRole>());
        }

        private bool IsCoordinatorForHealthcareProvider(string hrpNumber, string identPseudonym, int healthcareOrganization)
        {
            return _context.User.OfType<Coordinator>().AsNoTracking().Include(b => b.Institution).ThenInclude(i=>i.HealthcareOrganization)
                .Where(HasHprOrPseudonymAndIsActive<Coordinator>(hrpNumber, identPseudonym))
                .Any(b => b.Institution.HealthcareOrganization.Id == healthcareOrganization);
        }

        private bool IsRoleForInstitutions<TRole>(int[] institutionIds) where TRole : Domain.User.User
        {
            var hrpNumber = GetHprNumber();
            var pseudonym = GetPseudonym();
            var CoordinatorForInstitutionIds = _context
                .User
                .OfType<TRole>()
                .AsNoTracking()
                .Include(k => k.Institution)
                .Where(HasHprOrPseudonymAndIsActive<TRole>(hrpNumber, pseudonym))
                .Where(k => k.Discriminator == GetDiscriminator<TRole>())
                .Select(k => k.Institution.Id).ToList();

            if (institutionIds == null)
            {
                return CoordinatorForInstitutionIds.Any();
            }

            return CoordinatorForInstitutionIds.Any() && institutionIds.ToList().TrueForAll(iid => CoordinatorForInstitutionIds.Contains(iid));
        }

        private bool IsRoleForUser<TRole>(string hrpNumber, string pseudonym, int userId) where TRole : Domain.User.User
        {
            var institutionId = _context
                .User
                .AsNoTracking()
                .Include(b => b.Institution)
                .Where(b => b.Id == userId)
                .Select(b => b.Institution.Id).First();
            return IsRoleForInstitution<TRole>(hrpNumber, pseudonym, institutionId);
        }

        private bool IsRoleForSession<TRole>(string hrpNumber, string pseudonym, Guid sesjonId) where TRole : Domain.User.User
        {
            var institutionId = _context.Session
                .AsNoTracking()
                .Include(s => s.Department)
                .ThenInclude(a => a.Institution)
                .Where(s => s.Id == sesjonId)
                .Select(b => b.Department.Institution.Id).First();
            return IsRoleForInstitution<TRole>(hrpNumber, pseudonym, institutionId);
        }

        private bool IsRoleForDepartment<TRole>(int departmentId) where TRole : Domain.User.User
        {
            var institutionId = _context.Department.Include(a => a.Institution).First(a => a.Id == departmentId)
                .InstitutionId;

            return IsRoleForInstitution<TRole>(GetHprNumber(), GetPseudonym(), institutionId);
        }

        /// <summary>
        /// Stjålet fra https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha512?view=net-5.0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string CreateHash(string str)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }


        public string GetPseudonym()
        {
            var pseudonym = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == IdentityClaims.PidPseudonym)?.Value;
            return pseudonym;
        }

        private string GetDiscriminator<T>() where T : class
            => typeof(T).Name;

        private static Expression<Func<Domain.User.User, bool>> UserWithHprNumberWithoutIdentityPseudonym(string hrpNumber)
            => b => (b.IdentityPseudonym == null || b.IdentityPseudonym == "") && b.HPRNumber == hrpNumber;

        private async Task UpdateUserWithPseudonym<TRole>(string hrpNumber) where TRole : Domain.User.User
        {
            try
            {
                var userIdsToBeUpdated = _context
                    .User
                    .OfType<TRole>()
                    .AsNoTracking()
                    .Where(UserWithHprNumberWithoutIdentityPseudonym(hrpNumber))
                    .ToList();

                if (userIdsToBeUpdated.Any())
                {
                    _logger.LogInformation(
                        $"TI01: {nameof(UpdateUserWithPseudonym)}: Updating users! IDs: {string.Join(',', userIdsToBeUpdated.Select(b => b.Id))}");

                    var pseudonym = GetPseudonym();
                    foreach (var user in userIdsToBeUpdated)
                    {
                        user.IdentityPseudonym = pseudonym;
                        _context.Entry(user).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "TE01:Error while updating user who hadn't set Pseudonym");
            }
            
        }

        public string GetFirstName()
        {
            var fornavn = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == FirstNameInClaims)?.Value;
            return fornavn;
        }

        public string GetLastName()
        {
            var fornavn = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == LastNameInClaims)?.Value;
            return fornavn;
        }
    }
}
