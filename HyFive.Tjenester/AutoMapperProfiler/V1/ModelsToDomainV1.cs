using AutoMapper;
using HyFive.Domain.User;
using HyFive.Domain.Observation;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Domain.Observation.Gloves;
using HyFive.Domain.Session;
using HyFive.Domain.Place;

namespace HyFive.Services.AutoMapperProfiler.V1
{
    public class ModelsToDomainV1 : Profile
    {
        public ModelsToDomainV1()
        {
            CreateMap<Models.V1.Institution.Institution, Domain.Place.Institution>(MemberList.None);
            CreateMap<Models.V1.Institution.InstitutionType, InstitutionType>(MemberList.None);
            CreateMap<Models.V1.User.User, Domain.User.User>(MemberList.None);
            CreateMap<Models.V1.Institution.Department, Domain.Place.Department>(MemberList.None);
            CreateMap<Models.V1.Institution.DepartmentType, Domain.Place.DepartmentType>(MemberList.None);
            CreateMap<Models.V1.Observation.Role, Domain.Observation.Role>(MemberList.None);
            CreateMap<Models.V1.UserAccessRequest.UserAccessRequest, Domain.User.UserAccessRequest>(MemberList.None);

            CreateMap<Models.V1.Session.FourIndicationsSession, FourIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
            .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartTime));
            CreateMap<Models.V1.Observation.FourIndicatorsObservation, FourIndicationsObservation>(MemberList
                .None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<Models.V1.Observation.IndicationType, IndicationTypes>(MemberList.None);
            CreateMap<Models.V1.Observation.Activity, Activity>(MemberList.None);
            CreateMap<Models.V1.Observation.ActivityType, ActivityType>(MemberList.None);

            CreateMap<Models.V1.Session.HandJewelrySession, HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartTime));
            CreateMap<Models.V1.Observation.HandJewelryObservation, HandJewelryObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<Models.V1.Observation.HandJewelryType, HandJewelryType>(MemberList.None);

            CreateMap<Models.V1.Session.ProtectiveEquipmentSession, Domain.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartTime));
            CreateMap<
                    Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation, Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType, ProtectiveEquipmentSettingType>(MemberList.None);
            CreateMap<
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment, Domain.Observation.ProtectiveEquipment.ProtectiveEquipment>(MemberList.None);
            CreateMap<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType, ProtectiveEquipmentType>(
                MemberList.None);
            CreateMap<Models.V1.Observation.ProtectiveEquipment.MisuseType, MisuseType>(MemberList.None);

            CreateMap<Models.V1.Session.GloveSession, GloveSession>(MemberList.None)
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartTime));
            CreateMap<Models.V1.Observation.Gloves.GloveObservation, GloveObservation>(MemberList.None)
                .ForMember(dst => dst.RegistrationTime, opt => opt.MapFrom(src => src.RegistrationTime));
            CreateMap<Models.V1.Observation.Gloves.IndicatedGloveType, GloveWithIndicationType>(MemberList
                .None);
            CreateMap<Models.V1.Observation.Gloves.GeneralPurposeGloveType, GloveWithoutIndicationType>(MemberList
                .None);
            CreateMap<Models.V1.Observation.Gloves.PostGloveHandHygieneType, HandHygieneAfterGloveUseType>(
                MemberList.None);
            CreateMap<Models.V1.Overview.TransferStatusType, TransferStatusType>(MemberList.None);
        }
    }
}
