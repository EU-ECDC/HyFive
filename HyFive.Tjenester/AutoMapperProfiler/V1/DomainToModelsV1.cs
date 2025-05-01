using System;
using System.Linq;
using AutoMapper;
using HyFive.Domain.User;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Place;
using HyFive.Models.Session;
using HyFive.Models.V1.Institution;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Report.Beskyttelsesutstyr;
using HyFive.Models.V1.Report.FourIndications;
using HyFive.Models.V1.Report.HandJewelry;
using HyFive.Models.V1.Report.Glove;
using HyFive.Models.V1.Session;
using ProtectiveEquipmentSession = HyFive.Domain.Session.ProtectiveEquipmentSession;
using FourIndicationsSession = HyFive.Domain.Session.FourIndicationsSession;
using HandJewelrySession = HyFive.Domain.Session.HandJewelrySession;
using GloveSession = HyFive.Domain.Session.GloveSession;
using SessionType = HyFive.Models.V1.Session.SessionType;

namespace HyFive.Services.AutoMapperProfiler.V1
{
    public class DomainToModelsV1 : Profile
    {
        public DomainToModelsV1()
        {
            var norskTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            
            CreateMap<Domain.Place.Institution, Models.V1.Institution.Institution>(MemberList.None)
                .ForMember(dst => dst.Departments, opt => opt.MapFrom(i => i.Departments.ToList()));
            CreateMap<Domain.Place.InstitutionType, Models.V1.Institution.InstitutionType>(MemberList.None);
            CreateMap<Domain.User.User, Models.V1.User.User>(MemberList.None)
                .ForMember(dst => dst.InstitutionId, opt => opt.MapFrom(b => b.Institution != null ? b.Institution.Id : 0));
            CreateMap<Domain.User.UserAccessRequest, Models.V1.UserAccessRequest.UserAccessRequest>(MemberList.None);
            CreateMap<Domain.Place.Department, Models.V1.Institution.Department>(MemberList.None)
                .ForMember(dst => dst.DepartmentTypeId, opt => opt.MapFrom(o => o.DepartmentType != null ? o.DepartmentType.Id : 0))
                .ForMember(dst => dst.Roles, opt => opt.MapFrom(o => o.Roles.ToList()));
            CreateMap<Domain.Observation.Role, Models.V1.Observation.Role>(MemberList.None);
            CreateMap<Domain.Place.DepartmentType, Models.V1.Institution.DepartmentType>(MemberList.None);

            CreateMap<FourIndicationsSession, Models.V1.Session.FourIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.InstitutionsName, opt => opt.MapFrom(src => src.Observer.Institution.Name))
                .ForMember(dst => dst.InstitutionId, opt => opt.MapFrom(src => src.Observer.Institution.Id));
            CreateMap<FourIndicationsObservation, Models.V1.Observation.FourIndicatorsObservation>(MemberList
                .None);
            CreateMap<IndicationTypes, Models.V1.Observation.IndicationType>(MemberList.None);
            CreateMap<Activity, Models.V1.Observation.Activity>(MemberList.None);
            CreateMap<ActivityType, Models.V1.Observation.ActivityType>(MemberList.None);

            CreateMap<HandJewelrySession, Models.V1.Session.HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.InstitutionsName, opt => opt.MapFrom(src => src.Observer.Institution.Name))
                .ForMember(dst => dst.InstitutionId, opt => opt.MapFrom(src => src.Observer.Institution.Id));
            CreateMap<HandJewelryObservation, Models.V1.Observation.HandJewelryObservation>(MemberList.None);
            CreateMap<HandJewelryType, Models.V1.Observation.HandJewelryType>(MemberList.None);

            CreateMap<ProtectiveEquipmentSession, Models.V1.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.InstitutionsName, opt => opt.MapFrom(src => src.Observer.Institution.Name))
                .ForMember(dst => dst.InstitutionId, opt => opt.MapFrom(src => src.Observer.Institution.Id));
            CreateMap<ProtectiveEquipmentSession,
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(MemberList.None);
            CreateMap<Domain.Observation.ProtectiveEquipment.ProtectiveEquipment,
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment>(MemberList.None);
            CreateMap<ProtectiveEquipmentType, Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType>(
                MemberList.None);
            CreateMap<ProtectiveEquipmentSettingType,
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>(MemberList.None)
                .ForMember(dst => dst.EquipmentTypes, opt => opt.MapFrom(o => o.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes));
            CreateMap<MisuseType, Models.V1.Observation.ProtectiveEquipment.MisuseType>(MemberList.None);

            CreateMap<ProtectiveEquipmentSettingTypeProtectiveEquipmentType,
                    Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType>(MemberList.None)
                .ForMember(dst => dst.IsDefault, opt => opt.MapFrom(o => o.IsDefault))
                .ForMember(dst => dst.IsRequired, opt => opt.MapFrom(o => o.IsDefault))
                .ForMember(dst => dst.Code, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Code))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Name))
                .ForMember(dst => dst.IncorrectTypes, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.MisuseTypes));

            CreateMap<GloveSession, Models.V1.Session.GloveSession>(MemberList.None)
                .ForMember(dst => dst.Observations, opt => opt.MapFrom(src => src.Observations))
                .ForMember(dst => dst.InstitutionsName, opt => opt.MapFrom(src => src.Observer.Institution.Name))
                .ForMember(dst => dst.InstitutionId, opt => opt.MapFrom(src => src.Observer.Institution.Id));
            CreateMap<GloveObservation, Models.V1.Observation.Gloves.GloveObservation>(MemberList.None);
            CreateMap<GloveWithIndicationType, Models.V1.Observation.Gloves.IndicatedGloveType>(MemberList
                .None);
            CreateMap<GloveWithoutIndicationType, Models.V1.Observation.Gloves.GeneralPurposeGloveType>(MemberList
                .None);
            CreateMap<HandHygieneAfterGloveUseType, Models.V1.Observation.Gloves.PostGloveHandHygieneType>(
                MemberList.None);

            CreateMap<Domain.Place.Institution, Models.V1.Institution.InstitutionReport>(MemberList.None);

            CreateMap<Domain.Place.Institution, Models.V1.Overview.InstitutionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Departments.Sum(x => x.Sessions.Count)));
            CreateMap<Domain.Place.Department, Models.V1.Overview.DepartmentOverviewReport>(MemberList.None)
                .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Sessions.Count));

            CreateMap<Domain.Session.Session, SessionReport>(MemberList.None)
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.InstitutionName,
                    opt => opt.MapFrom(src => src.Observer.Institution.Name));

            CreateMap<FourIndicationsSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)));

            CreateMap<FourIndicationsObservation, ObservationOverviewReport>(MemberList.None);

            // HandJewelry
            CreateMap<HandJewelrySession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)));

            CreateMap<HandJewelryObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.HandJewelryTypes,
                    opt => opt.MapFrom(src => src.HandJewelry));

            // ProtectiveEquipment
            CreateMap<ProtectiveEquipmentSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)));

            CreateMap<ProtectiveEquipmentObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes, opt => opt.MapFrom(src => src.SettingType.Name))
                .ForMember(dest => dest.ProtectiveEquipment, opt => opt.MapFrom(src => src.ProtectiveEquipmentList))
                .ForMember(dest => dest.ProtectiveEquipmentObservation, opt => opt.MapFrom(src => src));
            

            CreateMap<Domain.Observation.ProtectiveEquipment.ProtectiveEquipment, ProtectiveEquipmentOverviewReport>(MemberList.None)
                .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.EquipmentType.Name))
                .ForMember(dest => dest.MisuseTypes, opt => opt.MapFrom(src => src.MisuseTypes.Select(ft => ft.Name)));

            // Glove
            CreateMap<GloveSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)));

            CreateMap<GloveObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.GloveObservation,
                    opt => opt.MapFrom(src => src));

            CreateMap<Domain.Session.TransferStatusType, TransferStatusType>(MemberList.None);

            CreateMap<Domain.Place.PredefinedComment, Models.V1.Institution.PredefinedComment>(MemberList.None);

            // Clinic
            CreateMap<Domain.Place.Clinic, Models.V1.Institution.Clinic>(MemberList.None)
                .ForMember(dest => dest.InstitutionId, opt => opt.MapFrom(src => src.Institution.Id));

            //Region
            CreateMap<Domain.Place.Region, Models.V1.Institution.Region>(MemberList.None);

            // Fire indikasjoner observasjon-rapport
            CreateMap<FourIndicationsObservation, FourIndicationsObservationReport>(MemberList.None)
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.FourIndicationsSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.FourIndicationsSession.Observer)))
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.FourIndicationsSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegistrationTime))
                .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.InstitutionType.Code))
                .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.InstitutionType.Code))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.Name))
                .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.Abbreviation))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Name))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.DepartmentType.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.FourIndicationsSession.TransmissionStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.FourIndicationsSession.Comment))
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(o => o.Activity.ActivityType.Name))
                .ForMember(dest => dest.Indications, opt => opt.MapFrom(o => string.Join(',', o.IndicationTypes.Select(i => i.Name))))
                .ForMember(dest => dest.SecondsUsed, opt => opt.MapFrom(o => o.Activity.TimeSpent))
                .ForMember(dest => dest.TimingWasPerformed, opt => opt.MapFrom(o => o.Activity.TimeRecordingWasDone))
                .ForMember(dest => dest.GlovesUsed, opt => opt.MapFrom(o => o.Activity.GloveUsed))
                .ForMember(dest => dest.HealthcareTrust, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.HealthcareOrganization.Name))
                .ForMember(dest => dest.RegionalHealthcareTrust, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.HealthcareOrganization.RegionaltHealthcareOrganization.Name))
                .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.Municipality.Number))
                .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institution.Municipality.Name));


            // Glove-rapport

            CreateMap<GloveObservation, GloveObservationReport>()
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.GloveSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.GloveSession.Observer)))
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.GloveSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegistrationTime))
                .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.InstitutionType.Code))
                .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.InstitutionType.Name))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.Name))
                .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.Abbreviation))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.GloveSession.Department.Name))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.GloveSession.Department.DepartmentType.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.GloveSession.TransmissionStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.GloveSession.Comment))
                .ForMember(dest => dest.HandHygieneAfterGloveUseCode, opt => opt.MapFrom(o => o.PostGloveHandHygieneType.Code))
                .ForMember(dest => dest.GlovesWithoutIndicationCode, opt => opt.MapFrom(o => string.Join(',',o.GeneralPurposeGloveTypes.Select(h => h.Code))))
                .ForMember(dest => dest.GlovesWithIndicationCode, opt => opt.MapFrom(o => string.Join(',', o.IndicatedGloveTypes.Select(h => h.Code))))
                .ForMember(dest => dest.HealthcareTrust, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.HealthcareOrganization.Name))
                .ForMember(dest => dest.RegionalHealthcareTrust, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.HealthcareOrganization.RegionaltHealthcareOrganization.Name))
                .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.Municipality.Number))
                .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.GloveSession.Department.Institution.Municipality.Name));

            // HandJewelry-rapport

            CreateMap<HandJewelryObservation, HandJewelryObservationReport>()
               .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.HandJewelrySession.Id))
               .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.HandJewelrySession.Observer)))
               .ForMember(dest => dest.SessionCreationTime, opt => opt.MapFrom(src => src.HandJewelrySession.CreatedDate))
               .ForMember(dest => dest.ObservationRegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime))
               .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.InstitutionType.Code))
               .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.InstitutionType.Name))
               .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.Name))
               .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.Abbreviation))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Name))
               .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.HandJewelrySession.Department.DepartmentType.Name))
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
               .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.HandJewelrySession.TransmissionStatus.Code))
               .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
               .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.HandJewelrySession.Comment))
               .ForMember(dest => dest.HandJewelryTypeCodes, opt => opt.MapFrom(o => string.Join(',', o.HandJewelry.Select(h => h.Code))))
               .ForMember(dest => dest.HandJewelryTypes, opt => opt.MapFrom(o => string.Join(',', o.HandJewelry.Select(h => h.Name))))
               .ForMember(dest => dest.HealthTrust, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.HealthcareOrganization.Name))
               .ForMember(dest => dest.RegionalHealthTrust, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.HealthcareOrganization.RegionaltHealthcareOrganization.Name))
               .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.Municipality.Number))
               .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institution.Municipality.Name));

            // ProtectiveEquipment-rapport
            CreateMap<HyFive.Domain.Observation.ProtectiveEquipment.ProtectiveEquipment, PPEObservationReport>()
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Observer)))
                .ForMember(dest => dest.CreateSessionTime, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.RegistrationTime))
                .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.InstitutionType.Code))
                .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.InstitutionType.Name))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.Name))
                .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.Abbreviation))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Name))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.DepartmentType.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.ProtectiveEquipmentObservation.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.TransmissionStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Comment))
                .ForMember(dest => dest.WasUsed, opt => opt.MapFrom(o => o.WasUsed))
                .ForMember(dest => dest.WasUsedCorrectly, opt => opt.MapFrom(o => o.WasUsedCorrectly))
                .ForMember(dest => dest.IsIndicated, opt => opt.MapFrom(o => o.IsRequired))
                .ForMember(dest => dest.ProtectiveEquipmentCode, opt => opt.MapFrom(o => o.EquipmentType.Code))
                .ForMember(dest => dest.ProtectiveEquipment, opt => opt.MapFrom(o => o.EquipmentType.Name))
                .ForMember(dest => dest.ProtectiveEquipmentSettingCode, opt => opt.MapFrom(o => o.ProtectiveEquipmentObservation.SettingType.Code))
                .ForMember(dest => dest.ProtectiveEquipmentSetting, opt => opt.MapFrom(o => o.ProtectiveEquipmentObservation.SettingType.Name))
                .ForMember(dest => dest.Misuse, opt => opt.MapFrom(o => string.Join(',',o.MisuseTypes.Select(f => f.Name))))
                .ForMember(dest => dest.HealthTrust, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.HealthcareOrganization.Name))
                .ForMember(dest => dest.RegionalHealthTrust, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.HealthcareOrganization.RegionaltHealthcareOrganization.Name))
                .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.Municipality.Number))
                .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Institution.Municipality.Name));

            CreateMap<Domain.Place.HealthcareOrganization, Models.V1.Institution.HealthcareOrganization>()
                .ForMember(dest => dest.RegionaltHealthcareOrganizationId, opt => opt.MapFrom(src => src.RegionaltHealthcareOrganization != null ? src.RegionaltHealthcareOrganization.Id : 0));

            CreateMap<Domain.Place.RegionaltHealthcareProvider, Models.V1.Institution.RegionalInstitution>(MemberList.None);

            CreateMap<Domain.Place.Municipality, Models.V1.Institution.Comment>(MemberList.None);
        }

        private static string GetObserverName(Observer observer)
        {
            return string.Join(" ", new[] { observer.FirstName, observer.LastName });
        }
    }
}