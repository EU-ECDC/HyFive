using System;

namespace HyFive.Services.User
{
    public class UserValidator
    {
        public static bool HasNameAndEmail(Models.V1.User.User user)
        {
            return !string.IsNullOrEmpty(user.FirstName)
                   && !string.IsNullOrEmpty(user.LastName)
                   && (!string.IsNullOrEmpty(user.Email));
        }
    }
}
