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

        public static bool IsValidIdentityPseudonym(string pseudonym)
        {
            return IsBase64String(pseudonym) && pseudonym?.Length == 44;
        }

        public static bool IsBase64String(string base64)
        {
            var buffer = new Span<byte>(new byte[base64.Length]);
            var isBase64String = Convert.TryFromBase64String(base64, buffer, out var converted);
            return isBase64String;
        }
    }
}
