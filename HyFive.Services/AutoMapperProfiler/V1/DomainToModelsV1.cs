using AutoMapper;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Models.Session;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Report.FiveIndications;
using HyFive.Models.V1.Report.Glove;
using HyFive.Models.V1.Report.HandJewelry;
using HyFive.Models.V1.Report.ProtectiveEquipment;
using HyFive.Models.V1.Session;
using System;
using System.Linq;
using FiveIndicationsSession = HyFive.Domain.Session.FiveIndicationsSession;
using GloveSession = HyFive.Domain.Session.GloveSession;
using HandJewelrySession = HyFive.Domain.Session.HandJewelrySession;
using ObserverUser = HyFive.Domain.User.User;
using ProtectiveEquipmentSession = HyFive.Domain.Session.ProtectiveEquipmentSession;

namespace HyFive.Services.AutoMapperProfiler.V1
{
    public class DomainToModelsV1 : Profile
    {
        public DomainToModelsV1()
        {
            
            CreateMap<Domain.Place.OrganisationUnitType, Models.V1.OrganisationUnit.OrganisationUnitType>(MemberList.None);
            CreateMap<Domain.User.User, Models.V1.User.User>(MemberList.None)
                .ForMember(dst => dst.IsDeactivated, opt => opt.MapFrom(src => src.IsDeactivated))
                .ForMember(dst => dst.UserPermissions, opt => opt.MapFrom(src => src.UserPermissions))
                .ForMember(dst => dst.UserIdentifiers, opt => opt.MapFrom(src => src.UserIdentifiers));
            CreateMap<Domain.Place.OrganisationUnit, Models.V1.OrganisationUnit.OrganisationUnit>(MemberList.None)
                .ForMember(dst => dst.Roles, opt => opt.MapFrom(o => o.OrganisationUnitRoles.Select(r => r.Role)))
                .ForMember(dst => dst.Children, opt => opt.MapFrom(o => o.Children));
            CreateMap<Domain.Observation.Role, Models.V1.Observation.Role>(MemberList.None);
            CreateMap<Domain.Place.OrganisationUnitLevel, Models.V1.OrganisationUnit.OrganisationUnitLevel>(MemberList.None);
            CreateMap<HyFive.Domain.Place.OrganisationUnitType, HyFive.Models.V1.OrganisationUnit.OrganisationUnitType>(MemberList.None);
            CreateMap<HyFive.Domain.Place.Address, HyFive.Models.V1.OrganisationUnit.Address>(MemberList.None);
            CreateMap<HyFive.Domain.Place.City, Models.V1.OrganisationUnit.City>(MemberList.None);
            CreateMap<HyFive.Domain.User.UserPermission, HyFive.Models.V1.User.UserPermission>(MemberList.None);
            CreateMap<HyFive.Domain.User.UserIdentifier, HyFive.Models.V1.User.UserIdentifier>(MemberList.None);
            CreateMap<HyFive.Domain.User.UserIdentifierType, HyFive.Models.V1.User.UserIdentifierType>(MemberList.None);

            CreateMap<FiveIndicationsSession, Models.V1.Session.FiveIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => GetFacilityId(src)))
                .ForMember(dst => dst.Facility, opt => opt.MapFrom(src => GetFacility(src)))
                .ForMember(dst => dst.Observations, opt => opt.MapFrom(src => src.Observations));
            CreateMap<FiveIndicationsObservation, Models.V1.Observation.FiveIndicatorsObservation>(MemberList.None)
                .ForMember(dst => dst.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dst => dst.Activity, opt => opt.MapFrom(src => src.Activity))
                .ForMember(dst => dst.IndicationTypes, opt => opt.MapFrom(src => src.IndicationTypes))
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dst => dst.RegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime));
            CreateMap<IndicationTypes, Models.V1.Observation.IndicationType>(MemberList.None);
            CreateMap<Activity, Models.V1.Observation.Activity>(MemberList.None);
            CreateMap<ActivityType, Models.V1.Observation.ActivityType>(MemberList.None);

            CreateMap<HandJewelrySession, Models.V1.Session.HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => GetFacilityId(src)))
                .ForMember(dst => dst.Facility, opt => opt.MapFrom(src => GetFacility(src)))
                .ForMember(dst => dst.Observations, opt => opt.MapFrom(src => src.Observations));
            CreateMap<HandJewelryObservation, Models.V1.Observation.HandJewelryObservation>(MemberList.None);
            CreateMap<HandJewelryType, Models.V1.Observation.HandJewelryType>();

            CreateMap<ProtectiveEquipmentSession, Models.V1.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => GetFacilityId(src)))
                .ForMember(dst => dst.Facility, opt => opt.MapFrom(src => GetFacility(src)))
                .ForMember(dst => dst.Observations, opt => opt.MapFrom(src => src.Observations));

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
                .ForMember(dst => dst.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.FacilityId, opt => opt.MapFrom(src => GetFacilityId(src)))
                .ForMember(dst => dst.Facility, opt => opt.MapFrom(src => GetFacility(src)))
                .ForMember(dst => dst.Observations, opt => opt.MapFrom(src => src.Observations));
            CreateMap<GloveObservation, Models.V1.Observation.Gloves.GloveObservation>(MemberList.None);
            CreateMap<GloveWithIndicationType, Models.V1.Observation.Gloves.GloveWithIndicationType>();
            CreateMap<GloveWithoutIndicationType, Models.V1.Observation.Gloves.GloveWithoutIndicationType>();
            CreateMap<HandHygieneAfterGloveUseType, Models.V1.Observation.Gloves.PostGloveHandHygieneType>();

            CreateMap<Domain.Place.OrganisationUnit, Models.V1.OrganisationUnit.FacilityReport>(MemberList.None);

            CreateMap<Domain.Place.OrganisationUnit, Models.V1.Overview.FacilityOverviewReport>(MemberList.None);
            CreateMap<Domain.Place.OrganisationUnit, Models.V1.Overview.UnitOverviewReport>(MemberList.None);

            CreateMap<Domain.Session.Session, SessionReport>(MemberList.None)
                .ForMember(dest => dest.UnitId,
                    opt => opt.MapFrom(src => src.OrganisationUnitId))
                .ForMember(dest => dest.UnitName,
                    opt => opt.MapFrom(src => src.OrganisationUnit != null
                        ? src.OrganisationUnit.Name
                        : null))
                .ForMember(dest => dest.DepartmentId,
                    opt => opt.MapFrom(src => src.OrganisationUnit != null && src.OrganisationUnit.Parent != null
                        ? src.OrganisationUnit.Parent.Id
                        : 0))
                .ForMember(dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.OrganisationUnit != null && src.OrganisationUnit.Parent != null
                        ? src.OrganisationUnit.Parent.Name
                        : null))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
                .ForMember(dest => dest.TypeName,
                    opt => opt.MapFrom(src => GetSessionTypeDisplayNameFromDiscriminator(src.Discriminator)))
                .ForMember(dest => dest.FacilityId,
                    opt => opt.MapFrom(src => src.OrganisationUnit != null &&
                                              src.OrganisationUnit.Parent != null &&
                                              src.OrganisationUnit.Parent.Parent != null
                        ? src.OrganisationUnit.Parent.Parent.Id
                        : 0))
                .ForMember(dest => dest.FacilityName,
                    opt => opt.MapFrom(src => src.OrganisationUnit != null &&
                                              src.OrganisationUnit.Parent != null &&
                                              src.OrganisationUnit.Parent.Parent != null
                        ? src.OrganisationUnit.Parent.Parent.Name
                        : null));
            

            CreateMap<FiveIndicationsObservation, ObservationOverviewReport>(MemberList.None);

            
            CreateMap<HandJewelryObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.HandJewelryTypes, opt => opt.MapFrom(src => src.HandJewelries));
            

            CreateMap<ProtectiveEquipmentObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes, opt => opt.MapFrom(src => src.SettingType.Name))
                .ForMember(dest => dest.ProtectiveEquipment, opt => opt.MapFrom(src => src.ProtectiveEquipmentList));
            

            CreateMap<Domain.Observation.ProtectiveEquipment.ProtectiveEquipment, ProtectiveEquipmentOverviewReport>(MemberList.None)
                .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.EquipmentType.Name))
                .ForMember(dest => dest.MisuseTypes, opt => opt.MapFrom(src => src.MisuseTypes.Select(ft => ft.Name)));
            

            CreateMap<GloveObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.GloveObservation,
                    opt => opt.MapFrom(src => src));

            CreateMap<Domain.Session.TransferStatusType, Models.V1.Overview.TransferStatusType>(MemberList.None);

            CreateMap<Domain.Place.PredefinedComment, Models.V1.OrganisationUnit.PredefinedComment>(MemberList.None);

            // Request
            CreateMap<Domain.Place.OrganisationUnit, Models.V1.OrganisationUnit.OrganisationUnit>(MemberList.None)
                .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.OrganisationUnitRoles.Select(r => r.Role).ToList()))
                .ForMember(d => d.Children, opt => opt.MapFrom(s => s.Children))
                .ForMember(d => d.HasObservations, opt => opt.Ignore());

            ApplySessionOverviewMappings(CreateMap<FiveIndicationsSession, SessionOverviewReport>(MemberList.None));
            ApplySessionOverviewMappings(CreateMap<HandJewelrySession, SessionOverviewReport>(MemberList.None));
            ApplySessionOverviewMappings(CreateMap<ProtectiveEquipmentSession, SessionOverviewReport>(MemberList.None));
            ApplySessionOverviewMappings(CreateMap<GloveSession, SessionOverviewReport>(MemberList.None));

            var fiveIndicationsMap = CreateMap<FiveIndicationsObservation, FiveIndicationsObservationReport>(MemberList.None);
            ApplyFiveIndicationsReportHeader(fiveIndicationsMap);
            fiveIndicationsMap
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.FiveIndicationsSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime))
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.Activity.ActivityType.Name))
                .ForMember(dest => dest.Indications, opt => opt.MapFrom(src => string.Join(",", src.IndicationTypes.Select(i => i.Name))))
                .ForMember(dest => dest.SecondsUsed, opt => opt.MapFrom(src => src.Activity.SecondsUsed))
                .ForMember(dest => dest.TimingWasPerformed, opt => opt.MapFrom(src => src.Activity.TimingWasPerformed))
                .ForMember(dest => dest.GlovesUsed, opt => opt.MapFrom(src => src.Activity.GlovesUsed));

            var gloveMap = CreateMap<GloveObservation, GloveObservationReport>(MemberList.None);
            ApplyGloveReportHeader(gloveMap);
            gloveMap
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.GloveSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime))
                .ForMember(dest => dest.HandHygieneAfterGloveUseCode, opt => opt.MapFrom(src => src.PostGloveHandHygieneType.Code))
                .ForMember(dest => dest.GlovesWithoutIndicationCode, opt => opt.MapFrom(src => string.Join(",", src.GloveWithoutIndicationTypes.Select(h => h.Code))))
                .ForMember(dest => dest.GlovesWithIndicationCode, opt => opt.MapFrom(src => string.Join(",", src.GloveWithIndicationTypes.Select(h => h.Code))));

            var handJewelryMap = CreateMap<HandJewelryObservation, HandJewelryObservationReport>(MemberList.None);
            ApplyHandJewelryReportHeader(handJewelryMap);
            handJewelryMap
                .ForMember(dest => dest.SessionCreationTime, opt => opt.MapFrom(src => src.HandJewelrySession.CreatedDate))
                .ForMember(dest => dest.ObservationRegistrationTime, opt => opt.MapFrom(src => src.RegisteredTime))
                .ForMember(dest => dest.BareBelowElbowsTypeCodes, opt => opt.MapFrom(src => string.Join(",", src.HandJewelries.Select(h => h.Code))))
                .ForMember(dest => dest.BareBelowElbows, opt => opt.MapFrom(src => string.Join(",", src.HandJewelries.Select(h => h.Name))));

            var ppeMap = CreateMap<Domain.Observation.ProtectiveEquipment.ProtectiveEquipment, PPEObservationReport>(MemberList.None);
            ApplyPpeReportHeader(ppeMap);
            ppeMap
                .ForMember(dest => dest.CreateSessionTime, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.CreatedDate))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.RegisteredTime))
                .ForMember(dest => dest.WasUsed, opt => opt.MapFrom(src => src.WasUsed))
                .ForMember(dest => dest.WasUsedCorrectly, opt => opt.MapFrom(src => src.WasUsedCorrectly))
                .ForMember(dest => dest.IsIndicated, opt => opt.MapFrom(src => src.IsRequired))
                .ForMember(dest => dest.ProtectiveEquipmentCode, opt => opt.MapFrom(src => src.EquipmentType.Code))
                .ForMember(dest => dest.ProtectiveEquipment, opt => opt.MapFrom(src => src.EquipmentType.Name))
                .ForMember(dest => dest.ProtectiveEquipmentSettingCode, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.SettingType.Code))
                .ForMember(dest => dest.ProtectiveEquipmentSetting, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.SettingType.Name))
                .ForMember(dest => dest.Misuse, opt => opt.MapFrom(src => string.Join(",", src.MisuseTypes.Select(f => f.Name))));
        }

        private static void ApplySessionOverviewMappings<TSource>(IMappingExpression<TSource, SessionOverviewReport> map)
            where TSource : Domain.Session.Session
        {
            map.ForMember(dest => dest.Type, opt => opt.MapFrom(src => SessionHelper.GetSessionType(src.Discriminator)))
               .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => GetSessionTypeDisplayNameFromDiscriminator(src.Discriminator)))
               .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => GetObserverName(src.Observer)))
               .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => GetUnitName(src)))
               .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => GetDepartmentName(src)))
               .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => GetDepartmentId(src)))
               .ForMember(dest => dest.FacilityId, opt => opt.MapFrom(src => GetFacilityId(src)));
        }

        private static void ApplyFiveIndicationsReportHeader(IMappingExpression<FiveIndicationsObservation, FiveIndicationsObservationReport> map)
        {
            map.ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.FiveIndicationsSession.Id))
               .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.FiveIndicationsSession.Observer)))
               .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => GetFacilityTypeCode(src.FiveIndicationsSession)))
               .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => GetFacilityTypeName(src.FiveIndicationsSession)))
               .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => GetFacilityName(src.FiveIndicationsSession)))
               .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => GetFacilityAbbreviation(src.FiveIndicationsSession)))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => GetUnitName(src.FiveIndicationsSession)))
               .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => GetUnitTypeName(src.FiveIndicationsSession)))
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
               .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(src => src.FiveIndicationsSession.TransferStatus.Code))
               .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(src => src.Comment))
               .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(src => src.FiveIndicationsSession.Comment))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => GetFacilityCity(src.FiveIndicationsSession)));
        }

        private static void ApplyGloveReportHeader(IMappingExpression<GloveObservation, GloveObservationReport> map)
        {
            map.ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.GloveSession.Id))
               .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.GloveSession.Observer)))
               .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => GetFacilityTypeCode(src.GloveSession)))
               .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => GetFacilityTypeName(src.GloveSession)))
               .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => GetFacilityName(src.GloveSession)))
               .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => GetFacilityAbbreviation(src.GloveSession)))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => GetUnitName(src.GloveSession)))
               .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => GetUnitTypeName(src.GloveSession)))
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
               .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(src => src.GloveSession.TransferStatus.Code))
               .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(src => src.Comment))
               .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(src => src.GloveSession.Comment))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => GetFacilityCity(src.GloveSession)));
        }

        private static void ApplyHandJewelryReportHeader(IMappingExpression<HandJewelryObservation, HandJewelryObservationReport> map)
        {
            map.ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.HandJewelrySession.Id))
               .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.HandJewelrySession.Observer)))
               .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => GetFacilityTypeCode(src.HandJewelrySession)))
               .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => GetFacilityTypeName(src.HandJewelrySession)))
               .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => GetFacilityName(src.HandJewelrySession)))
               .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => GetFacilityAbbreviation(src.HandJewelrySession)))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => GetUnitName(src.HandJewelrySession)))
               .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => GetUnitTypeName(src.HandJewelrySession)))
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
               .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(src => src.HandJewelrySession.TransferStatus.Code))
               .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(src => src.Comment))
               .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(src => src.HandJewelrySession.Comment))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => GetFacilityCity(src.HandJewelrySession)));
        }

        private static void ApplyPpeReportHeader(IMappingExpression<Domain.Observation.ProtectiveEquipment.ProtectiveEquipment, PPEObservationReport> map)
        {
            map.ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.Id))
               .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Id))
               .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => GetObserverName(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Observer)))
               .ForMember(dest => dest.FacilityTypeCode, opt => opt.MapFrom(src => GetFacilityTypeCode(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession)))
               .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => GetFacilityTypeName(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession)))
               .ForMember(dest => dest.Facility, opt => opt.MapFrom(src => GetFacilityName(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession)))
               .ForMember(dest => dest.FacilityAbbreviation, opt => opt.MapFrom(src => GetFacilityAbbreviation(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession)))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => GetUnitName(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession)))
               .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => GetUnitTypeName(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession)))
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.Role.Name))
               .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.TransferStatus.Code))
               .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(src => src.Comment))
               .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(src => src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession.Comment))
               .ForMember(dest => dest.City, opt => opt.MapFrom(src => GetFacilityCity(src.ProtectiveEquipmentObservation.ProtectiveEquipmentSession)));
        }


        private static string GetObserverName(ObserverUser observer)
        {
            if (observer == null)
                return null;

            return string.Join(" ", new[] { observer.FirstName, observer.LastName }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static Domain.Place.OrganisationUnit GetUnit(Domain.Session.Session session)
            => session?.OrganisationUnit;

        private static Domain.Place.OrganisationUnit GetDepartment(Domain.Session.Session session)
            => session?.OrganisationUnit?.Parent;

        private static Domain.Place.OrganisationUnit GetFacility(Domain.Session.Session session)
            => session?.OrganisationUnit?.Parent?.Parent;

        private static string GetUnitName(Domain.Session.Session session)
            => GetUnit(session)?.Name;

        private static string GetUnitTypeName(Domain.Session.Session session)
            => GetUnit(session)?.Type?.Name;

        private static int GetDepartmentId(Domain.Session.Session session)
            => GetDepartment(session)?.Id ?? 0;

        private static string GetDepartmentName(Domain.Session.Session session)
            => GetDepartment(session)?.Name;

        private static int GetFacilityId(Domain.Session.Session session)
            => GetFacility(session)?.Id ?? 0;

        private static string GetFacilityName(Domain.Session.Session session)
            => GetFacility(session)?.Name;

        private static string GetFacilityAbbreviation(Domain.Session.Session session)
            => GetFacility(session)?.Abbreviation;

        private static string GetFacilityTypeCode(Domain.Session.Session session)
            => GetFacility(session)?.Type?.Code;

        private static string GetFacilityTypeName(Domain.Session.Session session)
            => GetFacility(session)?.Type?.Name;

        private static string GetFacilityCity(Domain.Session.Session session)
            => GetFacility(session)?.Address?.City?.Name;

        public static string GetSessionTypeDisplayNameFromDiscriminator(string discriminator)
        {
            if (discriminator == nameof(FiveIndicationsSession))
                return "Hand hygiene";

            if (discriminator == nameof(HandJewelrySession))
                return "Bare Below Elbows";

            if (discriminator == nameof(ProtectiveEquipmentSession))
                return "Protective Equipment";

            if (discriminator == nameof(GloveSession))
                return "Gloves";

            return "Not selected";
        }
    }
}