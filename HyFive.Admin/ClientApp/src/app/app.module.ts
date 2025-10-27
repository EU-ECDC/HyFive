import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { MatPaginatorModule } from '@angular/material/paginator';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomePageForAdministrationComponent } from './home-page-for-administration/home-page-for-administration.component';
import { EditingOfFacilitiesComponent } from './admin/edit-facilities/editing-of-facilities.component';
import { EditFacilityComponent } from './admin/edit-facilities/edit-a-facility/edit-a-facility.component';
import { CreateFacilityComponent } from './admin/edit-facilities/create-facility/create-facility.component';
import { EditObserversComponent } from './_common/edit-observers/edit-observers.component';
import { ConfirmationDialogComponent } from './admin/edit-facilities/confirmation-dialog/confirmation-dialog.component';
import { EditingIndicationTypesComponent } from './admin/edit-codeworks/editing-indication-types/editing-indication-types.component';
import { EditingByHandjewelryTypeComponent } from './admin/edit-codeworks/editing-HandjewelryType/editing-of-handjewelry-type.component';
import { EditingFacilityTypesComponent } from './admin/edit-codeworks/editing-of-facility-types/editing-of-facility-types.component';
import { EditingCodeworkComponent } from './admin/edit-codeworks/editing-code-works.component';
import { EditingDepartmentsComponent } from './_common/editing-of-departments/editing-of-departments.component';
import { CreateDepartmentComponent } from './_common/editing-of-departments/create-department/create-department.component';
import { EditingActivityTypeComponent } from './admin/edit-codeworks/editing-of-activitytype/editing-activitytype.component';
import { EditingProtectiveEquipmentTypeComponent } from './admin/edit-codeworks/editing-of-protective-equipment-types/editing-of-protectiveequipment-types.component';
import { EditingMisuseTypesComponent } from './admin/edit-codeworks/editing-of-protective-equipment-types/editing-of-misuse-types/editing-of-misuse-types.component';
import { EditingOfProtectiveEquipmentSettingTypesComponent } from './admin/edit-codeworks/editing-of-protective-equipment-setting-types/editing-of-protective-equipment-setting-types.component';
import { OverviewObservationsComponent } from './admin/overview-observations/overview-observations.component';
import { DatePipe } from '@angular/common';
import { OverviewDepartmentSessionsComponent } from './admin/overview-observations/overview-department-sessions/overview-department-sessions.component';
import { EditingOfObserversComponent } from './coordinator/editing-observers/editing-of-observers.component';
import { ProfilsideComponent } from './profile-page/profile-page.component';
import { ClipboardModule } from 'ngx-clipboard';
import { EditingGlovesWithIndicationTypesComponent } from './admin/edit-codeworks/editing-gloves-with-indicationtypes/editing-gloves-with-indicationtypes.component';
import { EditingGlovewithoutindicationtypesComponent } from './admin/edit-codeworks/editing-of-gloveswithoutindicationtypes/editing-of-gloveswithoutindicationtypes.component';
import { EditingHandHygieneAfterGloveUseTypesComponent } from './admin/edit-codeworks/editing-of-hand-hygiene-after-glove-usetypes/editing-of-hand-hygiene-after-glove-usetypes.component';
import { TransferSessionsComponent } from './coordinator/transfer-sessions/transfer-sessions.component';
import { OverviewSessionsViewComponent } from './_common/overview-sessions-view/overview-sessions-view.component';
import { EditingPredefinedCommentsComponent } from './coordinator/edit-predefined-comments/edit-predefined-comments.component';
import { EditingOfDepartmentTypesComponent } from './admin/edit-codeworks/editing-of-departmenttypes/editing-of-departmentstype.component';
import { EditingUnitsComponent } from './coordinator/edit-units/editing-of-unit.component';
import { CreateUnitComponent } from './coordinator/edit-units/create-unit/create-unit.component';
import { EditAUnitComponent } from './coordinator/edit-units/edit-a-unit/edit-a-unit.component';
import { RoleSelectionDropdownComponent } from './_common/app-role-selection-dropdown/app-role-selection-dropdown.component';
import { EditingOfRolesComponent } from './admin/edit-codeworks/editing-of-roles/editing-of-roles.component';
import { OverviewAdminComponent } from './admin/overview-admin/overview-admin.component';
import { EditAdminComponent } from './admin/overview-admin/edit-admin/edit-admin.component';
import {AuthenticationFailedModalComponent} from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {EditFiveIndicationsObservationsComponent} from "./coordinator/edit-observations/edit-five-indications-observations/edit-five-indications-observations.component";
import {EditHandjewelryObservationsComponent} from "./coordinator/edit-observations/edit-handjewelry-observations/edit-handjewelry-observations.component";
import {EditGloveObservationsComponent} from "./coordinator/edit-observations/edit-glove-observations/edit-glove-observations.component";
import {IndicationSelectionComponent} from "./coordinator/edit-observations/edit-five-indications-observations/indication-selection/indication-selection.component";
import {ActivityChoiceDropdownComponent} from "./coordinator/edit-observations/edit-five-indications-observations/activity-choice/activity-choice-dropdown.component";
import { EditProtectiveEquipmentObservationsComponent } from "./coordinator/edit-observations/edit-protective-equipment-observations/edit-protective-equipment-observations.component";
import { EditProtectiveEquipmentObservationComponent } from './coordinator/edit-observations/edit-protective-equipment-observations/edit-protective-equipment-observation/edit-protective-equipment-observation.component';
import {ProtectiveEquipmentModalComponent} from "./coordinator/edit-observations/edit-protective-equipment-observations/protective-equipment-modal/protective-equipment-modal.component";
import { SearchHprNumberLinkComponent } from './_common/search-hprnumber-link/search-hprnumber-link.component';
import { EditSessionDataComponent } from './_common/overview-sessions-view/edit-sessionsdata/edit-sessionsdata.component';
import { EditCoordinatorsForCityComponent } from './coordinator/editing-of-coordinators/edit-coordinators-for-city.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { EditingCoordinatorsComponent } from './coordinator/editing-of-coordinators/editing-of-coordinators.component';
import { EditCoordinatorsComponent } from './_common/edit-coordinators/editCoordinators.component';
import { PseudonymDialogComponent } from './_common/edit-coordinators/pseudonym-dialog.component';
import { EmailComponent } from './admin/email/email.component';
import { FhiAngularComponentsModule, FhiMultiselectComponent } from '@folkehelseinstituttet/angular-components';
import { FhiAngularHighchartsModule } from '@folkehelseinstituttet/angular-highcharts';
import { ReportComponent } from './coordinator/reports/report.component';
import { ComplianceComponent } from './coordinator/reports/five-indications/compliance/compliance.component';
import { SortableColumnComponent } from './shared/sorting/sortable-column.component';
import { SortableTableDirective } from './shared/sorting/sortable-table.directive';
import { SortService } from './shared/sorting/sort.service';
import { DownloadExcelComponent } from './coordinator/reports/download/download-excel.component';
import { ComplianceFiveIndicationsPdfComponent } from './coordinator/reports/predefined/compliance-five-indications-pdf.component';
import { ComplianceHandJewelryPdfComponent } from './coordinator/reports/predefined/compliance-handJewelry-pdf.component';
import { CompliancePdfComponent } from './coordinator/reports/common/compliance-pdf.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthenticationFailedErrorInterceptor, multi: true },
];

@NgModule({
  declarations: [
    AppComponent,
    HomePageForAdministrationComponent,
    EditingOfFacilitiesComponent,
    EditFacilityComponent,
    CreateFacilityComponent,
    EditObserversComponent,
    EditingByHandjewelryTypeComponent,
    EditCoordinatorsComponent,
    ConfirmationDialogComponent,
    EditingCodeworkComponent,
    EditingIndicationTypesComponent,
    EditingActivityTypeComponent,
    EditingFacilityTypesComponent,
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
    EditingUnitsComponent,
    CreateUnitComponent,
    EditAUnitComponent,
    RoleSelectionDropdownComponent,
    IndicationSelectionComponent,
    ActivityChoiceDropdownComponent,
    EditingOfRolesComponent,
    OverviewAdminComponent,
    EditAdminComponent,
    AuthenticationFailedModalComponent,
    EditFiveIndicationsObservationsComponent,
    EditHandjewelryObservationsComponent,
    EditGloveObservationsComponent,
    EditProtectiveEquipmentObservationsComponent,
    EditProtectiveEquipmentObservationComponent,
    ProtectiveEquipmentModalComponent,
    SearchHprNumberLinkComponent,
    EditSessionDataComponent,
    EditingCoordinatorsComponent,
    EditCoordinatorsForCityComponent,
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
    FhiAngularHighchartsModule,
    MatPaginatorModule
  ],
  bootstrap: [AppComponent],
  providers: [DatePipe, httpInterceptorProviders, SortService, provideAnimationsAsync()]
})
export class AppModule { }
