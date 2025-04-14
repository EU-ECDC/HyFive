using AutoMapper;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observation;
using HyFive.Domene.Observation.ProtectiveEquipment;
using HyFive.Domene.Observation.Gloves;
using HyFive.Domene.Session;
using HyFive.Domene.Place;

namespace HyFive.Services.AutoMapperProfiler.V1
{
    public class ModellerV1TilDomene : Profile
    {
        public ModellerV1TilDomene()
        {
            CreateMap<Models.V1.Institution.Institution, Domene.Place.Institution>(MemberList.None);
            CreateMap<Models.V1.Institution.InstitutionType, InstitutionType>(MemberList.None);
            CreateMap<Models.V1.User.User, Domene.Bruker.Bruker>(MemberList.None);
            CreateMap<Models.V1.Institution.Department, Domene.Place.Avdeling>(MemberList.None);
            CreateMap<Models.V1.Institution.DepartmentType, Domene.Place.SectionType>(MemberList.None);
            CreateMap<Models.V1.Observation.Role, Domene.Observation.Role>(MemberList.None);
            CreateMap<Models.V1.ForesporselOmBrukertilgang.UserAccessRequest, Domene.Bruker.ForesporselOmBrukertilgang>(MemberList.None);

            CreateMap<Models.V1.Session.FourIndicationsSession, FourIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
            .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Models.V1.Observation.FourIndicatorsObservation, FourIndicationsObservation>(MemberList
                .None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<Models.V1.Observation.IndicationType, IndicationTypes>(MemberList.None);
            CreateMap<Models.V1.Observation.Activity, Activity>(MemberList.None);
            CreateMap<Models.V1.Observation.ActivityType, ActivityType>(MemberList.None);

            CreateMap<Models.V1.Session.HandJewelrySession, HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Models.V1.Observation.HandJewelryObservation, HandJewelryObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<Models.V1.Observation.HandJewelryType, HandJewelryType>(MemberList.None);

            CreateMap<Models.V1.Session.ProtectiveEquipmentSession, Domene.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<
                    Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation, Domene.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType, ProtectiveEquipmentSettingType>(MemberList.None);
            CreateMap<
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment, Domene.Observation.ProtectiveEquipment.ProtectiveEquipment>(MemberList.None);
            CreateMap<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType, ProtectiveEquipmentType>(
                MemberList.None);
            CreateMap<Models.V1.Observation.ProtectiveEquipment.IncorrectType, MisuseType>(MemberList.None);

            CreateMap<Models.V1.Session.HanskeSesjon, GloveSession>(MemberList.None)
                .ForMember(dst => dst.StartTime, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Models.V1.Observation.Gloves.GloveObservation, GloveObservation>(MemberList.None)
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<Models.V1.Observation.Gloves.IndicatedGloveType, IndicatedGloveType>(MemberList
                .None);
            CreateMap<Models.V1.Observation.Gloves.GeneralPurposeGloveType, GeneralPurposeGloveType>(MemberList
                .None);
            CreateMap<Models.V1.Observation.Gloves.PostGloveHandHygieneType, PostGloveHandHygiene>(
                MemberList.None);
            CreateMap<Models.V1.Overview.TransferStatusType, TransmissionStatusType>(MemberList.None);
        }
    }
}
