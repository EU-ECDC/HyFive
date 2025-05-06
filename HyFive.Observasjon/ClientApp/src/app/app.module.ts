import { BrowserModule, HammerModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import { AppComponent } from './app.component';
import { MainMenuComponent } from './main-menu/main-menu.component';
import { StartsideForObservasjonComponent } from './startside/startside-for-observasjon.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { RegistrereFireIndikasjonerComponent } from "./registrering/registrere-fire-indikasjoner/registrere-fire-indikasjoner.component";
import { FourIndicationsSessionService } from './services/data/four-indications-session.service';
import { RegisterActivityComponent } from './registrering/register-activity/register-activity.component';
import { MissedOpportunityComponent } from './registrering/missed-opportunity/missed-opportunity.component';
import { FireIndikasjonerObservasjonskortComponent } from './registrering/fire-indikasjoner-observasjonskort/fire-indikasjoner-observasjonskort.component';
import { IndikasjonsValgComponent } from './registrering/indikasjonsvalg/indikasjonsvalg.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HandHygieneHammerJS } from '../hammerjsConfig';
import { RegisterCommentComponent } from './registrering/register-comment/register-comment.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { IkkeSendteSesjonerComponent } from './sessions/ikke-sendte-sessions/ikke-sendte-sessions.component';
import { RedigerFireIndikasjonerObservasjonComponent } from './sessions/rediger-fire-indikasjoner-observasjon/rediger-fire-indikasjoner-observasjon.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FireIndikasjonerComponent } from './sessions/fire-indikasjoner/fire-indikasjoner.component';
import { SlettBekrefelsesdialogComponent } from './sessions/slett-bekreftelsesdialog/slett-bekreftelsesdialog.component';
import { RegistrereHandsmykkerComponent } from './registrering/registrere-handsmykker/registrere-handsmykker.component';
import { SaveShadowComponent } from './registrering/save-shadow/save-shadow.component';
import { DeleteShadowComponent } from './registrering/delete-shadow/delete-shadow.component';
import { HandJewelrySessionService } from './services/data/hand-Jewelry-session.service';
import { HandsmykkerObservasjonskortComponent } from './registrering/handsmykker-observasjonskort/handsmykker-observasjonskort.component';
import { HandsmykkerComponent } from './sessions/handsmykker/handsmykker.component';
import { RedigerHandsmykkerObservasjonComponent } from './sessions/rediger-handsmykker-observasjon/rediger-handsmykker-observasjon.component';
import { RoleSelectDropdownComponent } from './registrering/role-selection-dropdown/role-selection-dropdown.component';
import { SendteSesjonerComponent } from './sessions/sendte-sessions/sendte-sessions.component';
import { SesjonsoversiktComponent } from './sessions/sesjonsoversikt/sesjonsoversikt.component';
//import { FhiAccordionModule } from '@folkehelseinstituttet/ng-components';
import { RegisterProtectiveEquipmentComponent } from './registrering/register-protective-equipment/register-protective-equipment.component';
import { ProtectiveEquipmentObservationCardComponent } from './registrering/protective equipment-observation-card/protective equipment-observation-card.component';
import { ValgForBeskyttelsesutstyrComponent } from './startside/valg-for-beskyttelsesutstyr/valg-for-beskyttelsesutstyr.component';
import { NavigationLinkComponent } from './shared/navigation-link/navigation-link.component';
import { BeskyttelsesutstyrComponent } from './sessions/beskyttelsesutstyr/beskyttelsesutstyr.component';
import { AccordionComponent } from './shared/accordion/accordion.component';
import { SesjonsredigeringOverskriftComponent } from './sessions/sesjonsredigering-overskrift/sesjonsredigering-overskrift.component';
import { ToastrModule } from 'ngx-toastr';
import { ProtectiveEquipmentModalComponent } from './registrering/protective-equipment-modal/protective-equipment-modal.component';
import { NyttKortModalComponent } from './registrering/nytt-kort-modal/nytt-kort-modal.component';
import { SendteFireIndikasjonerSesjonComponent } from './sessions/sendte-sessions/sendte-fire-indikasjoner-sesjon/sendte-fire-indikasjoner-sesjon.component';
import { RedigerBeskyttelsesutstyrObservasjonComponent } from './sessions/rediger-beskyttelsesutstyr-observasjon/rediger-beskyttelsesutstyr-observasjon.component';
import { SendteSesjonsoversiktComponent } from './sessions/sendte-sessions/sendte-sesjonsoversikt/sendte-sesjonsoversikt.component';
import { SendteHandsmykkerSesjonComponent } from './sessions/sendte-sessions/sendte-handsmykker-sesjon/sendte-handsmykker-sesjon.component';
import { OfflineMeldingComponent } from './shared/offline-melding/offline-melding.component';
import { SendteBeskyttelsesutstyrSesjonComponent } from './sessions/sendte-sessions/sendte-beskyttelsesutstyr-sesjon/sendte-beskyttelsesutstyr-sesjon.component';
import { SesjonstatistikkComponent } from './sessions/fire-indikasjoner/sesjonstatistikk/sesjonstatistikk.component';
import { ToastrConfig } from './constants/toastr-config';
import { ObservationCounterComponent } from './shared/observation-counter/observation-counter.component';
import { InfoModalComponent } from './shared/info-modal/info-modal.component';
import { RegistrereHanskeComponent } from './registrering/registrere-hanske/registrere-hanske.component';
import { HanskeObservasjonskortComponent } from './registrering/hanske-observasjonskort/hanske-observasjonskort.component';
import { HanskeComponent } from './sessions/hanske/hanske.component';
import { SendteHanskeSesjonComponent } from './sessions/sendte-sessions/sendte-hanske-sesjon/sendte-hanske-sesjon.component';
import { RedigerHanskeObservasjonComponent } from './sessions/rediger-hanske-observasjon/rediger-hanske-observasjon.component';
import { DialogModalComponent } from './shared/dialog-modal/dialog-modal.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { NyttKortInfoComponent } from './shared/nytt-kort-info/nytt-kort-info.component';
import { LoginPageComponent } from './login-page/loginpage.component';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import {AppRoutingModule} from './app-routing.module';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import { AuthenticationFailedModalComponent } from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {DragDropModule} from "@angular/cdk/drag-drop";
import {HelpTextComponent} from "./shared/help-text/help-text.component";
import { HjelpetekstInnstillingerComponent } from './shared/hjelpetekst-innstillinger/hjelpetekst-innstillinger.component';
import { PseudonymComponent } from './shared/pseudonym-modal/pseudonym.component';


export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationFailedErrorInterceptor, multi: true },
];


@NgModule({
    declarations: [
        AppComponent,
        MainMenuComponent,
        StartsideForObservasjonComponent,
        RegistrereFireIndikasjonerComponent,
        FireIndikasjonerObservasjonskortComponent,
        RegisterActivityComponent,
        MissedOpportunityComponent,
        IndikasjonsValgComponent,
        RegisterCommentComponent,
        IkkeSendteSesjonerComponent,
        RedigerFireIndikasjonerObservasjonComponent,
        FireIndikasjonerComponent,
        SlettBekrefelsesdialogComponent,
        RegistrereHandsmykkerComponent,
        HandsmykkerObservasjonskortComponent,
        SaveShadowComponent,
        DeleteShadowComponent,
        HandsmykkerComponent,
        RedigerHandsmykkerObservasjonComponent,
        RoleSelectDropdownComponent,
        SendteSesjonerComponent,
        SesjonsoversiktComponent,
        RegisterProtectiveEquipmentComponent,
        ProtectiveEquipmentObservationCardComponent,
        ValgForBeskyttelsesutstyrComponent,
        NavigationLinkComponent,
        BeskyttelsesutstyrComponent,
        AccordionComponent,
        SesjonsredigeringOverskriftComponent,
        RedigerBeskyttelsesutstyrObservasjonComponent,
        ProtectiveEquipmentModalComponent,
        NyttKortModalComponent,
        SendteFireIndikasjonerSesjonComponent,
        SendteSesjonsoversiktComponent,
        OfflineMeldingComponent,
        SendteHandsmykkerSesjonComponent,
        SendteBeskyttelsesutstyrSesjonComponent,
        SesjonstatistikkComponent,
        ObservationCounterComponent,
        InfoModalComponent,
        RegistrereHanskeComponent,
        HanskeObservasjonskortComponent,
        HanskeComponent,
        SendteHanskeSesjonComponent,
        RedigerHanskeObservasjonComponent,
        DialogModalComponent,
        NyttKortInfoComponent,
        LoginPageComponent,
        SpinnerComponent,
        AuthenticationFailedModalComponent,
        HelpTextComponent,
        HjelpetekstInnstillingerComponent,
        PseudonymComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        AppRoutingModule,
        HttpClientModule,
        FormsModule,
        ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
        HammerModule,
        BrowserAnimationsModule,
        NgbModule,
        ToastrModule.forRoot(ToastrConfig.toastrConfig),
        FontAwesomeModule,
        //FhiAccordionModule,
        NgSelectModule,
        DragDropModule
    ],
    bootstrap: [AppComponent],
    providers: [FourIndicationsSessionService, HandJewelrySessionService, HandHygieneHammerJS, httpInterceptorProviders],
    exports: [
        RedigerBeskyttelsesutstyrObservasjonComponent,
        RegisterCommentComponent
    ]
})
export class AppModule {

}
