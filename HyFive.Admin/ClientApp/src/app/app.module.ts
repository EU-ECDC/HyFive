import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ForsideForAdministrasjonComponent } from './forside-for-administrasjon/forside-for-administrasjon.component';
import { RedigeringAvInstitusjonerComponent } from './fhiadmin/redigering-av-institusjoner/redigering-av-institusjoner.component';
import { RedigerEnInstitusjonComponent } from './fhiadmin/redigering-av-institusjoner/rediger-en-institusjon/rediger-en-institusjon.component';
import { OpprettInstitusjonComponent } from './fhiadmin/redigering-av-institusjoner/opprett-institusjon/opprett-institusjon.component';
import { RedigerObservatorerComponent } from './_felles/rediger-observatorer/rediger-observatorer.component';
import { BekrefelsesdialogComponent } from './fhiadmin/redigering-av-institusjoner/bekreftelsesdialog/bekreftelsesdialog.component';
import { RedigeringAvIndikasjonstyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-indikasjonstyper/redigering-av-indikasjonstyper.component';
import { RedigeringAvHandsmykketypeComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-handsmykketype/redigering-av-handsmykketype.component';
import { RedigeringAvInstitusjonstyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-institusjonstyper/redigering-av-institusjonstyper.component';
import { RedigeringAvKodeverkComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-kodeverk.component';
import { RedigeringAvAvdelingerComponent } from './_felles/redigering-av-avdelinger/redigering-av-avdelinger.component';
import { OpprettAvdelingComponent } from './_felles/redigering-av-avdelinger/opprett-avdeling/opprett-avdeling.component';
import { RedigeringAvAktivitettypeComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-aktivitettype/redigering-av-aktivitettype.component';
import { RedigeringAvBeskyttelsesutstyrtyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/redigering-av-beskyttelsesutstyrtyper.component';
import { RedigeringAvFeilbruktyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/redigering-av-feilbruktyper/redigering-av-feilbruktyper.component';
import { RedigeringAvBeskyttelsesutstyrsettingtyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrsettingtyper/redigering-av-beskyttelsesutstyrsettingtyper.component';
import { OversiktObservasjonerComponent } from './fhiadmin/oversikt-observasjoner/oversikt-observasjoner.component';
import { DatePipe } from '@angular/common';
import { OversiktAvdelingSesjonerComponent } from './fhiadmin/oversikt-observasjoner/oversikt-avdeling-sesjoner/oversikt-avdeling-sesjoner.component';
import { RedigeringAvObservatorerComponent } from './koordinator/redigering-av-observatorer/redigering-av-observatorer.component';
import { ProfilsideComponent } from './profilside/profilside.component';
import { ClipboardModule } from 'ngx-clipboard';
import { RedigeringAvHanskemedindikasjontyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-hanskemedindikasjontyper/redigering-av-hanskemedindikasjontyper.component';
import { RedigeringAvHanskeutenindikasjontyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-hanskeutenindikasjontyper/redigering-av-hanskeutenindikasjontyper.component';
import { RedigeringAvHandhygieneetterhanskebruktyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-handhygieneetterhanskebruktyper/redigering-av-handhygieneetterhanskebruktyper.component';
import { OverforSesjonerComponent } from './koordinator/overfor-sesjoner/overfor-sesjoner.component';
import { OversiktSesjonerVisningComponent } from './_felles/oversikt-sesjoner-visning/oversikt-sesjoner-visning.component';
import { RedigeringAvPredefinertKommentarerComponent } from './koordinator/redigering-av-predefinert-kommentarer/redigering-av-predefinert-kommentarer.component';
import { RedigeringAvAvdelingstyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-avdelingstyper/redigering-av-avdelingstyper.component';
import { RedigeringAvKlinikkerComponent } from './koordinator/redigering-av-klinikker/redigering-av-klinikker.component';
import { OpprettKlinikkComponent } from './koordinator/redigering-av-klinikker/opprett-klinikk/opprett-klinikk.component';
import { RedigerEnKlinikkComponent } from './koordinator/redigering-av-klinikker/rediger-en-klinikk/rediger-en-klinikk.component';
import { RedigeringAvRegionComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-region/redigering-av-region.component';
import { RolleValgDropdownComponent } from './_felles/rolle-valg-dropdown/rolle-valg-dropdown.component';
import { RedigeringAvRollerComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-roller/redigering-av-roller.component';
import { OversiktFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/oversikt-fhiadmin.component';
import { RedigerFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/rediger-fhiadmin/rediger-fhiadmin.component';
import {AuthenticationFailedModalComponent} from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {RedigerFireIndikasjonerObservasjonerComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/rediger-fire-indikasjoner-observasjoner.component";
import {RedigerHandsmykkeObservasjonerComponent} from "./koordinator/redigering-av-observasjoner/rediger-handsmykke-observasjoner/rediger-handsmykke-observasjoner.component";
import {RedigerHanskeObservasjonerComponent} from "./koordinator/redigering-av-observasjoner/rediger-hanske-observasjoner/rediger-hanske-observasjoner.component";
import {IndikasjonsValgComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/indikasjonsvalg/indikasjonsvalg.component";
import {AktivitetValgDropdownComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/aktivitetvalg/aktivitet-valg-dropdown.component";
import { RedigerBeskyttelsesutstyrObservasjonerComponent } from "./koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/rediger-beskyttelsesutstyr-observasjoner.component";
import { RedigerBeskyttelsesutstyrObservasjonComponent } from './koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/rediger-beskyttelsesutstyr-observasjon/rediger-beskyttelsesutstyr-observasjon.component';
import {BeskyttelsesutstyrModalComponent} from "./koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component";
import { SokHprnummerLenkeComponent } from './_felles/sok-hprnummer-lenke/sok-hprnummer-lenke.component';
import { RedigerSesjonsdataComponent } from './_felles/oversikt-sesjoner-visning/rediger-sesjonsdata/rediger-sesjonsdata.component';
import { ForesporselComponent } from "./koordinator/foresporsel/foresporsel.component";
import { HelseforetakComponent } from './fhiadmin/helseforetak/helseforetak.component';
import { RedigerKoordinatorerForHelseforetakComponent } from './koordinator/redigering-av-koordinatorer/rediger-koordinatorer-for-helseforetak.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { RedigeringAvKoordinatorerComponent } from './koordinator/redigering-av-koordinatorer/redigering-av-koordinatorer.component';
import { RedigerKoordinatorerComponent } from './_felles/rediger-koordinatorer/rediger-koordinatorer.component';
import { PseudonymDialogComponent } from './_felles/rediger-koordinatorer/pseudonym-dialog.component';
import { EpostComponent } from './fhiadmin/epost/epost.component';
import { FhiAngularComponentsModule, FhiMultiselectComponent } from '@folkehelseinstituttet/angular-components';
import { FhiAngularHighchartsModule } from '@folkehelseinstituttet/angular-highcharts';
import { RapporterComponent } from './koordinator/rapporter/rapporter.component';
import { EtterlevelseComponent } from './koordinator/rapporter/fireIndikasjoner/etterlevelse/etterlevelse.component';
import { SortableColumnComponent } from './shared/sorting/sortable-column.component';
import { SortableTableDirective } from './shared/sorting/sortable-table.directive';
import { SortService } from './shared/sorting/sort.service';
import { NedlastingExcelComponent } from './koordinator/rapporter/nedlasting/nedlasting-excel.component';
import { EtterlevelseFireIndikasjonerPdfComponent } from './koordinator/rapporter/predefinerte/etterlevelse-fire-indikasjoner-pdf.component';
import { EtterlevelseHandsmykkerPdfComponent } from './koordinator/rapporter/predefinerte/etterlevelse-handsmykker-pdf.component';
import { EtterlevelsePdfComponent } from './koordinator/rapporter/felles/etterlevelse-pdf.component';

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationFailedErrorInterceptor, multi: true },
];

@NgModule({
  declarations: [
    AppComponent,
    ForsideForAdministrasjonComponent,
    RedigeringAvInstitusjonerComponent,
    RedigerEnInstitusjonComponent,
    OpprettInstitusjonComponent,
    RedigerObservatorerComponent,
    RedigeringAvHandsmykketypeComponent,
    RedigerKoordinatorerComponent,
    BekrefelsesdialogComponent,
    RedigeringAvKodeverkComponent,
    RedigeringAvIndikasjonstyperComponent,
    RedigeringAvAktivitettypeComponent,
    RedigeringAvInstitusjonstyperComponent,
    RedigeringAvAvdelingerComponent,
    OpprettAvdelingComponent,
    RedigeringAvBeskyttelsesutstyrtyperComponent,
    RedigeringAvFeilbruktyperComponent,
    RedigeringAvBeskyttelsesutstyrsettingtyperComponent,
    OversiktObservasjonerComponent,
    OversiktAvdelingSesjonerComponent,
    RedigeringAvObservatorerComponent,
    ProfilsideComponent,
    RedigeringAvHanskemedindikasjontyperComponent,
    RedigeringAvHanskeutenindikasjontyperComponent,
    RedigeringAvHandhygieneetterhanskebruktyperComponent,
    OverforSesjonerComponent,
    OversiktSesjonerVisningComponent,
    RedigeringAvPredefinertKommentarerComponent,
    RedigeringAvAvdelingstyperComponent,
    RedigeringAvKlinikkerComponent,
    OpprettKlinikkComponent,
    RedigerEnKlinikkComponent,
    RedigeringAvRegionComponent,
    RolleValgDropdownComponent,
    IndikasjonsValgComponent,
    AktivitetValgDropdownComponent,
    RedigeringAvRollerComponent,
    OversiktFhiAdminComponent,
    RedigerFhiAdminComponent,
    AuthenticationFailedModalComponent,
    RedigerFireIndikasjonerObservasjonerComponent,
    RedigerHandsmykkeObservasjonerComponent,
    RedigerHanskeObservasjonerComponent,
    RedigerBeskyttelsesutstyrObservasjonerComponent,
    RedigerBeskyttelsesutstyrObservasjonComponent,
    BeskyttelsesutstyrModalComponent,
    SokHprnummerLenkeComponent,
    RedigerSesjonsdataComponent,
    ForesporselComponent,
    HelseforetakComponent,
    RedigeringAvKoordinatorerComponent,
    RedigerKoordinatorerForHelseforetakComponent,
    PseudonymDialogComponent,
    EpostComponent,
    RapporterComponent,
    EtterlevelseComponent,
    SortableColumnComponent,
    SortableTableDirective,
    NedlastingExcelComponent,
    EtterlevelseFireIndikasjonerPdfComponent,
    EtterlevelseHandsmykkerPdfComponent,
    EtterlevelsePdfComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
    ClipboardModule,
    NgbModule,
    NgMultiSelectDropDownModule.forRoot(),
    FhiAngularComponentsModule,
    FhiAngularHighchartsModule
  ],
  bootstrap: [AppComponent],
  providers: [DatePipe, httpInterceptorProviders, SortService]
})
export class AppModule { }
