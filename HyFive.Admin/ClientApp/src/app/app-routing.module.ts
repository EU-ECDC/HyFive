import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UrlPaths } from './_felles/konstanter/url-paths';
import { ForsideForAdministrasjonComponent } from './forside-for-administrasjon/forside-for-administrasjon.component';
import { RedigeringAvInstitusjonerComponent } from './fhiadmin/redigering-av-institusjoner/redigering-av-institusjoner.component';
import { RedigeringAvKodeverkComponent } from './fhiadmin/redigering-av-kodeverk/redigering-av-kodeverk.component';
import { OversiktObservasjonerComponent } from './fhiadmin/oversikt-observasjoner/oversikt-observasjoner.component';
import { RedigeringAvAvdelingerComponent } from './_felles/redigering-av-avdelinger/redigering-av-avdelinger.component';
import { OversiktAvdelingSesjonerComponent } from './fhiadmin/oversikt-observasjoner/oversikt-avdeling-sesjoner/oversikt-avdeling-sesjoner.component';
import { RedigeringAvObservatorerComponent } from "./koordinator/redigering-av-observatorer/redigering-av-observatorer.component";
import { ProfilsideComponent } from "./profilside/profilside.component";
import { OverforSesjonerComponent } from './koordinator/overfor-sesjoner/overfor-sesjoner.component';
import { RedigeringAvPredefinertKommentarerComponent } from './koordinator/redigering-av-predefinert-kommentarer/redigering-av-predefinert-kommentarer.component';
import { RedigeringAvKlinikkerComponent } from "./koordinator/redigering-av-klinikker/redigering-av-klinikker.component";
import { OversiktFhiAdminComponent } from './fhiadmin/oversikt-fhiadmin/oversikt-fhiadmin.component';
import { ForesporselComponent } from './koordinator/foresporsel/foresporsel.component';
import { HelseforetakComponent } from './fhiadmin/helseforetak/helseforetak.component';
import { RedigeringAvKoordinatorerComponent } from './koordinator/redigering-av-koordinatorer/redigering-av-koordinatorer.component';
import { EpostComponent } from './fhiadmin/epost/epost.component';
import { RapporterComponent } from './koordinator/rapporter/rapporter.component';
import { EtterlevelseComponent } from './koordinator/rapporter/fireIndikasjoner/etterlevelse/etterlevelse.component';
import { NedlastingExcelComponent } from './koordinator/rapporter/nedlasting/nedlasting-excel.component';
import { EtterlevelseFireIndikasjonerPdfComponent } from './koordinator/rapporter/predefinerte/etterlevelse-fire-indikasjoner-pdf.component';
import { EtterlevelseHandsmykkerPdfComponent } from './koordinator/rapporter/predefinerte/etterlevelse-handsmykker-pdf.component';

const defaultPath = `/${UrlPaths.forside}`;

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: defaultPath
  },
  {
    path: UrlPaths.forside,
    component: ForsideForAdministrasjonComponent
  },
  {
    path: UrlPaths.observasjoner,
    component: OversiktObservasjonerComponent
  },
  {
    path: UrlPaths.observasjonerAvdeling,
    component: OversiktAvdelingSesjonerComponent
  },
  {
    path: UrlPaths.overforSesjoner,
    component: OverforSesjonerComponent
  },
  {
    path: UrlPaths.redigeringAvInstitusjoner,
    component: RedigeringAvInstitusjonerComponent
  },
  {
    path: UrlPaths.redigeringAvKodeverk,
    component: RedigeringAvKodeverkComponent
  },
  {
    path: UrlPaths.redigeringAvAvdelinger,
    component: RedigeringAvAvdelingerComponent
  },
  {
    path: UrlPaths.redigeringAvKlinikker,
    component: RedigeringAvKlinikkerComponent
  },
  {
    path: UrlPaths.redigeringAvKoordinatorer,
    component: RedigeringAvKoordinatorerComponent
  },
  {
    path: UrlPaths.redigeringAvObservatorer,
    component: RedigeringAvObservatorerComponent
  },
  {
    path: UrlPaths.redigeringAvPredefinertkommentarer,
    component: RedigeringAvPredefinertKommentarerComponent
  },
  {
    path: UrlPaths.profil,
    component: ProfilsideComponent
  },
  {
    path: UrlPaths.institusjoner,
    component: RedigeringAvInstitusjonerComponent
  },
  {
    path: UrlPaths.fhiAdminOversikt,
    component: OversiktFhiAdminComponent
  },
  {
    path: UrlPaths.foresporsel,
    component: ForesporselComponent
  },
  {
    path: UrlPaths.helseforetak,
    component: HelseforetakComponent
  },
  {
    path: UrlPaths.epost,
    component: EpostComponent
  },
  {
    path: UrlPaths.rapporter,
    component: RapporterComponent,
    children: [
      {
        path: UrlPaths.fireindikasjonerEtterlevelse, component: EtterlevelseComponent
      },
      {
        path: UrlPaths.fireindikasjonerEtterlevelsePdf, component: EtterlevelseFireIndikasjonerPdfComponent
      },
      {
        path: UrlPaths.handsmykkerEtterlevelsePdf, component: EtterlevelseHandsmykkerPdfComponent
      },
      {
        path: UrlPaths.nedlastingExcel, component: NedlastingExcelComponent
      },
      {
        path: '',
        pathMatch: 'full',
        redirectTo: UrlPaths.fireindikasjonerEtterlevelsePdf
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
