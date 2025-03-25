using System;
using System.Linq;
using AutoMapper;
using HyFive.Domene.Bruker;
using HyFive.Domene.Observasjon;
using HyFive.Domene.Observasjon.Beskyttelsesutstyr;
using HyFive.Domene.Observasjon.Hansker;
using HyFive.Domene.Sted;
using HyFive.Modeller.Sesjon;
using HyFive.Modeller.V1.Institusjon;
using HyFive.Modeller.V1.Oversikt;
using HyFive.Modeller.V1.Rapport.Beskyttelsesutstyr;
using HyFive.Modeller.V1.Rapport.FireIndikasjoner;
using HyFive.Modeller.V1.Rapport.Handsmykke;
using HyFive.Modeller.V1.Rapport.Hanske;
using HyFive.Modeller.V1.Sesjon;
using BeskyttelsesutstyrSesjon = HyFive.Domene.Sesjon.BeskyttelsesutstyrSesjon;
using FireIndikasjonerSesjon = HyFive.Domene.Sesjon.FireIndikasjonerSesjon;
using HandsmykkeSesjon = HyFive.Domene.Sesjon.HandsmykkeSesjon;
using HanskeSesjon = HyFive.Domene.Sesjon.HanskeSesjon;
using SesjonType = HyFive.Modeller.V1.Sesjon.SesjonType;

namespace HyFive.Tjenester.AutoMapperProfiler.V1
{
    public class DomeneTilModellerV1 : Profile
    {
        public DomeneTilModellerV1()
        {
            var norskTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            
            CreateMap<Domene.Sted.Institusjon, Modeller.V1.Institusjon.Institusjon>(MemberList.None)
                .ForMember(dst => dst.Avdelinger, opt => opt.MapFrom(i => i.Avdelinger.ToList()));
            CreateMap<Domene.Sted.InstitusjonType, Modeller.V1.Institusjon.InstitusjonType>(MemberList.None);
            CreateMap<Domene.Bruker.Bruker, Modeller.V1.Bruker.Bruker>(MemberList.None)
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(b => b.Institusjon != null ? b.Institusjon.Id : 0));
            CreateMap<Domene.Bruker.ForesporselOmBrukertilgang, Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang>(MemberList.None);
            CreateMap<Domene.Sted.Avdeling, Modeller.V1.Institusjon.Avdeling>(MemberList.None)
                .ForMember(dst => dst.AvdelingTypeId, opt => opt.MapFrom(o => o.Avdelingtype != null ? o.Avdelingtype.Id : 0))
                .ForMember(dst => dst.Roller, opt => opt.MapFrom(o => o.Roller.ToList()));
            CreateMap<Domene.Observasjon.Rolle, Modeller.V1.Observasjon.Rolle>(MemberList.None);
            CreateMap<Domene.Sted.AvdelingType, Modeller.V1.Institusjon.AvdelingType>(MemberList.None);

            CreateMap<FireIndikasjonerSesjon, Modeller.V1.Sesjon.FireIndikasjonerSesjon>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observator.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observator.Institusjon.Id));
            CreateMap<FireIndikasjonerObservasjon, Modeller.V1.Observasjon.FireIndikasjonerObservasjon>(MemberList
                .None);
            CreateMap<IndikasjonType, Modeller.V1.Observasjon.IndikasjonType>(MemberList.None);
            CreateMap<Aktivitet, Modeller.V1.Observasjon.Aktivitet>(MemberList.None);
            CreateMap<AktivitetType, Modeller.V1.Observasjon.AktivitetType>(MemberList.None);

            CreateMap<HandsmykkeSesjon, Modeller.V1.Sesjon.HandsmykkeSesjon>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observator.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observator.Institusjon.Id));
            CreateMap<HandsmykkeObservasjon, Modeller.V1.Observasjon.HandsmykkeObservasjon>(MemberList.None);
            CreateMap<HandsmykkeType, Modeller.V1.Observasjon.HandsmykkeType>(MemberList.None);

            CreateMap<BeskyttelsesutstyrSesjon, Modeller.V1.Sesjon.BeskyttelsesutstyrSesjon>(MemberList.None)
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observator.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observator.Institusjon.Id));
            CreateMap<BeskyttelsesutstyrObservasjon,
                Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrObservasjon>(MemberList.None);
            CreateMap<Domene.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr,
                Modeller.V1.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr>(MemberList.None);
            CreateMap<BeskyttelsesutstyrType, Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType>(
                MemberList.None);
            CreateMap<BeskyttelsesutstyrsettingType,
                Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrsettingType>(MemberList.None)
                .ForMember(dst => dst.Utstyrstyper, opt => opt.MapFrom(o => o.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper));
            CreateMap<FeilbrukType, Modeller.V1.Observasjon.Beskyttelsesutstyr.FeilbrukType>(MemberList.None);

            CreateMap<BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrType,
                    Modeller.V1.Observasjon.Beskyttelsesutstyr.BeskyttelsesutstyrType>(MemberList.None)
                .ForMember(dst => dst.ErDefaultIndikert, opt => opt.MapFrom(o => o.ErDefaultIndikert))
                .ForMember(dst => dst.ErIndikert, opt => opt.MapFrom(o => o.ErDefaultIndikert))
                .ForMember(dst => dst.Kode, opt => opt.MapFrom(o => o.BeskyttelsesutstyrType.Kode))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(o => o.BeskyttelsesutstyrType.Id))
                .ForMember(dst => dst.Navn, opt => opt.MapFrom(o => o.BeskyttelsesutstyrType.Navn))
                .ForMember(dst => dst.Feilbruktyper, opt => opt.MapFrom(o => o.BeskyttelsesutstyrType.Feilbruktyper));

            CreateMap<HanskeSesjon, Modeller.V1.Sesjon.HanskeSesjon>(MemberList.None)
                .ForMember(dst => dst.Observasjoner, opt => opt.MapFrom(src => src.Observasjoner))
                .ForMember(dst => dst.Institusjonsnavn, opt => opt.MapFrom(src => src.Observator.Institusjon.Navn))
                .ForMember(dst => dst.InstitusjonId, opt => opt.MapFrom(src => src.Observator.Institusjon.Id));
            CreateMap<HanskeObservasjon, Modeller.V1.Observasjon.Hansker.HanskeObservasjon>(MemberList.None);
            CreateMap<HanskeMedIndikasjonType, Modeller.V1.Observasjon.Hansker.HanskeMedIndikasjonType>(MemberList
                .None);
            CreateMap<HanskeUtenIndikasjonType, Modeller.V1.Observasjon.Hansker.HanskeUtenIndikasjonType>(MemberList
                .None);
            CreateMap<HandhygieneEtterHanskebrukType, Modeller.V1.Observasjon.Hansker.HandhygieneEtterHanskebrukType>(
                MemberList.None);

            CreateMap<Domene.Sted.Institusjon, Modeller.V1.Institusjon.InstitusjonRapport>(MemberList.None);

            CreateMap<Domene.Sted.Institusjon, Modeller.V1.Oversikt.InstitusjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.AntallSesjoner, opt => opt.MapFrom(src => src.Avdelinger.Sum(x => x.Sesjoner.Count)));
            CreateMap<Domene.Sted.Avdeling, Modeller.V1.Oversikt.AvdelingOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.AntallSesjoner, opt => opt.MapFrom(src => src.Sesjoner.Count));

            CreateMap<Domene.Sesjon.Sesjon, SesjonRapport>(MemberList.None)
                .ForMember(dest => dest.Avdelingsnavn, opt => opt.MapFrom(src => src.Avdeling.Navn))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.Institusjonsnavn,
                    opt => opt.MapFrom(src => src.Observator.Institusjon.Navn));

            CreateMap<FireIndikasjonerSesjon, SesjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObservatorNavn, opt => opt.MapFrom(src => HentObservatorNavn(src.Observator)));

            CreateMap<FireIndikasjonerObservasjon, ObservasjonOversiktRapport>(MemberList.None);

            // Handsmykker
            CreateMap<HandsmykkeSesjon, SesjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObservatorNavn, opt => opt.MapFrom(src => HentObservatorNavn(src.Observator)));

            CreateMap<HandsmykkeObservasjon, ObservasjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.Handsmykketyper,
                    opt => opt.MapFrom(src => src.Handsmykker));

            // Beskyttelsesutstyr
            CreateMap<BeskyttelsesutstyrSesjon, SesjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObservatorNavn, opt => opt.MapFrom(src => HentObservatorNavn(src.Observator)));

            CreateMap<BeskyttelsesutstyrObservasjon, ObservasjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.Beskyttelsesutstyrsetting, opt => opt.MapFrom(src => src.Settingtype.Navn))
                .ForMember(dest => dest.Beskyttelsesutstyr, opt => opt.MapFrom(src => src.Beskyttelsesutstyrliste))
                .ForMember(dest => dest.BeskyttelsesutstyrObservasjon, opt => opt.MapFrom(src => src));
            

            CreateMap<Domene.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr, BeskyttelsesutstyrOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.Utstyrsnavn, opt => opt.MapFrom(src => src.Utstyrstype.Navn))
                .ForMember(dest => dest.Feilbruktyper, opt => opt.MapFrom(src => src.Feilbruktyper.Select(ft => ft.Navn)));

            // Hanske
            CreateMap<HanskeSesjon, SesjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => SesjonHelper.HentSesjonType(src.Discriminator)))
                .ForMember(dest => dest.ObservatorNavn, opt => opt.MapFrom(src => HentObservatorNavn(src.Observator)));

            CreateMap<HanskeObservasjon, ObservasjonOversiktRapport>(MemberList.None)
                .ForMember(dest => dest.HanskeObservasjon,
                    opt => opt.MapFrom(src => src));

            CreateMap<Domene.Sesjon.OverforingstatusType, OverforingstatusType>(MemberList.None);

            CreateMap<Domene.Sted.PredefinertKommentar, Modeller.V1.Institusjon.PredefinertKommentar>(MemberList.None);

            // Klinikk
            CreateMap<Domene.Sted.Klinikk, Modeller.V1.Institusjon.Klinikk>(MemberList.None)
                .ForMember(dest => dest.InstitusjonId, opt => opt.MapFrom(src => src.Institusjon.Id));

            //Region
            CreateMap<Domene.Sted.Region, Modeller.V1.Institusjon.Region>(MemberList.None);

            // Fire indikasjoner observasjon-rapport
            CreateMap<FireIndikasjonerObservasjon, FireIndikasjonerObservasjonRapport>(MemberList.None)
                .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Id))
                .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.FireIndikasjonerSesjon.Observator)))
                .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Opprettettidspunkt))
                .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.Registrerttidspunkt))
                .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Navn))
                .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Forkortelse))
                .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Navn))
                .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Avdelingtype.Navn))
                .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.Rolle.Navn))
                .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.FireIndikasjonerSesjon.Overforingstatus.Kode))
                .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Kommentar))
                .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.FireIndikasjonerSesjon.Kommentar))
                .ForMember(dest => dest.Aktivitet, opt => opt.MapFrom(o => o.Aktivitet.AktivitetType.Navn))
                .ForMember(dest => dest.Indikasjoner, opt => opt.MapFrom(o => string.Join(',', o.Indikasjonstyper.Select(i => i.Navn))))
                .ForMember(dest => dest.SekunderBrukt, opt => opt.MapFrom(o => o.Aktivitet.SekunderBrukt))
                .ForMember(dest => dest.TidtakingBleUtført, opt => opt.MapFrom(o => o.Aktivitet.TidtakingBleUtfort))
                .ForMember(dest => dest.BenyttetHanske, opt => opt.MapFrom(o => o.Aktivitet.BenyttetHanske))
                .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.FireIndikasjonerSesjon.Avdeling.Institusjon.Kommune.Navn));


            // Hanske-rapport

            CreateMap<HanskeObservasjon, HanskeObservasjonRapport>()
                .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.HanskeSesjon.Id))
                .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.HanskeSesjon.Observator)))
                .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.HanskeSesjon.Opprettettidspunkt))
                .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.Registrerttidspunkt))
                .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Navn))
                .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Forkortelse))
                .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Navn))
                .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Avdelingtype.Navn))
                .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.Rolle.Navn))
                .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.HanskeSesjon.Overforingstatus.Kode))
                .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Kommentar))
                .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.HanskeSesjon.Kommentar))
                .ForMember(dest => dest.HandhygieneEtterHanskebrukKode, opt => opt.MapFrom(o => o.HandhygieneEtterHanskebrukType.Kode))
                .ForMember(dest => dest.HanskeUtenIndikasjonKode, opt => opt.MapFrom(o => string.Join(',',o.HanskeUtenIndikasjonTyper.Select(h => h.Kode))))
                .ForMember(dest => dest.HanskeMedIndikasjonKode, opt => opt.MapFrom(o => string.Join(',', o.HanskeMedIndikasjonTyper.Select(h => h.Kode))))
                .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.HanskeSesjon.Avdeling.Institusjon.Kommune.Navn));

            // Handsmykke-rapport

            CreateMap<HandsmykkeObservasjon, HandsmykkeObservasjonRapport>()
               .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Id))
               .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.HandsmykkeSesjon.Observator)))
               .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Opprettettidspunkt))
               .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.Registrerttidspunkt))
               .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Institusjontype.Kode))
               .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Institusjontype.Navn))
               .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Navn))
               .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Forkortelse))
               .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Navn))
               .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Avdelingtype.Navn))
               .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.Rolle.Navn))
               .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.HandsmykkeSesjon.Overforingstatus.Kode))
               .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Kommentar))
               .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.HandsmykkeSesjon.Kommentar))
               .ForMember(dest => dest.HandsmykketypeKoder, opt => opt.MapFrom(o => string.Join(',', o.Handsmykker.Select(h => h.Kode))))
               .ForMember(dest => dest.Handsmykketyper, opt => opt.MapFrom(o => string.Join(',', o.Handsmykker.Select(h => h.Navn))))
               .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Helseforetak.Navn))
               .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
               .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Kommune.Nummer))
               .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.HandsmykkeSesjon.Avdeling.Institusjon.Kommune.Navn));

            // Beskyttelsesutstyr-rapport
            CreateMap<HyFive.Domene.Observasjon.Beskyttelsesutstyr.Beskyttelsesutstyr, BeskyttelsesutstyrObservasjonRapport>()
                .ForMember(dest => dest.ObservasjonId, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.Id))
                .ForMember(dest => dest.SesjonId, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Id))
                .ForMember(dest => dest.Observator, opt => opt.MapFrom(src => HentObservatorNavn(src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Observator)))
                .ForMember(dest => dest.SesjonOpprettettidspunkt, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Opprettettidspunkt))
                .ForMember(dest => dest.ObservasjonRegistrerttidspunkt, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.Registrerttidspunkt))
                .ForMember(dest => dest.InstitusjonstypeKode, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Institusjontype.Kode))
                .ForMember(dest => dest.Institusjonstype, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Institusjontype.Navn))
                .ForMember(dest => dest.Institusjon, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Navn))
                .ForMember(dest => dest.Institusjonsforkortelse, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Forkortelse))
                .ForMember(dest => dest.Avdeling, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Navn))
                .ForMember(dest => dest.Avdelingstype, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Avdelingtype.Navn))
                .ForMember(dest => dest.Rollenavn, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.Rolle.Navn))
                .ForMember(dest => dest.Overføringsstatus, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Overforingstatus.Kode))
                .ForMember(dest => dest.Observasjonskommentar, opt => opt.MapFrom(o => o.Kommentar))
                .ForMember(dest => dest.Sesjonskommentar, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Kommentar))
                .ForMember(dest => dest.BleBenyttet, opt => opt.MapFrom(o => o.BleBenyttet))
                .ForMember(dest => dest.BleBenyttetRiktig, opt => opt.MapFrom(o => o.BleBenyttetRiktig))
                .ForMember(dest => dest.ErIndikert, opt => opt.MapFrom(o => o.ErIndikert))
                .ForMember(dest => dest.BeskyttelsesutstyrKode, opt => opt.MapFrom(o => o.Utstyrstype.Kode))
                .ForMember(dest => dest.Beskyttelsesutstyr, opt => opt.MapFrom(o => o.Utstyrstype.Navn))
                .ForMember(dest => dest.BeskyttelsesutstyrsettingKode, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.Settingtype.Kode))
                .ForMember(dest => dest.Beskyttelsesutstyrsetting, opt => opt.MapFrom(o => o.BeskyttelsesutstyrObservasjon.Settingtype.Navn))
                .ForMember(dest => dest.Feilbruk, opt => opt.MapFrom(o => string.Join(',',o.Feilbruktyper.Select(f => f.Navn))))
                .ForMember(dest => dest.Helseforetak, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Helseforetak.Navn))
                .ForMember(dest => dest.RegionaltHelseforetak, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Helseforetak.RegionaltHelseforetak.Navn))
                .ForMember(dest => dest.Kommunenummer, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Kommune.Nummer))
                .ForMember(dest => dest.Kommune, opt => opt.MapFrom(src => src.BeskyttelsesutstyrObservasjon.BeskyttelsesutstyrSesjon.Avdeling.Institusjon.Kommune.Navn));

            CreateMap<Domene.Sted.Helseforetak, Modeller.V1.Institusjon.Helseforetak>()
                .ForMember(dest => dest.RegionaltHelseforetakId, opt => opt.MapFrom(src => src.RegionaltHelseforetak != null ? src.RegionaltHelseforetak.Id : 0));

            CreateMap<Domene.Sted.RegionaltHelseforetak, Modeller.V1.Institusjon.RegionaltHelseforetak>(MemberList.None);

            CreateMap<Domene.Sted.Kommune, Modeller.V1.Institusjon.Kommune>(MemberList.None);
        }

        private static string HentObservatorNavn(Observator observator)
        {
            return string.Join(" ", new[] { observator.Fornavn, observator.Etternavn });
        }
    }
}