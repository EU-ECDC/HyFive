using System;
using System.Linq;
using AutoMapper;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observation;
using HyFive.Domene.Observation.ProtectiveEquipment;
using HyFive.Domene.Observation.Gloves;
using HyFive.Domene.Place;
using HyFive.Models.Sesjon;
using HyFive.Models.V1.Institution;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Report.Beskyttelsesutstyr;
using HyFive.Models.V1.Report.FourIndications;
using HyFive.Models.V1.Report.HandJewelry;
using HyFive.Models.V1.Report.Glove;
using HyFive.Models.V1.Session;
using ProtectiveEquipmentSession = HyFive.Domene.Session.ProtectiveEquipmentSession;
using FourIndicationsSession = HyFive.Domene.Session.FourIndicationsSession;
using HandJewelrySession = HyFive.Domene.Session.HandJewelrySession;
using GloveSession = HyFive.Domene.Session.GloveSession;
using SesjonType = HyFive.Models.V1.Session.SesjonType;

namespace HyFive.Services.AutoMapperProfiler.V1
{
    public class DomeneTilModellerV1 : Profile
    {
        public DomeneTilModellerV1()
        {
            var norskTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            
            CreateMap<Domene.Place.Institution, Models.V1.Institution.Institution>(MemberList.None)
                .ForMember(dst => dst.Departments, opt => opt.MapFrom(i => i.Departments.ToList()));
            CreateMap<Domene.Place.InstitutionType, Models.V1.Institution.InstitutionType>(MemberList.None);
            CreateMap<Domene.Bruker.Bruker, Models.V1.User.User>(MemberList.None)
                .ForMember(dst => dst.InstitutionId, opt => opt.MapFrom(b => b.Institusjon != null ? b.Institusjon.Id : 0));
            CreateMap<Domene.Bruker.ForesporselOmBrukertilgang, Models.V1.ForesporselOmBrukertilgang.UserAccessRequest>(MemberList.None);
            CreateMap<Domene.Place.Avdeling, Models.V1.Institution.Department>(MemberList.None)
                .ForMember(dst => dst.AvdelingTypeId, opt => opt.MapFrom(o => o.Avdelingtype != null ? o.Avdelingtype.Id : 0))
                .ForMember(dst => dst.Roller, opt => opt.MapFrom(o => o.Roller.ToList()));
            CreateMap<Domene.Observation.Role, Models.V1.Observation.Role>(MemberList.None);
            CreateMap<Domene.Place.SectionType, Models.V1.Institution.DepartmentType>(MemberList.None);

            CreateMap<FourIndicationsSession, Models.V1.Session.FourIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<FourIndicationsObservation, Models.V1.Observation.FourIndicatorsObservation>(MemberList
                .None);
            CreateMap<IndicationTypes, Models.V1.Observation.IndicationType>(MemberList.None);
            CreateMap<Activity, Models.V1.Observation.Activity>(MemberList.None);
            CreateMap<ActivityType, Models.V1.Observation.ActivityType>(MemberList.None);

            CreateMap<HandJewelrySession, Models.V1.Session.HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<HandJewelryObservation, Models.V1.Observation.HandJewelryObservation>(MemberList.None);
            CreateMap<HandJewelryType, Models.V1.Observation.HandJewelryType>(MemberList.None);

            CreateMap<ProtectiveEquipmentSession, Models.V1.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<ProtectiveEquipmentSession,
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(MemberList.None);
            CreateMap<Domene.Observation.ProtectiveEquipment.ProtectiveEquipment,
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment>(MemberList.None);
            CreateMap<ProtectiveEquipmentType, Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType>(
                MemberList.None);
            CreateMap<ProtectiveEquipmentSettingType,
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType>(MemberList.None)
                .ForMember(dst => dst.EquipmentTypes, opt => opt.MapFrom(o => o.PPEConfigurationTypes));
            CreateMap<MisuseType, Models.V1.Observation.ProtectiveEquipment.IncorrectType>(MemberList.None);

            CreateMap<PPEConfigurationType,
                    Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType>(MemberList.None)
                .ForMember(dst => dst.IsDefault, opt => opt.MapFrom(o => o.IsDefault))
                .ForMember(dst => dst.IsRequired, opt => opt.MapFrom(o => o.IsDefault))
                .ForMember(dst => dst.Code, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Code))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Name))
                .ForMember(dst => dst.IncorrectTypes, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.MisuseTypes));

            CreateMap<GloveSession, Models.V1.Session.HanskeSesjon>(MemberList.None)
                .ForMember(dst => dst.Observasjoner, opt => opt.MapFrom(src => src.Observations))
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<GloveObservation, Models.V1.Observation.Gloves.GloveObservation>(MemberList.None);
            CreateMap<IndicatedGloveType, Models.V1.Observation.Gloves.IndicatedGloveType>(MemberList
                .None);
            CreateMap<GeneralPurposeGloveType, Models.V1.Observation.Gloves.GeneralPurposeGloveType>(MemberList
                .None);
            CreateMap<PostGloveHandHygiene, Models.V1.Observation.Gloves.PostGloveHandHygieneType>(
                MemberList.None);

            CreateMap<Domene.Place.Institution, Models.V1.Institution.InstitutionReport>(MemberList.None);

            CreateMap<Domene.Place.Institution, Models.V1.Overview.InstitutionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Departments.Sum(x => x.Sesjoner.Count)));
            CreateMap<Domene.Place.Avdeling, Models.V1.Overview.DepartmentOverviewReport>(MemberList.None)
                .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Sesjoner.Count));

            CreateMap<Domene.Session.Session, SesjonRapport>(MemberList.None)
                .ForMember(dest => dest.Avdelingsnavn, opt => opt.MapFrom(src => src.Department.Navn))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.Institusjonsnavn,
                    opt => opt.MapFrom(src => src.Observer.Institusjon.Navn));

            CreateMap<FourIndicationsSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => HentObservatorNavn(src.Observer)));

            CreateMap<FourIndicationsObservation, ObservationOverviewReport>(MemberList.None);

            // HandJewelry
            CreateMap<HandJewelrySession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => HentObservatorNavn(src.Observer)));

            CreateMap<HandJewelryObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.HandJewelryTypes,
                    opt => opt.MapFrom(src => src.HandJewelry));

            // ProtectiveEquipment
            CreateMap<ProtectiveEquipmentSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => HentObservatorNavn(src.Observer)));

            CreateMap<ProtectiveEquipmentSession, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.PPEConfigurationTypes, opt => opt.MapFrom(src => src.Settingtype.Name))
                .ForMember(dest => dest.ProtectiveEquipment, opt => opt.MapFrom(src => src.ProtectiveEquipmentList))
                .ForMember(dest => dest.ProtectiveEquipmentObservation, opt => opt.MapFrom(src => src));
            

            CreateMap<Domene.Observation.ProtectiveEquipment.ProtectiveEquipment, ProtectiveEquipmentOverviewReport>(MemberList.None)
                .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.EquipmentType.Name))
                .ForMember(dest => dest.MisuseTypes, opt => opt.MapFrom(src => src.MisuseTypes.Select(ft => ft.Name)));

            // Glove
            CreateMap<GloveSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => HentObservatorNavn(src.Observer)));

            CreateMap<GloveObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.GloveObservation,
                    opt => opt.MapFrom(src => src));

            CreateMap<Domene.Session.TransmissionStatusType, TransferStatusType>(MemberList.None);

            CreateMap<Domene.Place.PredefinedComments, Models.V1.Institution.PredefinedComment>(MemberList.None);

            // Clinic
            CreateMap<Domene.Place.Clinic, Models.V1.Institution.Clinic>(MemberList.None)
                .ForMember(dest => dest.InstitutionId, opt => opt.MapFrom(src => src.Institution.Id));

            //Region
            CreateMap<Domene.Place.Region, Models.V1.Institution.Region>(MemberList.None);

            // Fire indikasjoner observasjon-rapport
            CreateMap<FourIndicationsObservation, FourIndicationsObservationReport>(MemberList.None)
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.FourIndicationsSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => HentObservatorNavn(src.FourIndicationsSession.Observer)))
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.FourIndicationsSession.CreatedTime))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegistrationTime))
                .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Navn))
                .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Forkortelse))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Navn))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Avdelingtype.Navn))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.FourIndicationsSession.TransmissionStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.FourIndicationsSession.Comment))
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(o => o.Activity.ActivityType.Name))
                .ForMember(dest => dest.Indications, opt => opt.MapFrom(o => string.Join(',', o.IndicationTypes.Select(i => i.Name))))
                .ForMember(dest => dest.SecondsUsed, opt => opt.MapFrom(o => o.Activity.TimeSpent))
                .ForMember(dest => dest.TimingWasPerformed, opt => opt.MapFrom(o => o.Activity.TimeRecordingWasDone))
                .ForMember(dest => dest.GlovesUsed, opt => opt.MapFrom(o => o.Activity.GloveUsed))
                .ForMember(dest => dest.HealthcareTrust, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionalHealthcareTrust, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Kommune.Navn));


            // Glove-rapport

            CreateMap<GloveObservation, GloveObservationReport>()
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.GloveSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => HentObservatorNavn(src.GloveSession.Observer)))
                .ForMember(dest => dest.SessionCreatedTime, opt => opt.MapFrom(src => src.GloveSession.CreatedTime))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.RegistrationTime))
                .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Navn))
                .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Forkortelse))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.GloveSession.Department.Navn))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.GloveSession.Department.Avdelingtype.Navn))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.GloveSession.TransmissionStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.GloveSession.Comment))
                .ForMember(dest => dest.HandHygieneAfterGloveUseCode, opt => opt.MapFrom(o => o.HandhygieneEtterHanskebrukType.Code))
                .ForMember(dest => dest.GlovesWithoutIndicationCode, opt => opt.MapFrom(o => string.Join(',',o.GeneralPurposeGloveTypes.Select(h => h.Code))))
                .ForMember(dest => dest.GlovesWithIndicationCode, opt => opt.MapFrom(o => string.Join(',', o.IndicatedGloveTypes.Select(h => h.Code))))
                .ForMember(dest => dest.HealthcareTrust, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionalHealthcareTrust, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Kommune.Navn));

            // HandJewelry-rapport

            CreateMap<HandJewelryObservation, HandJewelryObservationReport>()
               .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.HandJewelrySession.Id))
               .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => HentObservatorNavn(src.HandJewelrySession.Observer)))
               .ForMember(dest => dest.SessionCreationTime, opt => opt.MapFrom(src => src.HandJewelrySession.CreatedTime))
               .ForMember(dest => dest.ObservationRegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime))
               .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Institusjontype.Kode))
               .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Institusjontype.Navn))
               .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Navn))
               .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Forkortelse))
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Navn))
               .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Avdelingtype.Navn))
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.Role.Name))
               .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.HandJewelrySession.TransmissionStatus.Code))
               .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
               .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.HandJewelrySession.Comment))
               .ForMember(dest => dest.HandJewelryTypeCodes, opt => opt.MapFrom(o => string.Join(',', o.HandJewelry.Select(h => h.Code))))
               .ForMember(dest => dest.HandJewelryTypes, opt => opt.MapFrom(o => string.Join(',', o.HandJewelry.Select(h => h.Name))))
               .ForMember(dest => dest.HealthTrust, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Helseforetak.Navn))
               .ForMember(dest => dest.RegionalHealthTrust, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
               .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Kommune.Nummer))
               .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Kommune.Navn));

            // ProtectiveEquipment-rapport
            CreateMap<HyFive.Domene.Observation.ProtectiveEquipment.ProtectiveEquipment, PPEObservationReport>()
                .ForMember(dest => dest.ObservationId, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.Id))
                .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Id))
                .ForMember(dest => dest.Observer, opt => opt.MapFrom(src => HentObservatorNavn(src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Observer)))
                .ForMember(dest => dest.CreateSessionTime, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.CreatedTime))
                .ForMember(dest => dest.ObservationRegisteredTime, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.RegistrationTime))
                .ForMember(dest => dest.InstitutionTypeCode, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.InstitutionType, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institution, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Navn))
                .ForMember(dest => dest.InstitutionAbbreviation, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Forkortelse))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Navn))
                .ForMember(dest => dest.DepartmentType, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Avdelingtype.Navn))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.Role.Name))
                .ForMember(dest => dest.TransferStatus, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.TransmissionStatus.Code))
                .ForMember(dest => dest.ObservationComment, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.SessionComment, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Comment))
                .ForMember(dest => dest.WasUsed, opt => opt.MapFrom(o => o.WasUsed))
                .ForMember(dest => dest.WasUsedCorrectly, opt => opt.MapFrom(o => o.WasUsedCorrectly))
                .ForMember(dest => dest.IsIndicated, opt => opt.MapFrom(o => o.IsRequired))
                .ForMember(dest => dest.ProtectiveEquipmentCode, opt => opt.MapFrom(o => o.EquipmentType.Code))
                .ForMember(dest => dest.ProtectiveEquipment, opt => opt.MapFrom(o => o.EquipmentType.Name))
                .ForMember(dest => dest.ProtectiveEquipmentSettingCode, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.SettingType.Code))
                .ForMember(dest => dest.ProtectiveEquipmentSetting, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.SettingType.Name))
                .ForMember(dest => dest.Misuse, opt => opt.MapFrom(o => string.Join(',',o.MisuseTypes.Select(f => f.Name))))
                .ForMember(dest => dest.HealthTrust, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionalHealthTrust, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.MunicipalityNumber, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Municipality, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Kommune.Navn));

            CreateMap<Domene.Place.HealthcareProvider, Models.V1.Institution.HealthcareEnterprise>()
                .ForMember(dest => dest.RegionaltHelseforetakId, opt => opt.MapFrom(src => src.RegionaltHealthcareProvider != null ? src.RegionaltHealthcareProvider.Id : 0));

            CreateMap<Domene.Place.RegionaltHealthcareProvider, Models.V1.Institution.RegionalInstitution>(MemberList.None);

            CreateMap<Domene.Place.Municipality, Models.V1.Institution.Comment>(MemberList.None);
        }

        private static string HentObservatorNavn(Observator observator)
        {
            return string.Join(" ", new[] { observator.Fornavn, observator.Etternavn });
        }
    }
}