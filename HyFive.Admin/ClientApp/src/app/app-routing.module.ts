import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UrlPaths } from './_common/constants/url-paths';
import { HomePageForAdministrationComponent } from './home-page-for-administration/home-page-for-administration.component';
import { EditingOfFacilitiesComponent } from './admin/edit-facilities/editing-of-facilities.component';
import { EditingCodeworkComponent } from './admin/edit-codeworks/editing-code-works.component';
import { OverviewObservationsComponent } from './admin/overview-observations/overview-observations.component';
import { EditingDepartmentsComponent } from './_common/editing-of-departments/editing-of-departments.component';
import { OverviewDepartmentSessionsComponent } from './admin/overview-observations/overview-department-sessions/overview-department-sessions.component';
import { EditingOfObserversComponent } from "./coordinator/editing-observers/editing-of-observers.component";
import { ProfilsideComponent } from "./profile-page/profile-page.component";
import { TransferSessionsComponent } from './coordinator/transfer-sessions/transfer-sessions.component';
import { EditingPredefinedCommentsComponent } from './coordinator/edit-predefined-comments/edit-predefined-comments.component';
import { EditingUnitsComponent } from "./coordinator/edit-units/editing-of-unit.component";
import { OverviewAdminComponent } from './admin/overview-admin/overview-admin.component';
import { EditingCoordinatorsComponent } from './coordinator/editing-of-coordinators/editing-of-coordinators.component';
import { EmailComponent } from './admin/email/email.component';
import { ReportComponent } from './coordinator/reports/report.component';
import { ComplianceComponent } from './coordinator/reports/hand-hygiene/compliance/compliance.component';
import { DownloadExcelComponent } from './coordinator/reports/download/download-excel.component';
import { ComplianceHandHygienePdfComponent } from './coordinator/reports/predefined/compliance-hand-hygiene-pdf.component';
import { ComplianceHandJewelryPdfComponent } from './coordinator/reports/predefined/compliance-handJewelry-pdf.component';

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
    path: UrlPaths.observationsUnit,
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
    path: UrlPaths.editingByUnits,
    component: EditingUnitsComponent
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
    path: UrlPaths.adminOverview,
    component: OverviewAdminComponent
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
        path: UrlPaths.handHygieneCompliance, component: ComplianceComponent
      },
      {
        path: UrlPaths.handHygieneCompliancePdf, component: ComplianceHandHygienePdfComponent
      },
      {
        path: UrlPaths.barebelowelbowsCompliancePdf, component: ComplianceHandJewelryPdfComponent
      },
      {
        path: UrlPaths.downloadExcel, component: DownloadExcelComponent
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: UrlPaths.handHygieneCompliance
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
