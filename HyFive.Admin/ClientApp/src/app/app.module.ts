import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomePageForAdministrationComponent } from './home-page-for-administration/home-page-for-administration.component';
import { EditingOfInstitutionsComponent } from './fhiadmin/redigering-av-institusjoner/editing-of-institutions.component';
import { EditInstitutionComponent } from './fhiadmin/redigering-av-institusjoner/edit-an-institution/edit-an-institution.component';
import { CreateInstitutionComponent } from './fhiadmin/redigering-av-institusjoner/create-institution/create-institution.component';
import { EditObserversComponent } from './_common/edit-observers/edit-observers.component';
import { ConfirmationDialogComponent } from './fhiadmin/redigering-av-institusjoner/confirmation-dialog/confirmation-dialog.component';
import { EditingIndicationTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-indication-types/editing-indication-types.component';
import { EditingByHandjewelryTypeComponent } from './fhiadmin/redigering-av-kodeverk/editing-HandjewelryType/editing-of-handjewelry-type.component';
import { EditingInstitutionTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-institution-types/editing-of-institution-types.component';
import { EditingCodeworkComponent } from './fhiadmin/redigering-av-kodeverk/editing-code-works.component';
import { EditingDepartmentsComponent } from './_common/redigering-av-avdelinger/editing-of-departments.component';
import { CreateDepartmentComponent } from './_common/redigering-av-avdelinger/create-department/create-department.component';
import { EditingActivityTypeComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-activitytype/editing-activitytype.component';
import { EditingProtectiveEquipmentTypeComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/editing-of-protectiveequipment-types.component';
import { EditingMisuseTypesComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/editing-of-misuse-types/editing-of-misuse-types.component';
import { EditingOfProtectiveEquipmentSettingTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-protective-equipment-setting-types/editing-of-protective-equipment-setting-types.component';
import { OverviewObservationsComponent } from './fhiadmin/oversikt-observasjoner/overview-observations.component';
import { DatePipe } from '@angular/common';
import { OverviewDepartmentSessionsComponent } from './fhiadmin/oversikt-observasjoner/overview-department-sessions/overview-department-sessions.component';
import { EditingOfObserversComponent } from './coordinator/editing-observers/editing-of-observers.component';
import { ProfilsideComponent } from './profile-page/profile-page.component';
import { ClipboardModule } from 'ngx-clipboard';
import { EditingGlovesWithIndicationTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-gloves-with-indicationtypes/editing-gloves-with-indicationtypes.component';
import { EditingGlovewithoutindicationtypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-gloveswithoutindicationtypes/editing-of-gloveswithoutindicationtypes.component';
import { EditingHandHygieneAfterGloveUseTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-hand-hygiene-after-glove-usetypes/editing-of-hand-hygiene-after-glove-usetypes.component';
import { TransferSessionsComponent } from './coordinator/transfer-sessions/transfer-sessions.component';
import { OverviewSessionsViewComponent } from './_common/oversikt-sesjoner-visning/overview-sessions-view.component';
import { EditingPredefinedCommentsComponent } from './coordinator/edit-predefined-comments/edit-predefined-comments.component';
import { EditingOfDepartmentTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-departmenttypes/editing-of-departmentstype.component';
import { EditingClinicsComponent } from './coordinator/redigering-av-klinikker/editing-of-clinic.component';
import { CreateClinicComponent } from './coordinator/redigering-av-klinikker/create-clinic/create-clinic.component';
import { EditAClinicComponent } from './coordinator/redigering-av-klinikker/edit-a-clinic/edit-a-clinic.component';
import { EditingRegionComponent } from './fhiadmin/redigering-av-kodeverk/editing-region.component/editing-region.component';
import { RoleSelectionDropdownComponent } from './_common/app-role-selection-dropdown/app-role-selection-dropdown.component';
import { EditingOfRolesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-roles/editing-of-roles.component';
import { OverviewFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/overview-fhiadmin.component';
import { EditFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/edit-fhiadmin/edit-fhiadmin.component';
import {AuthenticationFailedModalComponent} from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {EditFiveIndicationsObservationsComponent} from "./coordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/edit-five-indications-observations.component";
import {EditHandjewelryObservationsComponent} from "./coordinator/redigering-av-observasjoner/edit-handjewelry-observations/edit-handjewelry-observations.component";
import {EditGloveObservationsComponent} from "./coordinator/redigering-av-observasjoner/edit-glove-observations/edit-glove-observations.component";
import {IndicationSelectionComponent} from "./coordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/indication-selection/indication-selection.component";
import {ActivityChoiceDropdownComponent} from "./coordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/activity-choice/activity-choice-dropdown.component";
import { EditProtectiveEquipmentObservationsComponent } from "./coordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/edit-protective-equipment-observations.component";
import { EditProtectiveEquipmentObservationComponent } from './coordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/rediger-beskyttelsesutstyr-observasjon/edit-protective-equipment-observation.component';
import {ProtectiveEquipmentModalComponent} from "./coordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/protective-equipment-modal/protective-equipment-modal.component";
import { SearchHprNumberLinkComponent } from './_common/search-hprnumber-link/search-hprnumber-link.component';
import { EditSessionDataComponent } from './_common/oversikt-sesjoner-visning/edit-sessionsdata/edit-sessionsdata.component';
import { RequestComponent } from "./coordinator/request/request.component";
import { HealthEnterpriseComponent } from './fhiadmin/health-enterprise/health-enterprise.component';
import { EditCoordinatorsForHealthOrganizationComponent } from './coordinator/editing-of-coordinators/edit-coordinators-for-healthcareOrganization.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { EditingCoordinatorsComponent } from './coordinator/editing-of-coordinators/editing-of-coordinators.component';
import { EditCoordinatorsComponent } from './_common/edit-coordinators/editCoordinators.component';
import { PseudonymDialogComponent } from './_common/edit-coordinators/pseudonym-dialog.component';
import { EmailComponent } from './fhiadmin/email/email.component';
import { FhiAngularComponentsModule, FhiMultiselectComponent } from '@folkehelseinstituttet/angular-components';
import { FhiAngularHighchartsModule } from '@folkehelseinstituttet/angular-highcharts';
import { ReportComponent } from './coordinator/rapporter/report.component';
import { ComplianceComponent } from './coordinator/rapporter/fireIndikasjoner/compliance/compliance.component';
import { SortableColumnComponent } from './shared/sorting/sortable-column.component';
import { SortableTableDirective } from './shared/sorting/sortable-table.directive';
import { SortService } from './shared/sorting/sort.service';
import { DownloadExcelComponent } from './coordinator/rapporter/download/download-excel.component';
import { ComplianceFiveIndicationsPdfComponent } from './coordinator/rapporter/predefined/compliance-five-indications-pdf.component';
import { ComplianceHandJewelryPdfComponent } from './coordinator/rapporter/predefined/compliance-handJewelry-pdf.component';
import { CompliancePdfComponent } from './coordinator/rapporter/common/compliance-pdf.component';

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationFailedErrorInterceptor, multi: true },
];

@NgModule({
  declarations: [
    AppComponent,
    HomePageForAdministrationComponent,
    EditingOfInstitutionsComponent,
    EditInstitutionComponent,
    CreateInstitutionComponent,
    EditObserversComponent,
    EditingByHandjewelryTypeComponent,
    EditCoordinatorsComponent,
    ConfirmationDialogComponent,
    EditingCodeworkComponent,
    EditingIndicationTypesComponent,
    EditingActivityTypeComponent,
    EditingInstitutionTypesComponent,
    EditingDepartmentsComponent,
    CreateDepartmentComponent,
    EditingProtectiveEquipmentTypeComponent,
    EditingMisuseTypesComponent,
    EditingOfProtectiveEquipmentSettingTypesComponent,
    OverviewObservationsComponent,
    OverviewDepartmentSessionsComponent,
    EditingOfObserversComponent,
    ProfilsideComponent,
    EditingGlovesWithIndicationTypesComponent,
    EditingGlovewithoutindicationtypesComponent,
    EditingHandHygieneAfterGloveUseTypesComponent,
    TransferSessionsComponent,
    OverviewSessionsViewComponent,
    EditingPredefinedCommentsComponent,
    EditingOfDepartmentTypesComponent,
    EditingClinicsComponent,
    CreateClinicComponent,
    EditAClinicComponent,
    EditingRegionComponent,
    RoleSelectionDropdownComponent,
    IndicationSelectionComponent,
    ActivityChoiceDropdownComponent,
    EditingOfRolesComponent,
    OverviewFhiAdminComponent,
    EditFhiAdminComponent,
    AuthenticationFailedModalComponent,
    EditFiveIndicationsObservationsComponent,
    EditHandjewelryObservationsComponent,
    EditGloveObservationsComponent,
    EditProtectiveEquipmentObservationsComponent,
    EditProtectiveEquipmentObservationComponent,
    ProtectiveEquipmentModalComponent,
    SearchHprNumberLinkComponent,
    EditSessionDataComponent,
    RequestComponent,
    HealthEnterpriseComponent,
    EditingCoordinatorsComponent,
    EditCoordinatorsForHealthOrganizationComponent,
    PseudonymDialogComponent,
    EmailComponent,
    ReportComponent,
    ComplianceComponent,
    SortableColumnComponent,
    SortableTableDirective,
    DownloadExcelComponent,
    ComplianceFiveIndicationsPdfComponent,
    ComplianceHandJewelryPdfComponent,
    CompliancePdfComponent
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
