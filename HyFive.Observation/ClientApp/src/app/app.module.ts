import { BrowserModule, HammerModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import { AppComponent } from './app.component';
import { MainMenuComponent } from './main-menu/main-menu.component';
import { HomePageForObservationComponent } from './startside/home-page-observation.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { RegisterFiveIndicationsComponent } from "./registrering/register-five-indications/register-five-indications.component";
import { FiveIndicationsSessionService } from './services/data/five-indications-session.service';
import { RegisterActivityComponent } from './registrering/register-activity/register-activity.component';
import { MissedOpportunityComponent } from './registrering/missed-opportunity/missed-opportunity.component';
import { FiveIndicationsObservationCardComponent } from './registrering/five-indications-observation-card/five-indications-observation-card.component';
import { IndicationSelectionComponent } from './registrering/indication-selection/indication-selection.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HandHygieneHammerJS } from '../hammerjsConfig';
import { RegisterCommentComponent } from './registrering/register-comment/register-comment.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { RegisterHandjewelryComponent } from './registrering/register-hand-jewelry/register-hand-jewelry.component';
import { SaveShadowComponent } from './registrering/save-shadow/save-shadow.component';
import { DeleteShadowComponent } from './registrering/delete-shadow/delete-shadow.component';
import { HandJewelrySessionService } from './services/data/hand-Jewelry-session.service';
import { HandJewelryObservationCardComponent } from './registrering/handjewelry-observation-card/handjewelry-observation-card.component';
import { RoleSelectDropdownComponent } from './registrering/role-selection-dropdown/role-selection-dropdown.component';
//import { FhiAccordionModule } from '@folkehelseinstituttet/ng-components';
import { RegisterProtectiveEquipmentComponent } from './registrering/register-protective-equipment/register-protective-equipment.component';
import { ProtectiveEquipmentObservationCardComponent } from './registrering/protective equipment-observation-card/protective equipment-observation-card.component';
import { SelectionForProtectiveEquipmentComponent } from './startside/selection-for-protective-equipment/selection-for-protective-equipment.component';
import { NavigationLinkComponent } from './shared/navigation-link/navigation-link.component';
import { AccordionComponent } from './shared/accordion/accordion.component';
import { ToastrModule } from 'ngx-toastr';
import { ProtectiveEquipmentModalComponent } from './registrering/protective-equipment-modal/protective-equipment-modal.component';
import { NewCardModalComponent } from './registrering/new-card-modal/new-card-modal.component';
import { OfflineMessageComponent } from './shared/offline-message/offline-message.component';
import { ToastrConfig } from './constants/toastr-config';
import { ObservationCounterComponent } from './shared/observation-counter/observation-counter.component';
import { InfoModalComponent } from './shared/info-modal/info-modal.component';
import { RegisterGloveComponent } from './registrering/register-glove/register-glove.component';
import { GloveObservationCardComponent } from './registrering/glove-observation-card/glove-observation-card.component';
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
import { NotSentSessionsComponent } from './sesjoner/not-sent-sessions/not-sent-sessions.component';
import { EditFiveIndicationsObservationComponent } from './sesjoner/edit-five-indications-observation/edit-five-indications-observation.component';
import { FiveIndicationsComponent } from './sesjoner/fire-indikasjoner/five-indications.component';
import { DeleteConfirmationDialogComponent } from './sesjoner/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { HandJewelryComponent } from './sesjoner/handJewelry/handJewelry.component';
import { EditHandJewelryObservationComponent } from './sesjoner/edit-hand-jewelry-observation/edit-hand-jewelry-observation.component';
import { SessionOverviewComponent } from './sesjoner/session-overview/session-overview.component';
import { SentSessionsComponent } from './sesjoner/sent-sessions/sent-sessions.component';
import { ProtectiveEquipmentComponent } from './sesjoner/protection-equipment/protection-equipment.component';
import { SessionEditHeaderComponent } from './sesjoner/session-edit-header/session-edit-header.component';
import { SentFiveIndicationsSessionComponent } from './sesjoner/sent-sessions/sent-five-indications-session/sent-five-indications-session.component';
import { EditProtectiveEquipmentObservationComponent } from './sesjoner/edit-protective-equipment-observation/edit-protective-equipment-observation.component';
import { SentSessionOverviewComponent } from './sesjoner/sent-sessions/sent-session-overview/sent-session-overview.component';
import { SentHandJewelrySessionComponent } from './sesjoner/sent-sessions/sent-hand-jewelry-session/sent-hand-jewelry-session.component';
import { SentProtectiveEquipmentSessionComponent } from './sesjoner/sent-sessions/sent-protective-equipment-session/sent-protective-equipment-session.component';
import { SessionStatisticsComponent } from './sesjoner/fire-indikasjoner/session-statistics/session-statistics.component';
import { GloveComponent } from './sesjoner/glove/glove.component';
import { SentGloveSessionComponent } from './sesjoner/sent-sessions/sent-glove-session/sent-glove-session.component';
import { EditGloveObservationComponent } from './sesjoner/edit-glove-observation/edit-glove-observation.component';


export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationFailedErrorInterceptor, multi: true },
];


@NgModule({
    declarations: [
        AppComponent,
        MainMenuComponent,
        HomePageForObservationComponent,
        RegisterFiveIndicationsComponent,
        FiveIndicationsObservationCardComponent,
        RegisterActivityComponent,
        MissedOpportunityComponent,
        IndicationSelectionComponent,
        RegisterCommentComponent,
        NotSentSessionsComponent,
        EditFiveIndicationsObservationComponent,
        FiveIndicationsComponent,
        DeleteConfirmationDialogComponent,
        RegisterHandjewelryComponent,
        HandJewelryObservationCardComponent,
        SaveShadowComponent,
        DeleteShadowComponent,
        HandJewelryComponent,
        EditHandJewelryObservationComponent,
        RoleSelectDropdownComponent,
        SentSessionsComponent,
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
        SentFiveIndicationsSessionComponent,
        SentSessionOverviewComponent,
        OfflineMessageComponent,
        SentHandJewelrySessionComponent,
        SentProtectiveEquipmentSessionComponent,
        SessionStatisticsComponent,
        ObservationCounterComponent,
        InfoModalComponent,
        RegisterGloveComponent,
        GloveObservationCardComponent,
        GloveComponent,
        SentGloveSessionComponent,
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
    providers: [FiveIndicationsSessionService, HandJewelrySessionService, HandHygieneHammerJS, httpInterceptorProviders],
    exports: [
        EditProtectiveEquipmentObservationComponent,
        RegisterCommentComponent
    ]
})
export class AppModule {

}
