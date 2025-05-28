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
var core_module_1 = require("./core/core.module");
var shared_module_1 = require("./shared/shared.module");
var app_routing_module_1 = require("./app-routing.module");
var app_component_1 = require("./app.component");
var home_page_for_administration_component_1 = require("./home-page-for-administration/home-page-for-administration.component");
var editing_of_institutions_component_1 = require("./fhiadmin/redigering-av-institusjoner/editing-of-institutions.component");
var edit_an_institution_component_1 = require("./fhiadmin/redigering-av-institusjoner/edit-an-institution/edit-an-institution.component");
var create_institution_component_1 = require("./fhiadmin/redigering-av-institusjoner/create-institution/create-institution.component");
var edit_observers_component_1 = require("./_common/edit-observers/edit-observers.component");
var confirmation_dialog_component_1 = require("./fhiadmin/redigering-av-institusjoner/confirmation-dialog/confirmation-dialog.component");
var editing_indication_types_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-indication-types/editing-indication-types.component");
var editing_of_handjewelry_type_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-HandjewelryType/editing-of-handjewelry-type.component");
var editing_of_institution_types_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-of-institution-types/editing-of-institution-types.component");
var editing_code_works_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-code-works.component");
var editing_of_departments_component_1 = require("./_common/redigering-av-avdelinger/editing-of-departments.component");
var create_department_component_1 = require("./_common/redigering-av-avdelinger/create-department/create-department.component");
var editing_activitytype_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-of-activitytype/editing-activitytype.component");
var editing_of_protectiveequipment_types_component_1 = require("./fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/editing-of-protectiveequipment-types.component");
var editing_of_misuse_types_component_1 = require("./fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/editing-of-misuse-types/editing-of-misuse-types.component");
var editing_of_protective_equipment_setting_types_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-of-protective-equipment-setting-types/editing-of-protective-equipment-setting-types.component");
var overview_observations_component_1 = require("./fhiadmin/oversikt-observasjoner/overview-observations.component");
var common_1 = require("@angular/common");
var overview_department_sessions_component_1 = require("./fhiadmin/oversikt-observasjoner/overview-department-sessions/overview-department-sessions.component");
var editing_of_observers_component_1 = require("./coordinator/editing-observers/editing-of-observers.component");
var profile_page_component_1 = require("./profile-page/profile-page.component");
var ngx_clipboard_1 = require("ngx-clipboard");
var editing_gloves_with_indicationtypes_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-gloves-with-indicationtypes/editing-gloves-with-indicationtypes.component");
var editing_of_gloveswithoutindicationtypes_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-of-gloveswithoutindicationtypes/editing-of-gloveswithoutindicationtypes.component");
var editing_of_hand_hygiene_after_glove_usetypes_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-of-hand-hygiene-after-glove-usetypes/editing-of-hand-hygiene-after-glove-usetypes.component");
var transfer_sessions_component_1 = require("./coordinator/transfer-sessions/transfer-sessions.component");
var overview_sessions_view_component_1 = require("./_common/oversikt-sesjoner-visning/overview-sessions-view.component");
var edit_predefined_comments_component_1 = require("./coordinator/edit-predefined-comments/edit-predefined-comments.component");
var editing_of_departmentstype_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-of-departmenttypes/editing-of-departmentstype.component");
var editing_of_clinic_component_1 = require("./coordinator/redigering-av-klinikker/editing-of-clinic.component");
var create_clinic_component_1 = require("./coordinator/redigering-av-klinikker/create-clinic/create-clinic.component");
var edit_a_clinic_component_1 = require("./coordinator/redigering-av-klinikker/edit-a-clinic/edit-a-clinic.component");
var editing_region_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-region.component/editing-region.component");
var app_role_selection_dropdown_component_1 = require("./_common/app-role-selection-dropdown/app-role-selection-dropdown.component");
var editing_of_roles_component_1 = require("./fhiadmin/redigering-av-kodeverk/editing-of-roles/editing-of-roles.component");
var overview_fhiadmin_component_1 = require("./fhiadmin/oversikt-fhiadmin/overview-fhiadmin.component");
var edit_fhiadmin_component_1 = require("./fhiadmin/oversikt-fhiadmin/edit-fhiadmin/edit-fhiadmin.component");
var authentication_failed_modal_component_1 = require("./shared/authentication-failed-modal/authentication-failed-modal.component");
var http_1 = require("@angular/common/http");
var authentication_failed_error_interceptor_1 = require("./http-interceptors/authentication-failed-error.interceptor");
var ng_bootstrap_1 = require("@ng-bootstrap/ng-bootstrap");
var edit_four_indications_observations_component_1 = require("./coordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/edit-four-indications-observations.component");
var edit_handjewelry_observations_component_1 = require("./coordinator/redigering-av-observasjoner/edit-handjewelry-observations/edit-handjewelry-observations.component");
var edit_glove_observations_component_1 = require("./coordinator/redigering-av-observasjoner/edit-glove-observations/edit-glove-observations.component");
var indication_selection_component_1 = require("./coordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/indication-selection/indication-selection.component");
var activity_choice_dropdown_component_1 = require("./coordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/activity-choice/activity-choice-dropdown.component");
var edit_protective_equipment_observations_component_1 = require("./coordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/edit-protective-equipment-observations.component");
var edit_protective_equipment_observation_component_1 = require("./coordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/rediger-beskyttelsesutstyr-observasjon/edit-protective-equipment-observation.component");
var protective_equipment_modal_component_1 = require("./coordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/protective-equipment-modal/protective-equipment-modal.component");
var search_hprnumber_link_component_1 = require("./_common/search-hprnumber-link/search-hprnumber-link.component");
var edit_sessionsdata_component_1 = require("./_common/oversikt-sesjoner-visning/edit-sessionsdata/edit-sessionsdata.component");
var request_component_1 = require("./coordinator/request/request.component");
var health_enterprise_component_1 = require("./fhiadmin/health-enterprise/health-enterprise.component");
var edit_coordinators_for_healthcareOrganization_component_1 = require("./coordinator/editing-of-coordinators/edit-coordinators-for-healthcareOrganization.component");
var ng_multiselect_dropdown_1 = require("ng-multiselect-dropdown");
var editing_of_coordinators_component_1 = require("./coordinator/editing-of-coordinators/editing-of-coordinators.component");
var editCoordinators_component_1 = require("./_common/edit-coordinators/editCoordinators.component");
var pseudonym_dialog_component_1 = require("./_common/edit-coordinators/pseudonym-dialog.component");
var email_component_1 = require("./fhiadmin/email/email.component");
var angular_components_1 = require("@folkehelseinstituttet/angular-components");
var angular_highcharts_1 = require("@folkehelseinstituttet/angular-highcharts");
var report_component_1 = require("./coordinator/rapporter/report.component");
var compliance_component_1 = require("./coordinator/rapporter/fireIndikasjoner/compliance/compliance.component");
var sortable_column_component_1 = require("./shared/sorting/sortable-column.component");
var sortable_table_directive_1 = require("./shared/sorting/sortable-table.directive");
var sort_service_1 = require("./shared/sorting/sort.service");
var download_excel_component_1 = require("./coordinator/rapporter/download/download-excel.component");
var compliance_four_indications_pdf_component_1 = require("./coordinator/rapporter/predefined/compliance-four-indications-pdf.component");
var compliance_handJewelry_pdf_component_1 = require("./coordinator/rapporter/predefined/compliance-handJewelry-pdf.component");
var compliance_pdf_component_1 = require("./coordinator/rapporter/common/compliance-pdf.component");
exports.httpInterceptorProviders = [
    { provide: http_1.HTTP_INTERCEPTORS, useClass: authentication_failed_error_interceptor_1.AuthenticationFailedErrorInterceptor, multi: true },
];
var AppModule = function () {
    var _classDecorators = [(0, core_1.NgModule)({
            declarations: [
                app_component_1.AppComponent,
                home_page_for_administration_component_1.HomePageForAdministrationComponent,
                editing_of_institutions_component_1.EditingOfInstitutionsComponent,
                edit_an_institution_component_1.EditInstitutionComponent,
                create_institution_component_1.CreateInstitutionComponent,
                edit_observers_component_1.EditObserversComponent,
                editing_of_handjewelry_type_component_1.EditingByHandjewelryTypeComponent,
                editCoordinators_component_1.EditCoordinatorsComponent,
                confirmation_dialog_component_1.ConfirmationDialogComponent,
                editing_code_works_component_1.EditingCodeworkComponent,
                editing_indication_types_component_1.EditingIndicationTypesComponent,
                editing_activitytype_component_1.EditingActivityTypeComponent,
                editing_of_institution_types_component_1.EditingInstitutionTypesComponent,
                editing_of_departments_component_1.EditingDepartmentsComponent,
                create_department_component_1.CreateDepartmentComponent,
                editing_of_protectiveequipment_types_component_1.EditingProtectiveEquipmentTypeComponent,
                editing_of_misuse_types_component_1.EditingMisuseTypesComponent,
                editing_of_protective_equipment_setting_types_component_1.EditingOfProtectiveEquipmentSettingTypesComponent,
                overview_observations_component_1.OverviewObservationsComponent,
                overview_department_sessions_component_1.OverviewDepartmentSessionsComponent,
                editing_of_observers_component_1.EditingOfObserversComponent,
                profile_page_component_1.ProfilsideComponent,
                editing_gloves_with_indicationtypes_component_1.EditingGlovesWithIndicationTypesComponent,
                editing_of_gloveswithoutindicationtypes_component_1.EditingGlovewithoutindicationtypesComponent,
                editing_of_hand_hygiene_after_glove_usetypes_component_1.EditingHandHygieneAfterGloveUseTypesComponent,
                transfer_sessions_component_1.TransferSessionsComponent,
                overview_sessions_view_component_1.OverviewSessionsViewComponent,
                edit_predefined_comments_component_1.EditingPredefinedCommentsComponent,
                editing_of_departmentstype_component_1.EditingOfDepartmentTypesComponent,
                editing_of_clinic_component_1.EditingClinicsComponent,
                create_clinic_component_1.CreateClinicComponent,
                edit_a_clinic_component_1.EditAClinicComponent,
                editing_region_component_1.EditingRegionComponent,
                app_role_selection_dropdown_component_1.RoleSelectionDropdownComponent,
                indication_selection_component_1.IndicationSelectionComponent,
                activity_choice_dropdown_component_1.ActivityChoiceDropdownComponent,
                editing_of_roles_component_1.EditingOfRolesComponent,
                overview_fhiadmin_component_1.OverviewFhiAdminComponent,
                edit_fhiadmin_component_1.EditFhiAdminComponent,
                authentication_failed_modal_component_1.AuthenticationFailedModalComponent,
                edit_four_indications_observations_component_1.EditFourIndicationsObservationsComponent,
                edit_handjewelry_observations_component_1.EditHandjewelryObservationsComponent,
                edit_glove_observations_component_1.EditGloveObservationsComponent,
                edit_protective_equipment_observations_component_1.EditProtectiveEquipmentObservationsComponent,
                edit_protective_equipment_observation_component_1.EditProtectiveEquipmentObservationComponent,
                protective_equipment_modal_component_1.ProtectiveEquipmentModalComponent,
                search_hprnumber_link_component_1.SearchHprNumberLinkComponent,
                edit_sessionsdata_component_1.EditSessionDataComponent,
                request_component_1.RequestComponent,
                health_enterprise_component_1.HealthEnterpriseComponent,
                editing_of_coordinators_component_1.EditingCoordinatorsComponent,
                edit_coordinators_for_healthcareOrganization_component_1.EditCoordinatorsForHealthOrganizationComponent,
                pseudonym_dialog_component_1.PseudonymDialogComponent,
                email_component_1.EmailComponent,
                report_component_1.ReportComponent,
                compliance_component_1.ComplianceComponent,
                sortable_column_component_1.SortableColumnComponent,
                sortable_table_directive_1.SortableTableDirective,
                download_excel_component_1.DownloadExcelComponent,
                compliance_four_indications_pdf_component_1.ComplianceFourIndicationsPdfComponent,
                compliance_handJewelry_pdf_component_1.ComplianceHandJewelryPdfComponent,
                compliance_pdf_component_1.CompliancePdfComponent
            ],
            imports: [
                platform_browser_1.BrowserModule,
                app_routing_module_1.AppRoutingModule,
                core_module_1.CoreModule,
                shared_module_1.SharedModule,
                ngx_clipboard_1.ClipboardModule,
                ng_bootstrap_1.NgbModule,
                ng_multiselect_dropdown_1.NgMultiSelectDropDownModule.forRoot(),
                angular_components_1.FhiAngularComponentsModule,
                angular_highcharts_1.FhiAngularHighchartsModule
            ],
            bootstrap: [app_component_1.AppComponent],
            providers: [common_1.DatePipe, exports.httpInterceptorProviders, sort_service_1.SortService]
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