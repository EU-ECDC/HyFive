"use strict";
var __esDecorate = (this && this.__esDecorate) || function (ctor, descriptorIn, decorators, contextIn, initializers, extraInitializers) {
    function accept(f) { if (f !== void 0 && typeof f !== "function") throw new TypeError("Function expected"); return f; }
    var kind = contextIn.kind, key = kind === "getter" ? "get" : kind === "setter" ? "set" : "value";
    var target = !descriptorIn && ctor ? contextIn["static"] ? ctor : ctor.prototype : null;
    var descriptor = descriptorIn || (target ? Object.getOwnPropertyDescriptor(target, contextIn.name) : {});
    var _, done = false;
    for (var i = decorators.length - 1; i >= 0; i--) {
        var context = {};
        for (var p in contextIn) context[p] = p === "access" ? {} : contextIn[p];
        for (var p in contextIn.access) context.access[p] = contextIn.access[p];
        context.addInitializer = function (f) { if (done) throw new TypeError("Cannot add initializers after decoration has completed"); extraInitializers.push(accept(f || null)); };
        var result = (0, decorators[i])(kind === "accessor" ? { get: descriptor.get, set: descriptor.set } : descriptor[key], context);
        if (kind === "accessor") {
            if (result === void 0) continue;
            if (result === null || typeof result !== "object") throw new TypeError("Object expected");
            if (_ = accept(result.get)) descriptor.get = _;
            if (_ = accept(result.set)) descriptor.set = _;
            if (_ = accept(result.init)) initializers.unshift(_);
        }
        else if (_ = accept(result)) {
            if (kind === "field") initializers.unshift(_);
            else descriptor[key] = _;
        }
    }
    if (target) Object.defineProperty(target, contextIn.name, descriptor);
    done = true;
};
var __runInitializers = (this && this.__runInitializers) || function (thisArg, initializers, value) {
    var useValue = arguments.length > 2;
    for (var i = 0; i < initializers.length; i++) {
        value = useValue ? initializers[i].call(thisArg, value) : initializers[i].call(thisArg);
    }
    return useValue ? value : void 0;
};
var __setFunctionName = (this && this.__setFunctionName) || function (f, name, prefix) {
    if (typeof name === "symbol") name = name.description ? "[".concat(name.description, "]") : "";
    return Object.defineProperty(f, "name", { configurable: true, value: prefix ? "".concat(prefix, " ", name) : name });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.AppModule = exports.httpInterceptorProviders = void 0;
var platform_browser_1 = require("@angular/platform-browser");
var core_1 = require("@angular/core");
var forms_1 = require("@angular/forms");
var http_1 = require("@angular/common/http");
var app_component_1 = require("./app.component");
var main_menu_component_1 = require("./main-menu/main-menu.component");
var home_page_observation_component_1 = require("./startside/home-page-observation.component");
var service_worker_1 = require("@angular/service-worker");
var environment_1 = require("../environments/environment");
var register_four_indications_component_1 = require("./registrering/register-four-indications/register-four-indications.component");
var four_indications_session_service_1 = require("./services/data/four-indications-session.service");
var register_activity_component_1 = require("./registrering/register-activity/register-activity.component");
var missed_opportunity_component_1 = require("./registrering/missed-opportunity/missed-opportunity.component");
var four_indications_observation_card_component_1 = require("./registrering/four-indications-observation-card/four-indications-observation-card.component");
var indication_selection_component_1 = require("./registrering/indication-selection/indication-selection.component");
var animations_1 = require("@angular/platform-browser/animations");
var hammerjsConfig_1 = require("../hammerjsConfig");
var register_comment_component_1 = require("./registrering/register-comment/register-comment.component");
var ng_bootstrap_1 = require("@ng-bootstrap/ng-bootstrap");
var angular_fontawesome_1 = require("@fortawesome/angular-fontawesome");
var register_hand_jewelry_component_1 = require("./registrering/register-hand-jewelry/register-hand-jewelry.component");
var save_shadow_component_1 = require("./registrering/save-shadow/save-shadow.component");
var delete_shadow_component_1 = require("./registrering/delete-shadow/delete-shadow.component");
var hand_Jewelry_session_service_1 = require("./services/data/hand-Jewelry-session.service");
var handjewelry_observation_card_component_1 = require("./registrering/handjewelry-observation-card/handjewelry-observation-card.component");
var role_selection_dropdown_component_1 = require("./registrering/role-selection-dropdown/role-selection-dropdown.component");
//import { FhiAccordionModule } from '@folkehelseinstituttet/ng-components';
var register_protective_equipment_component_1 = require("./registrering/register-protective-equipment/register-protective-equipment.component");
var protective_equipment_observation_card_component_1 = require("./registrering/protective equipment-observation-card/protective equipment-observation-card.component");
var selection_for_protective_equipment_component_1 = require("./startside/selection-for-protective-equipment/selection-for-protective-equipment.component");
var navigation_link_component_1 = require("./shared/navigation-link/navigation-link.component");
var accordion_component_1 = require("./shared/accordion/accordion.component");
var ngx_toastr_1 = require("ngx-toastr");
var protective_equipment_modal_component_1 = require("./registrering/protective-equipment-modal/protective-equipment-modal.component");
var new_card_modal_component_1 = require("./registrering/new-card-modal/new-card-modal.component");
var offline_message_component_1 = require("./shared/offline-message/offline-message.component");
var toastr_config_1 = require("./constants/toastr-config");
var observation_counter_component_1 = require("./shared/observation-counter/observation-counter.component");
var info_modal_component_1 = require("./shared/info-modal/info-modal.component");
var register_glove_component_1 = require("./registrering/register-glove/register-glove.component");
var glove_observation_card_component_1 = require("./registrering/glove-observation-card/glove-observation-card.component");
var dialog_modal_component_1 = require("./shared/dialog-modal/dialog-modal.component");
var ng_select_1 = require("@ng-select/ng-select");
var new_card_info_component_1 = require("./shared/new-card-info/new-card-info.component");
var loginpage_component_1 = require("./login-page/loginpage.component");
var spinner_component_1 = require("./shared/spinner/spinner.component");
var app_routing_module_1 = require("./app-routing.module");
var authentication_failed_error_interceptor_1 = require("./http-interceptors/authentication-failed-error.interceptor");
var authentication_failed_modal_component_1 = require("./shared/authentication-failed-modal/authentication-failed-modal.component");
var drag_drop_1 = require("@angular/cdk/drag-drop");
var help_text_component_1 = require("./shared/help-text/help-text.component");
var help_text_setting_component_1 = require("./shared/help-text-setting/help-text-setting.component");
var pseudonym_component_1 = require("./shared/pseudonym-modal/pseudonym.component");
var not_sent_sessions_component_1 = require("./sesjoner/not-sent-sessions/not-sent-sessions.component");
var edit_four_indications_observation_component_1 = require("./sesjoner/edit-four-indications-observation/edit-four-indications-observation.component");
var four_indications_component_1 = require("./sesjoner/fire-indikasjoner/four-indications.component");
var delete_confirmation_dialog_component_1 = require("./sesjoner/delete-confirmation-dialog/delete-confirmation-dialog.component");
var handJewelry_component_1 = require("./sesjoner/handJewelry/handJewelry.component");
var edit_hand_jewelry_observation_component_1 = require("./sesjoner/edit-hand-jewelry-observation/edit-hand-jewelry-observation.component");
var session_overview_component_1 = require("./sesjoner/session-overview/session-overview.component");
var sent_sessions_component_1 = require("./sesjoner/sent-sessions/sent-sessions.component");
var protection_equipment_component_1 = require("./sesjoner/protection-equipment/protection-equipment.component");
var session_edit_header_component_1 = require("./sesjoner/session-edit-header/session-edit-header.component");
var sent_four_indications_session_component_1 = require("./sesjoner/sent-sessions/sent-four-indications-session/sent-four-indications-session.component");
var edit_protective_equipment_observation_component_1 = require("./sesjoner/edit-protective-equipment-observation/edit-protective-equipment-observation.component");
var sent_session_overview_component_1 = require("./sesjoner/sent-sessions/sent-session-overview/sent-session-overview.component");
var sent_hand_jewelry_session_component_1 = require("./sesjoner/sent-sessions/sent-hand-jewelry-session/sent-hand-jewelry-session.component");
var sent_protective_equipment_session_component_1 = require("./sesjoner/sent-sessions/sent-protective-equipment-session/sent-protective-equipment-session.component");
var session_statistics_component_1 = require("./sesjoner/fire-indikasjoner/session-statistics/session-statistics.component");
var glove_component_1 = require("./sesjoner/glove/glove.component");
var sent_glove_session_component_1 = require("./sesjoner/sent-sessions/sent-glove-session/sent-glove-session.component");
var edit_glove_observation_component_1 = require("./sesjoner/edit-glove-observation/edit-glove-observation.component");
exports.httpInterceptorProviders = [
    { provide: http_1.HTTP_INTERCEPTORS, useClass: authentication_failed_error_interceptor_1.AuthenticationFailedErrorInterceptor, multi: true },
];
var AppModule = function () {
    var _classDecorators = [(0, core_1.NgModule)({
            declarations: [
                app_component_1.AppComponent,
                main_menu_component_1.MainMenuComponent,
                home_page_observation_component_1.HomePageForObservationComponent,
                register_four_indications_component_1.RegisterFiveIndicationsComponent,
                four_indications_observation_card_component_1.FiveIndicationsObservationCardComponent,
                register_activity_component_1.RegisterActivityComponent,
                missed_opportunity_component_1.MissedOpportunityComponent,
                indication_selection_component_1.IndicationSelectionComponent,
                register_comment_component_1.RegisterCommentComponent,
                not_sent_sessions_component_1.NotSentSessionsComponent,
                edit_four_indications_observation_component_1.EditFiveIndicationsObservationComponent,
                four_indications_component_1.FiveIndicationsComponent,
                delete_confirmation_dialog_component_1.DeleteConfirmationDialogComponent,
                register_hand_jewelry_component_1.RegisterHandjewelryComponent,
                handjewelry_observation_card_component_1.HandJewelryObservationCardComponent,
                save_shadow_component_1.SaveShadowComponent,
                delete_shadow_component_1.DeleteShadowComponent,
                handJewelry_component_1.HandJewelryComponent,
                edit_hand_jewelry_observation_component_1.EditHandJewelryObservationComponent,
                role_selection_dropdown_component_1.RoleSelectDropdownComponent,
                sent_sessions_component_1.SentSessionsComponent,
                session_overview_component_1.SessionOverviewComponent,
                register_protective_equipment_component_1.RegisterProtectiveEquipmentComponent,
                protective_equipment_observation_card_component_1.ProtectiveEquipmentObservationCardComponent,
                selection_for_protective_equipment_component_1.SelectionForProtectiveEquipmentComponent,
                navigation_link_component_1.NavigationLinkComponent,
                protection_equipment_component_1.ProtectiveEquipmentComponent,
                accordion_component_1.AccordionComponent,
                session_edit_header_component_1.SessionEditHeaderComponent,
                edit_protective_equipment_observation_component_1.EditProtectiveEquipmentObservationComponent,
                protective_equipment_modal_component_1.ProtectiveEquipmentModalComponent,
                new_card_modal_component_1.NewCardModalComponent,
                sent_four_indications_session_component_1.SentFiveIndicationsSessionComponent,
                sent_session_overview_component_1.SentSessionOverviewComponent,
                offline_message_component_1.OfflineMessageComponent,
                sent_hand_jewelry_session_component_1.SentHandJewelrySessionComponent,
                sent_protective_equipment_session_component_1.SentProtectiveEquipmentSessionComponent,
                session_statistics_component_1.SessionStatisticsComponent,
                observation_counter_component_1.ObservationCounterComponent,
                info_modal_component_1.InfoModalComponent,
                register_glove_component_1.RegisterGloveComponent,
                glove_observation_card_component_1.GloveObservationCardComponent,
                glove_component_1.GloveComponent,
                sent_glove_session_component_1.SentGloveSessionComponent,
                edit_glove_observation_component_1.EditGloveObservationComponent,
                dialog_modal_component_1.DialogModalComponent,
                new_card_info_component_1.NewCardInfoComponent,
                loginpage_component_1.LoginPageComponent,
                spinner_component_1.SpinnerComponent,
                authentication_failed_modal_component_1.AuthenticationFailedModalComponent,
                help_text_component_1.HelpTextComponent,
                help_text_setting_component_1.HelpTextSettingsComponent,
                pseudonym_component_1.PseudonymComponent
            ],
            imports: [
                platform_browser_1.BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
                app_routing_module_1.AppRoutingModule,
                http_1.HttpClientModule,
                forms_1.FormsModule,
                service_worker_1.ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment_1.environment.production }),
                platform_browser_1.HammerModule,
                animations_1.BrowserAnimationsModule,
                ng_bootstrap_1.NgbModule,
                ngx_toastr_1.ToastrModule.forRoot(toastr_config_1.ToastrConfig.toastrConfig),
                angular_fontawesome_1.FontAwesomeModule,
                //FhiAccordionModule,
                ng_select_1.NgSelectModule,
                drag_drop_1.DragDropModule
            ],
            bootstrap: [app_component_1.AppComponent],
            providers: [four_indications_session_service_1.FiveIndicationsSessionService, hand_Jewelry_session_service_1.HandJewelrySessionService, hammerjsConfig_1.HandHygieneHammerJS, exports.httpInterceptorProviders],
            exports: [
                edit_protective_equipment_observation_component_1.EditProtectiveEquipmentObservationComponent,
                register_comment_component_1.RegisterCommentComponent
            ]
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var AppModule = _classThis = /** @class */ (function () {
        function AppModule_1() {
        }
        return AppModule_1;
    }());
    __setFunctionName(_classThis, "AppModule");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        AppModule = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return AppModule = _classThis;
}();
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map