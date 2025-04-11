using System;
using System.Linq;
using AutoMapper;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observation;
using HyFive.Domene.Observation.ProtectiveEquipment;
using HyFive.Domene.Observation.Gloves;
using HyFive.Domene.Place;
using HyFive.Modeller.Sesjon;
using HyFive.Modeller.V1.Institution;
using HyFive.Modeller.V1.Oversikt;
using HyFive.Modeller.V1.Rapport.Beskyttelsesutstyr;
using HyFive.Modeller.V1.Rapport.FireIndikasjoner;
using HyFive.Modeller.V1.Rapport.Handsmykke;
using HyFive.Modeller.V1.Rapport.Hanske;
using HyFive.Modeller.V1.Sesjon;
using ProtectiveEquipmentSession = HyFive.Domene.Session.ProtectiveEquipmentSession;
using FourIndicationsSession = HyFive.Domene.Session.FourIndicationsSession;
using HandJewelrySession = HyFive.Domene.Session.HandJewelrySession;
using GloveSession = HyFive.Domene.Session.GloveSession;
using SesjonType = HyFive.Modeller.V1.Sesjon.SesjonType;

namespace HyFive.Tjenester.AutoMapperProfiler.V1
{
    public class DomeneTilModellerV1 : Profile
    {
        public DomeneTilModellerV1()
        {
            var norskTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            
            CreateMap<Domene.Place.Institution, Modeller.V1.Institution.Institution>(MemberList.None)
                .ForMember(dst => dst.Departments, opt => opt.MapFrom(i => i.Departments.ToList()));
            CreateMap<Domene.Place.InstitutionType, Modeller.V1.Institution.InstitutionType>(MemberList.None);
            CreateMap<Domene.Bruker.Bruker, Modeller.V1.User.User>(MemberList.None)
                .ForMember(dst => dst.InstitutionId, opt => opt.MapFrom(b => b.Institusjon != null ? b.Institusjon.Id : 0));
            CreateMap<Domene.Bruker.ForesporselOmBrukertilgang, Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest>(MemberList.None);
            CreateMap<Domene.Place.Avdeling, Modeller.V1.Institution.Department>(MemberList.None)
                .ForMember(dst => dst.AvdelingTypeId, opt => opt.MapFrom(o => o.Avdelingtype != null ? o.Avdelingtype.Id : 0))
                .ForMember(dst => dst.Roller, opt => opt.MapFrom(o => o.Roller.ToList()));
            CreateMap<Domene.Observation.Role, Modeller.V1.Observasjon.Role>(MemberList.None);
            CreateMap<Domene.Place.SectionType, Modeller.V1.Institution.DepartmentType>(MemberList.None);

            CreateMap<FourIndicationsSession, Modeller.V1.Sesjon.FireIndikasjonerSesjon>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<FourIndicationsObservation, Modeller.V1.Observasjon.FireIndikasjonerObservasjon>(MemberList
                .None);
            CreateMap<IndicationTypes, Modeller.V1.Observasjon.IndicationType>(MemberList.None);
            CreateMap<Activity, Modeller.V1.Observasjon.Activity>(MemberList.None);
            CreateMap<ActivityType, Modeller.V1.Observasjon.ActivityType>(MemberList.None);

            CreateMap<HandJewelrySession, Modeller.V1.Sesjon.HandsmykkeSesjon>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<HandJewelryObservation, Modeller.V1.Observasjon.HandsmykkeObservasjon>(MemberList.None);
            CreateMap<HandJewelryType, Modeller.V1.Observasjon.HandJewelryType>(MemberList.None);

            CreateMap<ProtectiveEquipmentSession, Modeller.V1.Sesjon.BeskyttelsesutstyrSesjon>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<ProtectiveEquipmentSession,
                Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentObservation>(MemberList.None);
            CreateMap<Domene.Observation.ProtectiveEquipment.ProtectiveEquipment,
                Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment>(MemberList.None);
            CreateMap<ProtectiveEquipmentType, Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentType>(
                MemberList.None);
            CreateMap<ProtectiveEquipmentSettingType,
                Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentSettingType>(MemberList.None)
                .ForMember(dst => dst.EquipmentTypes, opt => opt.MapFrom(o => o.PPEConfigurationTypes));
            CreateMap<MisuseType, Modeller.V1.Observasjon.Beskyttelsesutstyr.MisuseType>(MemberList.None);

            CreateMap<PPEConfigurationType,
                    Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentType>(MemberList.None)
                .ForMember(dst => dst.IsDefault, opt => opt.MapFrom(o => o.IsDefault))
                .ForMember(dst => dst.IsRequired, opt => opt.MapFrom(o => o.IsDefault))
                .ForMember(dst => dst.Code, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Code))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Id))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.Name))
                .ForMember(dst => dst.MisuseTypes, opt => opt.MapFrom(o => o.ProtectiveEquipmentType.MisuseTypes));

            CreateMap<GloveSession, Modeller.V1.Sesjon.HanskeSesjon>(MemberList.None)
                .ForMember(dst => dst.Observasjoner, opt => opt.MapFrom(src => src.Observations))
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observer.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observer.Institusjon.Id));
            CreateMap<GloveObservation, Modeller.V1.Observasjon.Gloves.GloveObservation>(MemberList.None);
            CreateMap<IndicatedGloveType, Modeller.V1.Observasjon.Gloves.IndicatedGloveType>(MemberList
                .None);
            CreateMap<GeneralPurposeGloveType, Modeller.V1.Observasjon.Gloves.GeneralPurposeGloveType>(MemberList
                .None);
            CreateMap<PostGloveHandHygiene, Modeller.V1.Observasjon.Gloves.PostGloveHandHygieneType>(
                MemberList.None);

            CreateMap<Domene.Place.Institution, Modeller.V1.Institution.InstitutionReport>(MemberList.None);

            CreateMap<Domene.Place.Institution, Modeller.V1.Oversikt.InstitutionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.NumberOfSessions, opt => opt.MapFrom(src => src.Departments.Sum(x => x.Sesjoner.Count)));
            CreateMap<Domene.Place.Avdeling, Modeller.V1.Oversikt.DepartmentOverviewReport>(MemberList.None)
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

            // Hanske
            CreateMap<GloveSession, SessionOverviewReport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObserverName, opt => opt.MapFrom(src => HentObservatorNavn(src.Observer)));

            CreateMap<GloveObservation, ObservationOverviewReport>(MemberList.None)
                .ForMember(dest => dest.GloveObservation,
                    opt => opt.MapFrom(src => src));

            CreateMap<Domene.Session.TransmissionStatusType, TransferStatusType>(MemberList.None);

            CreateMap<Domene.Place.PredefinedComments, Modeller.V1.Institution.PredefinedComment>(MemberList.None);

            // Clinic
            CreateMap<Domene.Place.Clinic, Modeller.V1.Institution.Clinic>(MemberList.None)
                .ForMember(dest => dest.InstitutionId, opt => opt.MapFrom(src => src.Institution.Id));

            //Region
            CreateMap<Domene.Place.Region, Modeller.V1.Institution.Region>(MemberList.None);

            // Fire indikasjoner observasjon-rapport
            CreateMap<FourIndicationsObservation, FireIndikasjonerObservasjonRapport>(MemberList.None)
                .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.FourIndicationsSession.Id))
                .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.FourIndicationsSession.Observer)))
                .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.FourIndicationsSession.CreatedTime))
                .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.RegistrationTime))
                .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Navn))
                .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Forkortelse))
                .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Navn))
                .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Avdelingtype.Navn))
                .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.FourIndicationsSession.TransmissionStatus.Code))
                .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.FourIndicationsSession.Comment))
                .ForMember(dest => dest.Aktivitet, opt => opt.MapFrom(o => o.Activity.ActivityType.Name))
                .ForMember(dest => dest.Indikasjoner, opt => opt.MapFrom(o => string.Join(',', o.IndicationTypes.Select(i => i.Name))))
                .ForMember(dest => dest.SekunderBrukt, opt => opt.MapFrom(o => o.Activity.TimeSpent))
                .ForMember(dest => dest.TidtakingBleUtført, opt => opt.MapFrom(o => o.Activity.TimeRecordingWasDone))
                .ForMember(dest => dest.BenyttetHanske, opt => opt.MapFrom(o => o.Activity.GloveUsed))
                .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.FourIndicationsSession.Department.Institusjon.Kommune.Navn));


            // Hanske-rapport

            CreateMap<GloveObservation, HanskeObservasjonRapport>()
                .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.GloveSession.Id))
                .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.GloveSession.Observer)))
                .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.GloveSession.CreatedTime))
                .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.RegistrationTime))
                .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Navn))
                .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Forkortelse))
                .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.GloveSession.Department.Navn))
                .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.GloveSession.Department.Avdelingtype.Navn))
                .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.Role.Name))
                .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.GloveSession.TransmissionStatus.Code))
                .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.GloveSession.Comment))
                .ForMember(dest => dest.HandhygieneEtterHanskebrukKode, opt => opt.MapFrom(o => o.HandhygieneEtterHanskebrukType.Code))
                .ForMember(dest => dest.HanskeUtenIndikasjonKode, opt => opt.MapFrom(o => string.Join(',',o.GeneralPurposeGloveTypes.Select(h => h.Code))))
                .ForMember(dest => dest.HanskeMedIndikasjonKode, opt => opt.MapFrom(o => string.Join(',', o.IndicatedGloveTypes.Select(h => h.Code))))
                .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.GloveSession.Department.Institusjon.Kommune.Navn));

            // Handsmykke-rapport

            CreateMap<HandJewelryObservation, HandsmykkeObservasjonRapport>()
               .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.HandJewelrySession.Id))
               .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.HandJewelrySession.Observer)))
               .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.HandJewelrySession.CreatedTime))
               .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.RegistrationTime))
               .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Institusjontype.Kode))
               .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Institusjontype.Navn))
               .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Navn))
               .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Forkortelse))
               .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Navn))
               .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Avdelingtype.Navn))
               .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.Role.Name))
               .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.HandJewelrySession.TransmissionStatus.Code))
               .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Comment))
               .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.HandJewelrySession.Comment))
               .ForMember(dest => dest.HandsmykketypeKoder, opt => opt.MapFrom(o => string.Join(',', o.HandJewelry.Select(h => h.Code))))
               .ForMember(dest => dest.Handsmykketyper, opt => opt.MapFrom(o => string.Join(',', o.HandJewelry.Select(h => h.Name))))
               .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Helseforetak.Navn))
               .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
               .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Kommune.Nummer))
               .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.HandJewelrySession.Department.Institusjon.Kommune.Navn));

            // ProtectiveEquipment-rapport
            CreateMap<HyFive.Domene.Observation.ProtectiveEquipment.ProtectiveEquipment, BeskyttelsesutstyrObservasjonRapport>()
                .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.Id))
                .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Id))
                .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Observer)))
                .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.CreatedTime))
                .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.RegistrationTime))
                .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Navn))
                .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Forkortelse))
                .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Navn))
                .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Avdelingtype.Navn))
                .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.Role.Name))
                .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.TransmissionStatus.Code))
                .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Comment))
                .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Comment))
                .ForMember(dest => dest.BleBenyttet, opt => opt.MapFrom(o => o.WasUsed))
                .ForMember(dest => dest.BleBenyttetRiktig, opt => opt.MapFrom(o => o.WasUsedCorrectly))
                .ForMember(dest => dest.ErIndikert, opt => opt.MapFrom(o => o.IsRequired))
                .ForMember(dest => dest.BeskyttelsesutstyrKode, opt => opt.MapFrom(o => o.EquipmentType.Code))
                .ForMember(dest => dest.Beskyttelsesutstyr, opt => opt.MapFrom(o => o.EquipmentType.Name))
                .ForMember(dest => dest.BeskyttelsesutstyrsettingKode, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.SettingType.Code))
                .ForMember(dest => dest.Beskyttelsesutstyrsetting, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.SettingType.Name))
                .ForMember(dest => dest.Feilbruk, opt => opt.MapFrom(o => string.Join(',',o.MisuseTypes.Select(f => f.Name))))
                .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.ProtectiveEquipmentSession.Department.Institusjon.Kommune.Navn));

            CreateMap<Domene.Place.HealthcareProvider, Modeller.V1.Institution.HealthcareEnterprise>()
                .ForMember(dest => dest.RegionaltHelseforetakId, opt => opt.MapFrom(src => src.RegionaltHealthcareProvider != null ? src.RegionaltHealthcareProvider.Id : 0));

            CreateMap<Domene.Place.RegionaltHealthcareProvider, Modeller.V1.Institution.RegionalInstitution>(MemberList.None);

            CreateMap<Domene.Place.Municipality, Modeller.V1.Institution.Comment>(MemberList.None);
        }

        private static string HentObservatorNavn(Observator observator)
        {
            return string.Join(" ", new[] { observator.Fornavn, observator.Etternavn });
        }
    }
}