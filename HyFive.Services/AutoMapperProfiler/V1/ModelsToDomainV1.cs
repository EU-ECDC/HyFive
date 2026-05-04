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
            CreateMap<Models.V1.OrganisationUnit.OrganisationUnit, Domain.Place.OrganisationUnit>(MemberList.None);
            CreateMap<Models.V1.OrganisationUnit.OrganisationUnitType, OrganisationUnitType>(MemberList.None);
            CreateMap<Models.V1.User.User, Domain.User.User>(MemberList.None);
            CreateMap<Models.V1.Observation.Role, Domain.Observation.Role>(MemberList.None);

            CreateMap<Models.V1.OrganisationUnit.OrganisationUnit, Domain.Place.OrganisationUnit>(MemberList.None)
                .ForMember(dst => dst.Children, opt => opt.Ignore())
                .ForMember(dst => dst.Parent, opt => opt.Ignore())
                .ForMember(dst => dst.OrganisationUnitRoles, opt => opt.Ignore())
                .ForMember(dst => dst.OutgoingAssociations, opt => opt.Ignore())
                .ForMember(dst => dst.IncomingAssociations, opt => opt.Ignore());

            CreateMap<Models.V1.Session.FiveIndicationsSession, Domain.Session.FiveIndicationsSession>(MemberList.None)
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.OrganisationUnit, opt => opt.Ignore())
                .ForMember(dst => dst.Observer, opt => opt.Ignore())
                .ForMember(dst => dst.TransferStatus, opt => opt.Ignore());

            CreateMap<Models.V1.Observation.FiveIndicatorsObservation, FiveIndicationsObservation>(MemberList
                .None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime));
            CreateMap<Models.V1.Observation.IndicationType, IndicationTypes>(MemberList.None);
            CreateMap<Models.V1.Observation.Activity, Activity>(MemberList.None);
            CreateMap<Models.V1.Observation.ActivityType, ActivityType>(MemberList.None);

            CreateMap<Models.V1.Session.HandJewelrySession, HandJewelrySession>(MemberList.None)
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.OrganisationUnit, opt => opt.Ignore())
                .ForMember(dst => dst.Observer, opt => opt.Ignore())
                .ForMember(dst => dst.TransferStatus, opt => opt.Ignore());
            CreateMap<Models.V1.Observation.HandJewelryObservation, HandJewelryObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime));
            CreateMap<Models.V1.Observation.HandJewelryType, HandJewelryType>(MemberList.None);

            CreateMap<Models.V1.Session.ProtectiveEquipmentSession, Domain.Session.ProtectiveEquipmentSession>(MemberList.None)
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.OrganisationUnit, opt => opt.Ignore())
                .ForMember(dst => dst.Observer, opt => opt.Ignore())
                .ForMember(dst => dst.TransferStatus, opt => opt.Ignore());
            CreateMap<
                    Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation, Domain.Observation.ProtectiveEquipment.ProtectiveEquipmentObservation>(MemberList.None)
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.RegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime));
            CreateMap<
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentSettingType, ProtectiveEquipmentSettingType>(MemberList.None);
            CreateMap<
                Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipment, Domain.Observation.ProtectiveEquipment.ProtectiveEquipment>(MemberList.None);
            CreateMap<Models.V1.Observation.ProtectiveEquipment.ProtectiveEquipmentType, ProtectiveEquipmentType>(
                MemberList.None);
            CreateMap<Models.V1.Observation.ProtectiveEquipment.MisuseType, MisuseType>(MemberList.None);

            CreateMap<Models.V1.Session.GloveSession, GloveSession>(MemberList.None)
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dst => dst.Comment, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Comment) ? null : src.Comment))
                .ForMember(dst => dst.OrganisationUnit, opt => opt.Ignore())
                .ForMember(dst => dst.Observer, opt => opt.Ignore())
                .ForMember(dst => dst.TransferStatus, opt => opt.Ignore());
            CreateMap<Models.V1.Observation.Gloves.GloveObservation, GloveObservation>(MemberList.None)
                .ForMember(dst => dst.RegisteredTime, opt => opt.MapFrom(src => src.RegisteredTime));
            CreateMap<Models.V1.Observation.Gloves.GloveWithIndicationType, GloveWithIndicationType>(MemberList
                .None);
            CreateMap<Models.V1.Observation.Gloves.GloveWithoutIndicationType, GloveWithoutIndicationType>(MemberList
                .None);
            CreateMap<Models.V1.Observation.Gloves.PostGloveHandHygieneType, HandHygieneAfterGloveUseType>(
                MemberList.None);
            CreateMap<Models.V1.Overview.TransferStatusType, TransferStatusType>(MemberList.None);
        }
    }
}
