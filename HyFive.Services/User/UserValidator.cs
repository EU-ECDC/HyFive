using HyFive.Models.V1.User;
using Microsoft.IdentityModel.Tokens;
using System;

namespace HyFive.Services.User
{
    public class UserValidator
    {
        public static bool HasNameAndEmail(CreateUpdateUserRequest request)
        {
            return !string.IsNullOrEmpty(request.FirstName)
                   && !string.IsNullOrEmpty(request.LastName)
                   && (!string.IsNullOrEmpty(request.Email));
        }
    }
}
