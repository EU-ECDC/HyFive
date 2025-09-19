import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UrlPaths } from './_common/konstanter/url-paths';
import { HomePageForAdministrationComponent } from './home-page-for-administration/home-page-for-administration.component';
import { EditingOfFacilitiesComponent } from './fhiadmin/redigering-av-institusjoner/editing-of-facilities.component';
import { EditingCodeworkComponent } from './fhiadmin/redigering-av-kodeverk/editing-code-works.component';
import { OverviewObservationsComponent } from './fhiadmin/oversikt-observasjoner/overview-observations.component';
import { EditingDepartmentsComponent } from './_common/redigering-av-avdelinger/editing-of-departments.component';
import { OverviewDepartmentSessionsComponent } from './fhiadmin/oversikt-observasjoner/overview-department-sessions/overview-department-sessions.component';
import { EditingOfObserversComponent } from "./coordinator/editing-observers/editing-of-observers.component";
import { ProfilsideComponent } from "./profile-page/profile-page.component";
import { TransferSessionsComponent } from './coordinator/transfer-sessions/transfer-sessions.component';
import { EditingPredefinedCommentsComponent } from './coordinator/edit-predefined-comments/edit-predefined-comments.component';
import { EditingClinicsComponent } from "./coordinator/redigering-av-klinikker/editing-of-clinic.component";
import { OverviewFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/overview-fhiadmin.component';
import { RequestComponent } from './coordinator/request/request.component';
import { HealthEnterpriseComponent } from './fhiadmin/health-enterprise/health-enterprise.component';
import { EditingCoordinatorsComponent } from './coordinator/editing-of-coordinators/editing-of-coordinators.component';
import { EmailComponent } from './fhiadmin/email/email.component';
import { ReportComponent } from './coordinator/rapporter/report.component';
import { ComplianceComponent } from './coordinator/rapporter/fireIndikasjoner/compliance/compliance.component';
import { DownloadExcelComponent } from './coordinator/rapporter/download/download-excel.component';
import { ComplianceFiveIndicationsPdfComponent } from './coordinator/rapporter/predefined/compliance-five-indications-pdf.component';
import { ComplianceHandJewelryPdfComponent } from './coordinator/rapporter/predefined/compliance-handJewelry-pdf.component';

const defaultPath = `/${UrlPaths.homePage}`;

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: defaultPath
  },
  {
    path: UrlPaths.homePage,
    component: HomePageForAdministrationComponent
  },
  {
    path: UrlPaths.observations,
    component: OverviewObservationsComponent
  },
  {
    path: UrlPaths.observationsDepartment,
    component: OverviewDepartmentSessionsComponent
  },
  {
    path: UrlPaths.transferSessions,
    component: TransferSessionsComponent
  },
  {
    path: UrlPaths.editingByFacilities,
    component: EditingOfFacilitiesComponent
  },
  {
    path: UrlPaths.editingByCodeworks,
    component: EditingCodeworkComponent
  },
  {
    path: UrlPaths.editingOfDepartments,
    component: EditingDepartmentsComponent
  },
  {
    path: UrlPaths.editingByClinics,
    component: EditingClinicsComponent
  },
  {
    path: UrlPaths.editingByCoordinators,
    component: EditingCoordinatorsComponent
  },
  {
    path: UrlPaths.editingByObservers,
    component: EditingOfObserversComponent
  },
  {
    path: UrlPaths.editingPredefinedComments,
    component: EditingPredefinedCommentsComponent
  },
  {
    path: UrlPaths.profile,
    component: ProfilsideComponent
  },
  {
    path: UrlPaths.facilities,
    component: EditingOfFacilitiesComponent
  },
  {
    path: UrlPaths.fhiAdminOverview,
    component: OverviewFhiAdminComponent
  },
  {
    path: UrlPaths.request,
    component: RequestComponent
  },
  {
    path: UrlPaths.healthcareOrganization,
    component: HealthEnterpriseComponent
  },
  {
    path: UrlPaths.email,
    component: EmailComponent
  },
  {
    path: UrlPaths.reports,
    component: ReportComponent,
    children: [
      {
        path: UrlPaths.fiveIndicationsCompliance, component: ComplianceComponent
      },
      {
        path: UrlPaths.fiveIndicationsCompliancePdf, component: ComplianceFiveIndicationsPdfComponent
      },
      {
        path: UrlPaths.handjewelryCompliancePdf, component: ComplianceHandJewelryPdfComponent
      },
      {
        path: UrlPaths.downloadExcel, component: DownloadExcelComponent
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: UrlPaths.fiveIndicationsCompliance
      }
    ]
  },
  {
    path: '**',
    redirectTo: defaultPath
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {})],
  exports: [RouterModule]
})
export class AppRoutingModule { }
