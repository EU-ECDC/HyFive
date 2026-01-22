using HyFive.Models.V1.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public static class CoordinatorForCityValidator
    {
        public static bool CanBeUpdated(
            CityCoordinator coordinator,
            out string errorCode,
            out object[] args)
        {
            errorCode = string.Empty;
            args = Array.Empty<object>();

            if (string.IsNullOrWhiteSpace(coordinator.FirstName))
            {
                errorCode = "FirstNameRequired";
                return false;
            }

            if (string.IsNullOrWhiteSpace(coordinator.LastName))
            {
                errorCode = "LastNameRequired";
                return false;
            }

            if (string.IsNullOrWhiteSpace(coordinator.Email))
            {
                errorCode = "EmailRequired";
                return false;
            }

            return true;
        }
    }
}
