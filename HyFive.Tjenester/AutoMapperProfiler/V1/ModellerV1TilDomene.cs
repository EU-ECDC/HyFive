using AutoMapper;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observation;
using HyFive.Domene.Observation.ProtectiveEquipment;
using HyFive.Domene.Observation.Gloves;
using HyFive.Domene.Session;
using HyFive.Domene.Place;

namespace HyFive.Tjenester.AutoMapperProfiler.V1
{
    public class ModellerV1TilDomene : Profile
    {
        public ModellerV1TilDomene()
        {
            CreateMap<Modeller.V1.Institution.Institution, Domene.Place.Institution>(MemberList.None);
            CreateMap<Modeller.V1.Institution.InstitutionType, InstitutionType>(MemberList.None);
            CreateMap<Modeller.V1.User.User, Domene.Bruker.Bruker>(MemberList.None);
            CreateMap<Modeller.V1.Institution.Department, Domene.Place.Avdeling>(MemberList.None);
            CreateMap<Modeller.V1.Institution.DepartmentType, Domene.Place.SectionType>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Role, Domene.Observation.Role>(MemberList.None);
            CreateMap<Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest, Domene.Bruker.ForesporselOmBrukertilgang>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.FireIndikasjonerSesjon, FourIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
            .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Modeller.V1.Observasjon.FireIndikasjonerObservasjon, FourIndicationsObservation>(MemberList
                .None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<Modeller.V1.Observasjon.IndicationType, IndicationTypes>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Activity, Activity>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.ActivityType, ActivityType>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.HandsmykkeSesjon, HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Modeller.V1.Observasjon.HandsmykkeObservasjon, HandJewelryObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<Modeller.V1.Observasjon.HandJewelryType, HandJewelryType>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.BeskyttelsesutstyrSesjon, Domene.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<
                    Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentObservation, Domene.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<
                Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentSettingType, ProtectiveEquipmentSettingType>(MemberList.None);
            CreateMap<
                Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipment, Domene.Observation.ProtectiveEquipment.ProtectiveEquipment>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Beskyttelsesutstyr.ProtectiveEquipmentType, ProtectiveEquipmentType>(
                MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Beskyttelsesutstyr.MisuseType, MisuseType>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.HanskeSesjon, GloveSession>(MemberList.None)
                .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Modeller.V1.Observasjon.Gloves.GloveObservation, GloveObservation>(MemberList.None)
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<Modeller.V1.Observasjon.Gloves.IndicatedGloveType, IndicatedGloveType>(MemberList
                .None);
            CreateMap<Modeller.V1.Observasjon.Gloves.GeneralPurposeGloveType, GeneralPurposeGloveType>(MemberList
                .None);
            CreateMap<Modeller.V1.Observasjon.Gloves.PostGloveHandHygieneType, PostGloveHandHygiene>(
                MemberList.None);
            CreateMap<Modeller.V1.Oversikt.TransferStatusType, TransmissionStatusType>(MemberList.None);
        }
    }
}
