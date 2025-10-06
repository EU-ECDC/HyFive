using System;
using System.Linq;
using AutoMapper;
using ObserverUser = HyFive.Domain.User.User;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Place;
using HyFive.Models.Session;
using HyFive.Models.V1.Facility;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Report.Beskyttelsesutstyr;
using HyFive.Models.V1.Report.FiveIndications;
using HyFive.Models.V1.Report.HandJewelry;
using HyFive.Models.V1.Report.Glove;
using HyFive.Models.V1.Session;
using ProtectiveEquipmentSession = HyFive.Domain.Session.ProtectiveEquipmentSession;
using FiveIndicationsSession = HyFive.Domain.Session.FiveIndicationsSession;
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
            
            CreateMap<Domain.Place.Facility, Models.V1.Facility.Facility>(MemberList.None)
                .ForMember(dst => dst.Departments, opt => opt.MapFrom(i => i.Departments.ToList()));
            CreateMap<Domain.Place.FacilityType, Models.V1.Facility.FacilityType>(MemberList.None);
            CreateMap<Domain.User.User, Models.V1.User.User>(MemberList.None)
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(b => b.Facility != null ? b.Facility.Id : 0))
                .ForMember(dst => dst.IsDisabled, opt => opt.MapFrom(src => src.IsDeactivated));
            CreateMap<Domain.Place.Department, Models.V1.Facility.Department>(MemberList.None)
                .ForMember(dst => dst.DepartmentTypeId, opt => opt.MapFrom(o => o.DepartmentType != null ? o.DepartmentType.Id : 0))
                .ForMember(dst => dst.Roles, opt => opt.MapFrom(o => o.Roles.ToList()));
            CreateMap<Domain.Observation.Role, Models.V1.Observation.Role>(MemberList.None);
            CreateMap<Domain.Place.DepartmentType, Models.V1.Facility.DepartmentType>(MemberList.None);

            CreateMap<FiveIndicationsSession, Models.V1.Session.FiveIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.FacilityName, opt => opt.MapFrom(src => src.Observer.Facility.Name))
                .ForMember(dst => dst.CreatedDate, opt => opt.MapFrom(src => src.Observer.Facility.CreatedTime))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => src.Observer.Facility.Id));
            CreateMap<FiveIndicationsObservation, Models.V1.Observation.FiveIndicatorsObservation>(MemberList
                .None);
            CreateMap<IndicationTypes, Models.V1.Observation.IndicationType>(MemberList.None);
            CreateMap<Activity, Models.V1.Observation.Activity>(MemberList.None);
            CreateMap<ActivityType, Models.V1.Observation.ActivityType>(MemberList.None);

            CreateMap<HandJewelrySession, Models.V1.Session.HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.FacilityName, opt => opt.MapFrom(src => src.Observer.Facility.Name))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => src.Observer.Facility.Id));
            CreateMap<HandJewelryObservation, Models.V1.Observation.HandJewelryObservation>(MemberList.None);
            CreateMap<HandJewelryType, Models.V1.Observation.HandJewelryType>(MemberList.None);

            CreateMap<ProtectiveEquipmentSession, Models.V1.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.FacilityName, opt => opt.MapFrom(src => src.Observer.Facility.Name))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => src.Observer.Facility.Id));

            CreateMap<ProtectiveEquipmentObservation, Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(MemberList.None)
            .ForMember(dst => dst.ProtectiveEquipmentList, opt => opt.MapFrom(src => src.ProtectiveEquipmentList))
            .ForMember(dst => dst.SettingType, opt => opt.MapFrom(src => src.SettingType))
            .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => src.Comment));
            
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
                .ForMember(dst => dst.MisuseTypes, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.MisuseTypes));

            CreateMap<GloveSession, Models.V1.Session.GloveSession>(MemberList.None)
                .ForMember(dst => dst.Observations, opt => opt.MapFrom(src => src.Observations))
                .ForMember(dst => dst.FacilityName, opt => opt.MapFrom(src => src.Observer.Facility.Name))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => src.Observer.Facility.Id));
            CreateMap<GloveObservation, Models.V1.Observation.Gloves.GloveObservation>(MemberList.None);
            CreateMap<GloveWithIndicationType, Models.V1.Observation.Gloves.GloveWithIndicationType>(MemberList
                .None);
            CreateMap<GloveWithoutIndicationType, Models.V1.Observation.Gloves.GloveWithoutIndicationType>(MemberList
                .None);
            CreateMap<HandHygieneAfterGloveUseType, Models.V1.Observation.Gloves.PostGloveHandHygieneType>(
                MemberList.None);

            CreateMap<Domain.Place.Facility, Models.V1.Facility.FacilityReport>(MemberList.None);

            CreateMap<Domain.Place.Facility, Models.V1.Overview.FacilityOverviewReport>(MemberList.None)
                .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Departments.Sum(x => x.Sessions.Count)));
            CreateMap<Domain.Place.Department, Models.V1.Overview.DepartmentOverviewReport>(MemberList.None)
                .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Sessions.Count));

            CreateMap<Domain.Session.Session, SessionReport>(MemberList.None)
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.FacilityName,
                    opt => opt.MapFrom(src => src.Observer.Facility.Name));

            CreateMap<FiveIndicationsSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)));

            CreateMap<FiveIndicationsObservation, ObservationOverviewReport>(MemberList.None);

            // HandJewelries
            CreateMap<HandJewelrySession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)));

            CreateMap<HandJewelryObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.HandJewelryTypes,
                    opt => opt.MapFrom(src => src.HandJewelries));

            // ProtectiveEquipment
            CreateMap<ProtectiveEquipmentSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)));

            CreateMap<ProtectiveEquipmentObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes, opt => opt.MapFrom(src => src.SettingType.Name))
                .ForMember(dest => dest.ProtectiveEquipment, opt => opt.MapFrom(src => src.ProtectiveEquipmentList));
                //.ForMember(dest => dest.ProtectiveEquipmentObservation, opt => opt.MapFrom(src => src));
            

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

            CreateMap<Domain.Place.PredefinedComment, Models.V1.Facility.PredefinedComment>(MemberList.None);

            // Clinic
            CreateMap<Domain.Place.Clinic, Models.V1.Facility.Clinic>(MemberList.None)
                .ForMember(dest => dest.FacilityId, opt => opt.MapFrom(src => src.Facility.Id));

            // Five Indications obsrvation-report
            CreateMap<FiveIndicationsObservation, FiveIndicationsObservationReport>(MemberList.None)
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.FiveIndicationsSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.FiveIndicationsSession.Observer)))
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.FiveIndicationsSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime))
                .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => src.FiveIndicationsSession.Department.Facility.FacilityType.Code))
                .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.FiveIndicationsSession.Department.Facility.FacilityType.Code))
                .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => src.FiveIndicationsSession.Department.Facility.Name))
                .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => src.FiveIndicationsSession.Department.Facility.Abbreviation))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.FiveIndicationsSession.Department.Name))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.FiveIndicationsSession.Department.DepartmentType.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.FiveIndicationsSession.TransferStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.FiveIndicationsSession.Comment))
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(o => o.Activity.ActivityType.Name))
                .ForMember(dest => dest.Indications, opt => opt.MapFrom(o => string.Join(',', o.IndicationTypes.Select(i => i.Name))))
                .ForMember(dest => dest.SecondsUsed, opt => opt.MapFrom(o => o.Activity.SecondsUsed))
                .ForMember(dest => dest.TimingWasPerformed, opt => opt.MapFrom(o => o.Activity.TimingWasPerformed))
                .ForMember(dest => dest.GlovesUsed, opt => opt.MapFrom(o => o.Activity.GlovesUsed))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.FiveIndicationsSession.Department.Facility.City.Name));


            // Glove-report

            CreateMap<GloveObservation, GloveObservationReport>()
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.GloveSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.GloveSession.Observer)))
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.GloveSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime))
                .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => src.GloveSession.Department.Facility.FacilityType.Code))
                .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.GloveSession.Department.Facility.FacilityType.Name))
                .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => src.GloveSession.Department.Facility.Name))
                .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => src.GloveSession.Department.Facility.Abbreviation))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.GloveSession.Department.Name))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.GloveSession.Department.DepartmentType.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.GloveSession.TransferStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.GloveSession.Comment))
                .ForMember(dest => dest.HandHygieneAfterGloveUseCode, opt => opt.MapFrom(o => o.PostGloveHandHygieneType.Code))
                .ForMember(dest => dest.GlovesWithoutIndicationCode, opt => opt.MapFrom(o => string.Join(',',o.GloveWithoutIndicationTypes.Select(h => h.Code))))
                .ForMember(dest => dest.GlovesWithIndicationCode, opt => opt.MapFrom(o => string.Join(',', o.GloveWithIndicationTypes.Select(h => h.Code))))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.GloveSession.Department.Facility.City.Name));

            // HandJewelries-report

            CreateMap<HandJewelryObservation, HandJewelryObservationReport>()
               .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.HandJewelrySession.Id))
               .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.HandJewelrySession.Observer)))
               .ForMember(dest => dest.SessionCreationTime, opt => opt.MapFrom(src => src.HandJewelrySession.CreatedDate))
               .ForMember(dest => dest.ObservationRegistrationTime, opt => opt.MapFrom(src => src.RegisteredTime))
               .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Facility.FacilityType.Code))
               .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Facility.FacilityType.Name))
               .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Facility.Name))
               .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Facility.Abbreviation))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Name))
               .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.HandJewelrySession.Department.DepartmentType.Name))
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
               .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.HandJewelrySession.TransferStatus.Code))
               .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
               .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.HandJewelrySession.Comment))
               .ForMember(dest => dest.HandJewelryTypeCodes, opt => opt.MapFrom(o => string.Join(',', o.HandJewelries.Select(h => h.Code))))
               .ForMember(dest => dest.HandJewelryTypes, opt => opt.MapFrom(o => string.Join(',', o.HandJewelries.Select(h => h.Name))))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Facility.City.Name));

            // ProtectiveEquipment-report
            CreateMap<HyFive.Domain.Observation.ProtectiveEquipment.ProtectiveEquipment, PPEObservationReport>()
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Observer)))
                .ForMember(dest => dest.CreateSessionTime, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.RegisteredTime))
                .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Facility.FacilityType.Code))
                .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Facility.FacilityType.Name))
                .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Facility.Name))
                .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Facility.Abbreviation))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Name))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.DepartmentType.Name))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.ProtectiveEquipmentObservation.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.TransferStatus.Code))
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
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Department.Facility.City.Name));

            CreateMap<Domain.Place.City, Models.V1.Facility.City>(MemberList.None);
                

        
        }

        private static string GetObserverName(ObserverUser observer)
        {
            return string.Join(" ", new[] { observer.FirstName, observer.LastName });
        }
    }
}