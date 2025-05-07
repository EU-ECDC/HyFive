import { BrowserModule, HammerModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import { AppComponent } from './app.component';
import { MainMenuComponent } from './main-menu/main-menu.component';
import { StartsideForObservasjonComponent } from './startside/startside-for-observasjon.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { RegisterFourIndicationsComponent } from "./registrering/register-four-indications/register-four-indications.component";
import { FourIndicationsSessionService } from './services/data/four-indications-session.service';
import { RegisterActivityComponent } from './registrering/register-activity/register-activity.component';
import { MissedOpportunityComponent } from './registrering/missed-opportunity/missed-opportunity.component';
import { FourIndicationsObservationCardComponent } from './registrering/four-indications-observation-card/four-indications-observation-card.component';
import { IndicationSelectionComponent } from './registrering/indication-selection/indication-selection.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HandHygieneHammerJS } from '../hammerjsConfig';
import { RegisterCommentComponent } from './registrering/register-comment/register-comment.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NotSentSessionsComponent } from './sessions/ikke-sendte-sessions/ikke-sendte-sessions.component';
import { EditFourIndicationsObservationComponent } from './sessions/rediger-fire-indikasjoner-observasjon/rediger-fire-indikasjoner-observasjon.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FourIndicationsComponent } from './sessions/fire-indikasjoner/fire-indikasjoner.component';
import { DeleteConfirmationDialogComponent } from './sessions/slett-bekreftelsesdialog/slett-bekreftelsesdialog.component';
import { RegisterHandjewelryComponent } from './registrering/register-hand-jewelry/register-hand-jewelry.component';
import { SaveShadowComponent } from './registrering/save-shadow/save-shadow.component';
import { DeleteShadowComponent } from './registrering/delete-shadow/delete-shadow.component';
import { HandJewelrySessionService } from './services/data/hand-Jewelry-session.service';
import { HandJewelryObservationCardComponent } from './registrering/handjewelry-observation-card/handjewelry-observation-card.component';
import { HandJewelryComponent } from './sessions/handsmykker/handsmykker.component';
import { EditHandJewelryObservationComponent } from './sessions/rediger-handsmykker-observasjon/rediger-handsmykker-observasjon.component';
import { RoleSelectDropdownComponent } from './registrering/role-selection-dropdown/role-selection-dropdown.component';
import { SendteSesjonerComponent } from './sessions/sendte-sessions/sendte-sessions.component';
import { SessionOverviewComponent } from './sessions/sesjonsoversikt/sesjonsoversikt.component';
//import { FhiAccordionModule } from '@folkehelseinstituttet/ng-components';
import { RegisterProtectiveEquipmentComponent } from './registrering/register-protective-equipment/register-protective-equipment.component';
import { ProtectiveEquipmentObservationCardComponent } from './registrering/protective equipment-observation-card/protective equipment-observation-card.component';
import { SelectionForProtectiveEquipmentComponent } from './startside/selection-for-protective-equipment/selection-for-protective-equipment.component';
import { NavigationLinkComponent } from './shared/navigation-link/navigation-link.component';
import { ProtectiveEquipmentComponent } from './sessions/beskyttelsesutstyr/beskyttelsesutstyr.component';
import { AccordionComponent } from './shared/accordion/accordion.component';
import { SessionEditHeaderComponent } from './sessions/sesjonsredigering-overskrift/sesjonsredigering-overskrift.component';
import { ToastrModule } from 'ngx-toastr';
import { ProtectiveEquipmentModalComponent } from './registrering/protective-equipment-modal/protective-equipment-modal.component';
import { NewCardModalComponent } from './registrering/new-card-modal/new-card-modal.component';
import { SendteFireIndikasjonerSesjonComponent } from './sessions/sendte-sessions/sendte-fire-indikasjoner-sesjon/sendte-fire-indikasjoner-sesjon.component';
import { EditProtectiveEquipmentObservationComponent } from './sessions/rediger-beskyttelsesutstyr-observasjon/rediger-beskyttelsesutstyr-observasjon.component';
import { SendteSesjonsoversiktComponent } from './sessions/sendte-sessions/sendte-sesjonsoversikt/sendte-sesjonsoversikt.component';
import { SendteHandsmykkerSesjonComponent } from './sessions/sendte-sessions/sendte-handsmykker-sesjon/sendte-handsmykker-sesjon.component';
import { OfflineMessageComponent } from './shared/offline-message/offline-message.component';
import { SendteBeskyttelsesutstyrSesjonComponent } from './sessions/sendte-sessions/sendte-beskyttelsesutstyr-sesjon/sendte-beskyttelsesutstyr-sesjon.component';
import { SessionStatisticsComponent } from './sessions/fire-indikasjoner/sesjonstatistikk/sesjonstatistikk.component';
import { ToastrConfig } from './constants/toastr-config';
import { ObservationCounterComponent } from './shared/observation-counter/observation-counter.component';
import { InfoModalComponent } from './shared/info-modal/info-modal.component';
import { RegisterGloveComponent } from './registrering/register-glove/register-glove.component';
import { GloveObservationCardComponent } from './registrering/glove-observation-card/glove-observation-card.component';
import { GloveComponent } from './sessions/hanske/hanske.component';
import { SendteHanskeSesjonComponent } from './sessions/sendte-sessions/sendte-hanske-sesjon/sendte-hanske-sesjon.component';
import { EditGloveObservationComponent } from './sessions/rediger-hanske-observasjon/rediger-hanske-observasjon.component';
import { DialogModalComponent } from './shared/dialog-modal/dialog-modal.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { NewCardInfoComponent } from './shared/new-card-info/new-card-info.component';
import { LoginPageComponent } from './login-page/loginpage.component';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import {AppRoutingModule} from './app-routing.module';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import { AuthenticationFailedModalComponent } from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {DragDropModule} from "@angular/cdk/drag-drop";
import {HelpTextComponent} from "./shared/help-text/help-text.component";
import { HelpTextSettingsComponent } from './shared/help-text-setting/help-text-setting.component';
import { PseudonymComponent } from './shared/pseudonym-modal/pseudonym.component';


export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationFailedErrorInterceptor, multi: true },
];


@NgModule({
    declarations: [
        AppComponent,
        MainMenuComponent,
        StartsideForObservasjonComponent,
        RegisterFourIndicationsComponent,
        FourIndicationsObservationCardComponent,
        RegisterActivityComponent,
        MissedOpportunityComponent,
        IndicationSelectionComponent,
        RegisterCommentComponent,
        NotSentSessionsComponent,
        EditFourIndicationsObservationComponent,
        FourIndicationsComponent,
        DeleteConfirmationDialogComponent,
        RegisterHandjewelryComponent,
        HandJewelryObservationCardComponent,
        SaveShadowComponent,
        DeleteShadowComponent,
        HandJewelryComponent,
        EditHandJewelryObservationComponent,
        RoleSelectDropdownComponent,
        SendteSesjonerComponent,
        SessionOverviewComponent,
        RegisterProtectiveEquipmentComponent,
        ProtectiveEquipmentObservationCardComponent,
        SelectionForProtectiveEquipmentComponent,
        NavigationLinkComponent,
        ProtectiveEquipmentComponent,
        AccordionComponent,
        SessionEditHeaderComponent,
        EditProtectiveEquipmentObservationComponent,
        ProtectiveEquipmentModalComponent,
        NewCardModalComponent,
        SendteFireIndikasjonerSesjonComponent,
        SendteSesjonsoversiktComponent,
        OfflineMessageComponent,
        SendteHandsmykkerSesjonComponent,
        SendteBeskyttelsesutstyrSesjonComponent,
        SessionStatisticsComponent,
        ObservationCounterComponent,
        InfoModalComponent,
        RegisterGloveComponent,
        GloveObservationCardComponent,
        GloveComponent,
        SendteHanskeSesjonComponent,
        EditGloveObservationComponent,
        DialogModalComponent,
        NewCardInfoComponent,
        LoginPageComponent,
        SpinnerComponent,
        AuthenticationFailedModalComponent,
        HelpTextComponent,
        HelpTextSettingsComponent,
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
        EditProtectiveEquipmentObservationComponent,
        RegisterCommentComponent
    ]
})
export class AppModule {

}
