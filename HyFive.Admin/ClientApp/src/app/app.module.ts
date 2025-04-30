import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomePageForAdministrationComponent } from './front-page-for-administration/front-page-for-administration.component';
import { EditingOfInstitutionsComponent } from './fhiadmin/redigering-av-institusjoner/editing-of-institutions.component';
import { EditInstitutionComponent } from './fhiadmin/redigering-av-institusjoner/edit-an-institution/edit-an-institution.component';
import { CreateInstitutionComponent } from './fhiadmin/redigering-av-institusjoner/create-institution/create-institution.component';
import { EditObserversComponent } from './_felles/edit-observers/edit-observers.component';
import { ConfirmationDialogComponent } from './fhiadmin/redigering-av-institusjoner/confirmation-dialog/confirmation-dialog.component';
import { EditingIndicationTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-indication-types/editing-indication-types.component';
import { EditingByHandjewelryTypeComponent } from './fhiadmin/redigering-av-kodeverk/editing-HandjewelryType/editing-of-handjewelry-type.component';
import { EditingInstitutionTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-institution-types/editing-of-institution-types.component';
import { EditingCodeworkComponent } from './fhiadmin/redigering-av-kodeverk/editing-code-works.component';
import { EditingDepartmentsComponent } from './_felles/redigering-av-avdelinger/editing-of-departments.component';
import { CreateDepartmentComponent } from './_felles/redigering-av-avdelinger/create-department/create-department.component';
import { EditingActivityTypeComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-activitytype/editing-activitytype.component';
import { EditingProtectiveEquipmentTypeComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/editing-of-protectiveequipment-types.component';
import { EditingMisuseTypesComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-beskyttelsesutstyrtyper/editing-of-misuse-types/editing-of-misuse-types.component';
import { EditingOfProtectiveEquipmentSettingTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-protective-equipment-setting-types/editing-of-protective-equipment-setting-types.component';
import { OverviewObservationsComponent } from './fhiadmin/oversikt-observasjoner/overview-observations.component';
import { DatePipe } from '@angular/common';
import { OverviewDepartmentSessionsComponent } from './fhiadmin/oversikt-observasjoner/oversikt-avdeling-sesjoner/overview-department-sessions.component';
import { RedigeringAvObservatorerComponent } from './koordinator/redigering-av-observatorer/redigering-av-observatorer.component';
import { ProfilsideComponent } from './profilside/profilside.component';
import { ClipboardModule } from 'ngx-clipboard';
import { EditingGlovesWithIndicationTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-gloves-with-indicationtypes/editing-gloves-with-indicationtypes.component';
import { EditingGlovewithoutindicationtypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-gloveswithoutindicationtypes/editing-of-gloveswithoutindicationtypes.component';
import { EditingHandHygieneAfterGloveUseTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-hand-hygiene-after-glove-usetypes/editing-of-hand-hygiene-after-glove-usetypes.component';
import { TransferSessionsComponent } from './koordinator/transfer-sessions/transfer-sessions.component';
import { OverviewSessionsViewComponent } from './_felles/oversikt-sesjoner-visning/overview-sessions-view.component';
import { RedigeringAvPredefinertKommentarerComponent } from './koordinator/redigering-av-predefinert-kommentarer/redigering-av-predefinert-kommentarer.component';
import { EditingOfDepartmentTypesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-departmenttypes/editing-of-departmentstype.component';
import { EditingClinicsComponent } from './koordinator/redigering-av-klinikker/editing-of-clinic.component';
import { CreateClinicComponent } from './koordinator/redigering-av-klinikker/create-clinic/create-clinic.component';
import { EditAClinicComponent } from './koordinator/redigering-av-klinikker/edit-a-clinic/edit-a-clinic.component';
import { EditingRegionComponent } from './fhiadmin/redigering-av-kodeverk/editing-region.component/editing-region.component';
import { RoleSelectionDropdownComponent } from './_felles/app-role-selection-dropdown/app-role-selection-dropdown.component';
import { EditingOfRolesComponent } from './fhiadmin/redigering-av-kodeverk/editing-of-roles/editing-of-roles.component';
import { OverviewFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/overview-fhiadmin.component';
import { EditFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/edit-fhiadmin/edit-fhiadmin.component';
import {AuthenticationFailedModalComponent} from './shared/authentication-failed-modal/authentication-failed-modal.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {AuthenticationFailedErrorInterceptor} from './http-interceptors/authentication-failed-error.interceptor';
import {NgbModule} from "@ng-bootstrap/ng-bootstrap";
import {EditFourIndicationsObservationsComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/edit-four-indications-observations.component";
import {EditHandjewelryObservationsComponent} from "./koordinator/redigering-av-observasjoner/edit-handjewelry-observations/edit-handjewelry-observations.component";
import {EditGloveObservationsComponent} from "./koordinator/redigering-av-observasjoner/edit-glove-observations/edit-glove-observations.component";
import {IndicationSelectionComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/indication-selection/indication-selection.component";
import {ActivityChoiceDropdownComponent} from "./koordinator/redigering-av-observasjoner/rediger-fire-indikasjoner-observasjoner/activity-choice/activity-choice-dropdown.component";
import { EditProtectiveEquipmentObservationsComponent } from "./koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/edit-protective-equipment-observations.component";
import { EditProtectiveEquipmentObservationComponent } from './koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/rediger-beskyttelsesutstyr-observasjon/edit-protective-equipment-observation.component';
import {ProtectiveEquipmentModalComponent} from "./koordinator/redigering-av-observasjoner/rediger-beskyttelsesutstyr-observasjoner/protective-equipment-modal/protective-equipment-modal.component";
import { SearchHprNumberLinkComponent } from './_felles/search-hprnumber-link/search-hprnumber-link.component';
import { EditSessionDataComponent } from './_felles/oversikt-sesjoner-visning/edit-sessionsdata/edit-sessionsdata.component';
import { RequestComponent } from "./koordinator/request/request.component";
import { HealthEnterpriseComponent } from './fhiadmin/health-enterprise/health-enterprise.component';
import { EditCoordinatorsForHealthEnterprisesComponent } from './koordinator/editing-of-coordinators/edit-coordinators-for-healthcareOrganization.component';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { EditingCoordinatorsComponent } from './koordinator/editing-of-coordinators/editing-of-coordinators.component';
import { EditCoordinatorsComponent } from './_felles/edit-coordinators/editCoordinators.component';
import { PseudonymDialogComponent } from './_felles/edit-coordinators/pseudonym-dialog.component';
import { EmailComponent } from './fhiadmin/email/email.component';
import { FhiAngularComponentsModule, FhiMultiselectComponent } from '@folkehelseinstituttet/angular-components';
import { FhiAngularHighchartsModule } from '@folkehelseinstituttet/angular-highcharts';
import { ReportComponent } from './koordinator/rapporter/report.component';
import { ComplianceComponent } from './koordinator/rapporter/fireIndikasjoner/compliance/compliance.component';
import { SortableColumnComponent } from './shared/sorting/sortable-column.component';
import { SortableTableDirective } from './shared/sorting/sortable-table.directive';
import { SortService } from './shared/sorting/sort.service';
import { DownloadExcelComponent } from './koordinator/rapporter/download/download-excel.component';
import { ComplianceFourIndicationsPdfComponent } from './koordinator/rapporter/predefined/compliance-four-indications-pdf.component';
import { ComplianceHandJewelryPdfComponent } from './koordinator/rapporter/predefined/compliance-handJewelry-pdf.component';
import { CompliancePdfComponent } from './koordinator/rapporter/common/compliance-pdf.component';

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
    RedigeringAvObservatorerComponent,
    ProfilsideComponent,
    EditingGlovesWithIndicationTypesComponent,
    EditingGlovewithoutindicationtypesComponent,
    EditingHandHygieneAfterGloveUseTypesComponent,
    TransferSessionsComponent,
    OverviewSessionsViewComponent,
    RedigeringAvPredefinertKommentarerComponent,
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
    EditFourIndicationsObservationsComponent,
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
    EditCoordinatorsForHealthEnterprisesComponent,
    PseudonymDialogComponent,
    EmailComponent,
    ReportComponent,
    ComplianceComponent,
    SortableColumnComponent,
    SortableTableDirective,
    DownloadExcelComponent,
    ComplianceFourIndicationsPdfComponent,
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
