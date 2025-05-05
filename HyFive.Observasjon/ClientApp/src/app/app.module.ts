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
import { RegistrerAktivitetComponent } from './registrering/registrer-aktivitet/registrer-aktivitet.component';
import { MissedOpportunityComponent } from './registrering/missed-opportunity/missed-opportunity.component';
import { FireIndikasjonerObservasjonskortComponent } from './registrering/fire-indikasjoner-observasjonskort/fire-indikasjoner-observasjonskort.component';
import { IndikasjonsValgComponent } from './registrering/indikasjonsvalg/indikasjonsvalg.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HandHygieneHammerJS } from '../hammerjsConfig';
import { RegistrerKommentarComponent } from './registrering/registrer-kommentar/registrer-kommentar.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { IkkeSendteSesjonerComponent } from './sessions/ikke-sendte-sessions/ikke-sendte-sessions.component';
import { RedigerFireIndikasjonerObservasjonComponent } from './sessions/rediger-fire-indikasjoner-observasjon/rediger-fire-indikasjoner-observasjon.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FireIndikasjonerComponent } from './sessions/fire-indikasjoner/fire-indikasjoner.component';
import { SlettBekrefelsesdialogComponent } from './sessions/slett-bekreftelsesdialog/slett-bekreftelsesdialog.component';
import { RegistrereHandsmykkerComponent } from './registrering/registrere-handsmykker/registrere-handsmykker.component';
import { LagreSkyggeComponent } from './registrering/lagre-skygge/lagre-skygge.component';
import { SlettSkyggeComponent } from './registrering/slett-skygge/slett-skygge.component';
import { HandsmykkeSesjonService } from './services/data/handsmykke-sesjon.service';
import { HandsmykkerObservasjonskortComponent } from './registrering/handsmykker-observasjonskort/handsmykker-observasjonskort.component';
import { HandsmykkerComponent } from './sessions/handsmykker/handsmykker.component';
import { RedigerHandsmykkerObservasjonComponent } from './sessions/rediger-handsmykker-observasjon/rediger-handsmykker-observasjon.component';
import { RolleValgDropdownComponent } from './registrering/rolle-valg-dropdown/rolle-valg-dropdown.component';
import { SendteSesjonerComponent } from './sessions/sendte-sessions/sendte-sessions.component';
import { SesjonsoversiktComponent } from './sessions/sesjonsoversikt/sesjonsoversikt.component';
//import { FhiAccordionModule } from '@folkehelseinstituttet/ng-components';
import { RegistrereBeskyttelsesutstyrComponent } from './registrering/registrere-beskyttelsesutstyr/registrere-beskyttelsesutstyr.component';
import { BeskyttelsesutstyrObservasjonskortComponent } from './registrering/beskyttelsesutstyr-observasjonskort/beskyttelsesutstyr-observasjonskort.component';
import { ValgForBeskyttelsesutstyrComponent } from './startside/valg-for-beskyttelsesutstyr/valg-for-beskyttelsesutstyr.component';
import { NavigasjonslenkeComponent } from './shared/navigasjonslenke/navigasjonslenke.component';
import { BeskyttelsesutstyrComponent } from './sessions/beskyttelsesutstyr/beskyttelsesutstyr.component';
import { AccordionComponent } from './shared/accordion/accordion.component';
import { SesjonsredigeringOverskriftComponent } from './sessions/sesjonsredigering-overskrift/sesjonsredigering-overskrift.component';
import { ToastrModule } from 'ngx-toastr';
import { BeskyttelsesutstyrModalComponent } from './registrering/beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component';
import { NyttKortModalComponent } from './registrering/nytt-kort-modal/nytt-kort-modal.component';
import { SendteFireIndikasjonerSesjonComponent } from './sessions/sendte-sessions/sendte-fire-indikasjoner-sesjon/sendte-fire-indikasjoner-sesjon.component';
import { RedigerBeskyttelsesutstyrObservasjonComponent } from './sessions/rediger-beskyttelsesutstyr-observasjon/rediger-beskyttelsesutstyr-observasjon.component';
import { SendteSesjonsoversiktComponent } from './sessions/sendte-sessions/sendte-sesjonsoversikt/sendte-sesjonsoversikt.component';
import { SendteHandsmykkerSesjonComponent } from './sessions/sendte-sessions/sendte-handsmykker-sesjon/sendte-handsmykker-sesjon.component';
import { OfflineMeldingComponent } from './shared/offline-melding/offline-melding.component';
import { SendteBeskyttelsesutstyrSesjonComponent } from './sessions/sendte-sessions/sendte-beskyttelsesutstyr-sesjon/sendte-beskyttelsesutstyr-sesjon.component';
import { SesjonstatistikkComponent } from './sessions/fire-indikasjoner/sesjonstatistikk/sesjonstatistikk.component';
import { ToastrConfig } from './constants/toastr-config';
import { ObservasjonstellerComponent } from './shared/observasjonsteller/observasjonsteller.component';
import { InfoModalComponent } from './shared/info-modal/info-modal.component';
import { RegistrereHanskeComponent } from './registrering/registrere-hanske/registrere-hanske.component';
import { HanskeObservasjonskortComponent } from './registrering/hanske-observasjonskort/hanske-observasjonskort.component';
import { HanskeComponent } from './sessions/hanske/hanske.component';
import { SendteHanskeSesjonComponent } from './sessions/sendte-sessions/sendte-hanske-sesjon/sendte-hanske-sesjon.component';
import { RedigerHanskeObservasjonComponent } from './sessions/rediger-hanske-observasjon/rediger-hanske-observasjon.component';
import { DialogModalComponent } from './shared/dialog-modal/dialog-modal.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { NyttKortInfoComponent } from './shared/nytt-kort-info/nytt-kort-info.component';
import { LoginsideComponent } from './loginside/loginside.component';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import {AppRoutingModule} from './app-routing.module';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import { AuthenticationFailedModalComponent } from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {DragDropModule} from "@angular/cdk/drag-drop";
import {HjelpetekstComponent} from "./shared/hjelpetekst/hjelpetekst.component";
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
        RegistrerAktivitetComponent,
        MissedOpportunityComponent,
        IndikasjonsValgComponent,
        RegistrerKommentarComponent,
        IkkeSendteSesjonerComponent,
        RedigerFireIndikasjonerObservasjonComponent,
        FireIndikasjonerComponent,
        SlettBekrefelsesdialogComponent,
        RegistrereHandsmykkerComponent,
        HandsmykkerObservasjonskortComponent,
        LagreSkyggeComponent,
        SlettSkyggeComponent,
        HandsmykkerComponent,
        RedigerHandsmykkerObservasjonComponent,
        RolleValgDropdownComponent,
        SendteSesjonerComponent,
        SesjonsoversiktComponent,
        RegistrereBeskyttelsesutstyrComponent,
        BeskyttelsesutstyrObservasjonskortComponent,
        ValgForBeskyttelsesutstyrComponent,
        NavigasjonslenkeComponent,
        BeskyttelsesutstyrComponent,
        AccordionComponent,
        SesjonsredigeringOverskriftComponent,
        RedigerBeskyttelsesutstyrObservasjonComponent,
        BeskyttelsesutstyrModalComponent,
        NyttKortModalComponent,
        SendteFireIndikasjonerSesjonComponent,
        SendteSesjonsoversiktComponent,
        OfflineMeldingComponent,
        SendteHandsmykkerSesjonComponent,
        SendteBeskyttelsesutstyrSesjonComponent,
        SesjonstatistikkComponent,
        ObservasjonstellerComponent,
        InfoModalComponent,
        RegistrereHanskeComponent,
        HanskeObservasjonskortComponent,
        HanskeComponent,
        SendteHanskeSesjonComponent,
        RedigerHanskeObservasjonComponent,
        DialogModalComponent,
        NyttKortInfoComponent,
        LoginsideComponent,
        SpinnerComponent,
        AuthenticationFailedModalComponent,
        HjelpetekstComponent,
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
    providers: [FourIndicationsSessionService, HandsmykkeSesjonService, HandHygieneHammerJS, httpInterceptorProviders],
    exports: [
        RedigerBeskyttelsesutstyrObservasjonComponent,
        RegistrerKommentarComponent
    ]
})
export class AppModule {

}
