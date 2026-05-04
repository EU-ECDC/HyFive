import { BrowserModule, HammerModule } from '@angular/platform-browser';
import { APP_INITIALIZER, LOCALE_ID, NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClient, HttpClientModule} from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { AppComponent } from './app.component';
import { MainMenuComponent } from './main-menu/main-menu.component';
import { HomePageForObservationComponent } from './startside/home-page-observation.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { RegisterActivityComponent } from './registrering/register-activity/register-activity.component';
import { MissedOpportunityComponent } from './registrering/missed-opportunity/missed-opportunity.component';
import { HandHygieneObservationCardComponent } from './registrering/hand-hygiene-observation-card/hand-hygiene-observation-card.component';
import { IndicationSelectionComponent } from './registrering/indication-selection/indication-selection.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HandHygieneHammerJS } from '../hammerjsConfig';
import { RegisterCommentComponent } from './registrering/register-comment/register-comment.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { RegisterBareBelowElbowsComponent } from './registrering/register-bare-below-elbows/register-bare-below-elbows.component';
import { SaveShadowComponent } from './registrering/save-shadow/save-shadow.component';
import { DeleteShadowComponent } from './registrering/delete-shadow/delete-shadow.component';
import { HandJewelrySessionService } from './services/data/hand-Jewelry-session.service';
import { BareBelowElbowsObservationCardComponent } from './registrering/bare-below-elbows-observation-card/bare-below-elbows-observation-card.component';
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
import { NotSentSessionsComponent } from './sessions/not-sent-sessions/not-sent-sessions.component';
import { EditHandHygieneObservationComponent } from './sessions/edit-hand-hygiene-observation/edit-hand-hygiene-observation.component';
import { HandHygieneComponent } from './sessions/hand-hygiene/hand-hygiene.component';
import { DeleteConfirmationDialogComponent } from './sessions/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { HandJewelryComponent } from './sessions/handJewelry/handJewelry.component';
import { EditHandJewelryObservationComponent } from './sessions/edit-hand-jewelry-observation/edit-hand-jewelry-observation.component';
import { SessionOverviewComponent } from './sessions/session-overview/session-overview.component';
import { SentSessionsComponent } from './sessions/sent-sessions/sent-sessions.component';
import { ProtectiveEquipmentComponent } from './sessions/protection-equipment/protection-equipment.component';
import { SessionEditHeaderComponent } from './sessions/session-edit-header/session-edit-header.component';
import { EditProtectiveEquipmentObservationComponent } from './sessions/edit-protective-equipment-observation/edit-protective-equipment-observation.component';
import { SentSessionOverviewComponent } from './sessions/sent-sessions/sent-session-overview/sent-session-overview.component';
import { SentHandJewelrySessionComponent } from './sessions/sent-sessions/sent-hand-jewelry-session/sent-hand-jewelry-session.component';
import { SentProtectiveEquipmentSessionComponent } from './sessions/sent-sessions/sent-protective-equipment-session/sent-protective-equipment-session.component';
import { SessionStatisticsComponent } from './sessions/hand-hygiene/session-statistics/session-statistics.component';
import { GloveComponent } from './sessions/glove/glove.component';
import { SentGloveSessionComponent } from './sessions/sent-sessions/sent-glove-session/sent-glove-session.component';
import { EditGloveObservationComponent } from './sessions/edit-glove-observation/edit-glove-observation.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { LanguageService } from './services/data/language-service';
import { LanguageSelectorComponent } from './shared/language-selector/language-selector.component';
import { DatePipe, registerLocaleData } from '@angular/common';
import localeEl from '@angular/common/locales/el';
import localeEn from '@angular/common/locales/en';
import { RegisterHandHygieneComponent } from './registrering/register-hand-hygiene/register-hand-hygiene.component';
import { SentHandHygieneSessionComponent } from './sessions/sent-sessions/sent-hand-hygiene-session/sent-hand-hygiene-session.component';
import { HandHygieneSessionService } from './services/data/hand-hygiene-session.service';

registerLocaleData(localeEl);
registerLocaleData(localeEn);

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationFailedErrorInterceptor, multi: true },
];

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}


export function languageInitializer(langService: LanguageService) {
  return () => langService.initLanguage();
}

export function getLocale(): string {
  return localStorage.getItem('lang') || 'en';
}


@NgModule({
    declarations: [
        AppComponent,
        MainMenuComponent,
        HomePageForObservationComponent,
        RegisterHandHygieneComponent,
        HandHygieneObservationCardComponent,
        RegisterActivityComponent,
        MissedOpportunityComponent,
        IndicationSelectionComponent,
        RegisterCommentComponent,
        NotSentSessionsComponent,
        EditHandHygieneObservationComponent,
        HandHygieneComponent,
        DeleteConfirmationDialogComponent,
        RegisterBareBelowElbowsComponent,
        BareBelowElbowsObservationCardComponent,
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
        SentHandHygieneSessionComponent,
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
        PseudonymComponent,
        LanguageSelectorComponent
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
        DragDropModule,
        MatPaginatorModule,
        TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    })
    ],
    bootstrap: [AppComponent],
    providers: [DatePipe,
                HandHygieneSessionService,
                HandJewelrySessionService,
                HandHygieneHammerJS,
                httpInterceptorProviders,
                {
                provide: APP_INITIALIZER,
                useFactory: languageInitializer,
                deps: [LanguageService],
                multi: true
              },
              { provide: LOCALE_ID, useFactory: getLocale },
              // {
              //   provide: LOCALE_ID,
              //   deps: [LanguageService],
              //   useFactory: (langService: LanguageService) => langService.getLocale()
              // }
            ],
    exports: [
        EditProtectiveEquipmentObservationComponent,
        RegisterCommentComponent
    ]
})
export class AppModule {

}
