using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Services.City
{
    public static class CoordinatorForCityHelper
    {
        public static Coordinator GetCoordinator(
            HandHygieneContext context, int facilityId, string email)
        {
            return context.Coordinator.FirstOrDefault(k =>
                k.Facility.Id == facilityId &&
                !string.IsNullOrEmpty(k.Email) &&
                k.Email == email);
        }

        public static Coordinator CreateCoordinatorForFacility(
            HandHygieneContext context, CityCoordinator coordinator, int facilityId)
        {
            var facility = context.Facility.FirstOrDefault(i => i.Id == facilityId);

            return new Coordinator
            {
                FirstName = coordinator.FirstName,
                LastName = coordinator.LastName,
                Email = coordinator.Email,
                HPRNumber = coordinator.HPRNumber,
                IdentityPseudonym = coordinator.IdentityPseudonym,
                Facility = facility
            };
        }
    }
}
