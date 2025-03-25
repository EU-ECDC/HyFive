using AutoMapper;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;
using HyFive.Domene.Observasjon.Hansker;
using HyFive.Domene.Sesjon;
using HyFive.Domene.Sted;

namespace HyFive.Tjenester.AutoMapperProfiler.V1
{
    public class ModellerV1TilDomene : Profile
    {
        public ModellerV1TilDomene()
        {
            CreateMap<Modeller.V1.Institusjon.Institusjon, Domene.Sted.Institusjon>(MemberList.None);
            CreateMap<Modeller.V1.Institusjon.InstitusjonType, InstitusjonType>(MemberList.None);
            CreateMap<Modeller.V1.Bruker.Bruker, Domene.Bruker.Bruker>(MemberList.None);
            CreateMap<Modeller.V1.Institusjon.Avdeling, Domene.Sted.Avdeling>(MemberList.None);
            CreateMap<Modeller.V1.Institusjon.AvdelingType, Domene.Sted.AvdelingType>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Rolle, Domene.Observasjon.Rolle>(MemberList.None);
            CreateMap<Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang, Domene.Bruker.ForesporselOmBrukertilgang>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.FireIndikasjonerSesjon, FireIndikasjonerSesjon>(MemberList.None)
                .ForMember(dst => dst.Kommentar, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
            .ForMember(dst => dst.Starttidspunkt, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Modeller.V1.Observasjon.FireIndikasjonerObservasjon, FireIndikasjonerObservasjon>(MemberList
                .None)
                .ForMember(dst => dst.Kommentar, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.Registrerttidspunkt, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<Modeller.V1.Observasjon.IndikasjonType, IndikasjonType>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Aktivitet, Aktivitet>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.AktivitetType, AktivitetType>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.HandsmykkeSesjon, HandsmykkeSesjon>(MemberList.None)
                .ForMember(dst => dst.Kommentar, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.Starttidspunkt, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Modeller.V1.Observasjon.HandsmykkeObservasjon, HandsmykkeObservasjon>(MemberList.None)
                .ForMember(dst => dst.Kommentar, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.Registrerttidspunkt, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<Modeller.V1.Observasjon.HandsmykkeType, HandsmykkeType>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.BeskyttelsesutstyrSesjon, BeskyttelsesutstyrSesjon>(MemberList.None)
                .ForMember(dst => dst.Kommentar, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.Starttidspunkt, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<
                    Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrObservasjon, BeskyttelsesutstyrObservasjon>(MemberList.None)
                .ForMember(dst => dst.Kommentar, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Kommentar) ? null : src.Kommentar))
                .ForMember(dst => dst.Registrerttidspunkt, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<
                Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrsettingType, BeskyttelsesutstyrsettingType>(MemberList.None);
            CreateMap<
                Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr, Domene.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr>(MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType, BeskyttelsesutstyrType>(
                MemberList.None);
            CreateMap<Modeller.V1.Observasjon.Beskyttelsesutstyr.FeilbrukType, FeilbrukType>(MemberList.None);

            CreateMap<Modeller.V1.Sesjon.HanskeSesjon, HanskeSesjon>(MemberList.None)
                .ForMember(dst => dst.Starttidspunkt, opt => opt.MapFrom(src => src.Starttidspunkt));
            CreateMap<Modeller.V1.Observasjon.Hansker.HanskeObservasjon, HanskeObservasjon>(MemberList.None)
                .ForMember(dst => dst.Registrerttidspunkt, opt => opt.MapFrom(src => src.Registrerttidspunkt));
            CreateMap<Modeller.V1.Observasjon.Hansker.HanskeMedIndikasjonType, HanskeMedIndikasjonType>(MemberList
                .None);
            CreateMap<Modeller.V1.Observasjon.Hansker.HanskeUtenIndikasjonType, HanskeUtenIndikasjonType>(MemberList
                .None);
            CreateMap<Modeller.V1.Observasjon.Hansker.HandhygieneEtterHanskebrukType, HandhygieneEtterHanskebrukType>(
                MemberList.None);
            CreateMap<Modeller.V1.Oversikt.OverforingstatusType, OverforingstatusType>(MemberList.None);
        }
    }
}
