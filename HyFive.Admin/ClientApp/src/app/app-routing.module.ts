import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UrlPaths } from './_felles/konstanter/url-paths';
import { HomePageForAdministrationComponent } from './front-page-for-administration/front-page-for-administration.component';
import { EditingOfInstitutionsComponent } from './fhiadmin/redigering-av-institusjoner/editing-of-institutions.component';
import { EditingCodeworkComponent } from './fhiadmin/redigering-av-kodeverk/editing-code-works.component';
import { OverviewObservationsComponent } from './fhiadmin/oversikt-observasjoner/overview-observations.component';
import { EditingDepartmentsComponent } from './_felles/redigering-av-avdelinger/editing-of-departments.component';
import { OverviewDepartmentSessionsComponent } from './fhiadmin/oversikt-observasjoner/oversikt-avdeling-sesjoner/overview-department-sessions.component';
import { RedigeringAvObservatorerComponent } from "./koordinator/redigering-av-observatorer/redigering-av-observatorer.component";
import { ProfilsideComponent } from "./profilside/profilside.component";
import { OverforSesjonerComponent } from './koordinator/opposite-sessions/overfor-sesjoner.component';
import { RedigeringAvPredefinertKommentarerComponent } from './koordinator/redigering-av-predefinert-kommentarer/redigering-av-predefinert-kommentarer.component';
import { RedigeringAvKlinikkerComponent } from "./koordinator/redigering-av-klinikker/redigering-av-klinikker.component";
import { OverviewFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/overview-fhiadmin.component';
import { ForesporselComponent } from './koordinator/foresporsel/foresporsel.component';
import { HealthEnterpriseComponent } from './fhiadmin/health-enterprise/health-enterprise.component';
import { RedigeringAvKoordinatorerComponent } from './koordinator/redigering-av-koordinatorer/redigering-av-koordinatorer.component';
import { EmailComponent } from './fhiadmin/email/email.component';
import { RapporterComponent } from './koordinator/rapporter/rapporter.component';
import { EtterlevelseComponent } from './koordinator/rapporter/fireIndikasjoner/etterlevelse/etterlevelse.component';
import { NedlastingExcelComponent } from './koordinator/rapporter/nedlasting/nedlasting-excel.component';
import { EtterlevelseFireIndikasjonerPdfComponent } from './koordinator/rapporter/predefinerte/etterlevelse-fire-indikasjoner-pdf.component';
import { EtterlevelseHandsmykkerPdfComponent } from './koordinator/rapporter/predefinerte/etterlevelse-handsmykker-pdf.component';

const defaultPath = `/${UrlPaths.frontPage}`;

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: defaultPath
  },
  {
    path: UrlPaths.frontPage,
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
    path: UrlPaths.oppositeSessions,
    component: OverforSesjonerComponent
  },
  {
    path: UrlPaths.editingByInstitutions,
    component: EditingOfInstitutionsComponent
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
    component: RedigeringAvKlinikkerComponent
  },
  {
    path: UrlPaths.editingByCoordinators,
    component: RedigeringAvKoordinatorerComponent
  },
  {
    path: UrlPaths.editingByObservers,
    component: RedigeringAvObservatorerComponent
  },
  {
    path: UrlPaths.editingPredefinedComments,
    component: RedigeringAvPredefinertKommentarerComponent
  },
  {
    path: UrlPaths.profile,
    component: ProfilsideComponent
  },
  {
    path: UrlPaths.institutions,
    component: EditingOfInstitutionsComponent
  },
  {
    path: UrlPaths.fhiAdminOverview,
    component: OverviewFhiAdminComponent
  },
  {
    path: UrlPaths.request,
    component: ForesporselComponent
  },
  {
    path: UrlPaths.healthcareEnterprise,
    component: HealthEnterpriseComponent
  },
  {
    path: UrlPaths.email,
    component: EmailComponent
  },
  {
    path: UrlPaths.reports,
    component: RapporterComponent,
    children: [
      {
        path: UrlPaths.fourindicationsCompliance, component: EtterlevelseComponent
      },
      {
        path: UrlPaths.fourindicationsCompliancePdf, component: EtterlevelseFireIndikasjonerPdfComponent
      },
      {
        path: UrlPaths.handjewelryCompliancePdf, component: EtterlevelseHandsmykkerPdfComponent
      },
      {
        path: UrlPaths.downloadExcel, component: NedlastingExcelComponent
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: UrlPaths.fourindicationsCompliancePdf
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
