import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ForsideForAdministrasjonComponent } from './forside-for-administrasjon/forside-for-administrasjon.component';
import { RedigeringAvInstitusjonerComponent } from './fhiadmin/redigering-av-institusjoner/redigering-av-institusjoner.component';
import { RedigerEnInstitusjonComponent } from './fhiadmin/redigering-av-institusjoner/edit-an-institution/edit-an-institution.component';
import { OpprettInstitusjonComponent } from './fhiadmin/redigering-av-institusjoner/opprett-institusjon/opprett-institusjon.component';
import { RedigerObservatorerComponent } from './_felles/rediger-observatorer/rediger-observatorer.component';
import { ConfirmationDialogComponent } from './fhiadmin/redigering-av-institusjoner/confirmation-dialog/confirmation-dialog.component';
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
import { OverviewSessionsViewComponent } from './_felles/oversikt-sesjoner-visning/overview-sessions-view.component';
import { RedigeringAvPredefinertKommentarerComponent } from './koordinator/redigering-av-predefinert-kommentarer/redigering-av-predefinert-kommentarer.component';
import { RedigeringAvAvdelingstyperComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-avdelingstyper/redigering-av-avdelingstyper.component';
import { RedigeringAvKlinikkerComponent } from './koordinator/redigering-av-klinikker/redigering-av-klinikker.component';
import { OpprettKlinikkComponent } from './koordinator/redigering-av-klinikker/opprett-klinikk/opprett-klinikk.component';
import { RedigerEnKlinikkComponent } from './koordinator/redigering-av-klinikker/rediger-en-klinikk/rediger-en-klinikk.component';
import { RedigeringAvRegionComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-region/redigering-av-region.component';
import { RoleSelectionDropdownComponent } from './_felles/app-role-selection-dropdown/app-role-selection-dropdown.component';
import { RedigeringAvRollerComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-roles/redigering-av-roles.component';
import { OversiktFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/oversikt-fhiadmin.component';
import { RedigerFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/rediger-fhiadmin/rediger-fhiadmin.component';
import {AuthenticationFailedModalComponent} from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {EditFourIndicationsObservationsComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/edit-four-indications-observations.component";
import {EditHandjewelryObservationsComponent} from "./koordinator/redigering-av-observasjoner/edit-handjewelry-observations/edit-handjewelry-observations.component";
import {RedigerGloveObservationerComponent} from "./koordinator/redigering-av-observasjoner/rediger-hanske-observasjoner/rediger-hanske-observasjoner.component";
import {IndicationSelectionComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/indication-selection/indication-selection.component";
import {ActivityChoiceDropdownComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/activity-choice/activity-choice-dropdown.component";
import { RedigerBeskyttelsesutstyrObservasjonerComponent } from "./koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/rediger-beskyttelsesutstyr-observasjoner.component";
import { RedigerBeskyttelsesutstyrObservasjonComponent } from './koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/rediger-beskyttelsesutstyr-observasjon/rediger-beskyttelsesutstyr-observasjon.component';
import {BeskyttelsesutstyrModalComponent} from "./koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component";
import { SokHprnummerLenkeComponent } from './_felles/sok-hprnummer-lenke/sok-hprnummer-lenke.component';
import { EditSessionDataComponent } from './_felles/oversikt-sesjoner-visning/edit-sessionsdata/edit-sessionsdata.component';
import { ForesporselComponent } from "./koordinator/foresporsel/foresporsel.component";
import { HelseforetakComponent } from './fhiadmin/helseforetak/helseforetak.component';
import { RedigerKoordinatorerForHelseforetakComponent } from './koordinator/redigering-av-koordinatorer/rediger-koordinatorer-for-helseforetak.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { RedigeringAvKoordinatorerComponent } from './koordinator/redigering-av-koordinatorer/redigering-av-koordinatorer.component';
import { EditCoordinatorsComponent } from './_felles/edit-coordinators/editCoordinators.component';
import { PseudonymDialogComponent } from './_felles/edit-coordinators/pseudonym-dialog.component';
import { EmailComponent } from './fhiadmin/email/email.component';
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
    EditCoordinatorsComponent,
    ConfirmationDialogComponent,
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
    OverviewSessionsViewComponent,
    RedigeringAvPredefinertKommentarerComponent,
    RedigeringAvAvdelingstyperComponent,
    RedigeringAvKlinikkerComponent,
    OpprettKlinikkComponent,
    RedigerEnKlinikkComponent,
    RedigeringAvRegionComponent,
    RoleSelectionDropdownComponent,
    IndicationSelectionComponent,
    ActivityChoiceDropdownComponent,
    RedigeringAvRollerComponent,
    OversiktFhiAdminComponent,
    RedigerFhiAdminComponent,
    AuthenticationFailedModalComponent,
    EditFourIndicationsObservationsComponent,
    EditHandjewelryObservationsComponent,
    RedigerGloveObservationerComponent,
    RedigerBeskyttelsesutstyrObservasjonerComponent,
    RedigerBeskyttelsesutstyrObservasjonComponent,
    BeskyttelsesutstyrModalComponent,
    SokHprnummerLenkeComponent,
    EditSessionDataComponent,
    ForesporselComponent,
    HelseforetakComponent,
    RedigeringAvKoordinatorerComponent,
    RedigerKoordinatorerForHelseforetakComponent,
    PseudonymDialogComponent,
    EmailComponent,
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
