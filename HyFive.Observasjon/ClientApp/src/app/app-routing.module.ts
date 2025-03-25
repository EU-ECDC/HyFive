import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Urls } from './konstanter/urls';
import { LoginsideComponent } from './loginside/loginside.component';
import { StartsideForObservasjonComponent } from './startside/startside-for-observasjon.component';
import { RegistrereFireIndikasjonerComponent } from './registrering/registrere-fire-indikasjoner/registrere-fire-indikasjoner.component';
import { IkkeSendteSesjonerComponent } from './sesjoner/ikke-sendte-sesjoner/ikke-sendte-sesjoner.component';
import { SendteSesjonerComponent } from './sesjoner/sendte-sesjoner/sendte-sesjoner.component';
import { FireIndikasjonerComponent } from './sesjoner/fire-indikasjoner/fire-indikasjoner.component';
import { RegistrereHandsmykkerComponent } from './registrering/registrere-handsmykker/registrere-handsmykker.component';
import { HandsmykkerComponent } from './sesjoner/handsmykker/handsmykker.component';
import { RegistrereBeskyttelsesutstyrComponent } from './registrering/registrere-beskyttelsesutstyr/registrere-beskyttelsesutstyr.component';
import { BeskyttelsesutstyrComponent } from './sesjoner/beskyttelsesutstyr/beskyttelsesutstyr.component';
import { SendteFireIndikasjonerSesjonComponent } from './sesjoner/sendte-sesjoner/sendte-fire-indikasjoner-sesjon/sendte-fire-indikasjoner-sesjon.component';
import { SendteHandsmykkerSesjonComponent } from './sesjoner/sendte-sesjoner/sendte-handsmykker-sesjon/sendte-handsmykker-sesjon.component';
import { SendteBeskyttelsesutstyrSesjonComponent } from './sesjoner/sendte-sesjoner/sendte-beskyttelsesutstyr-sesjon/sendte-beskyttelsesutstyr-sesjon.component';
import { RegistrereHanskeComponent } from './registrering/registrere-hanske/registrere-hanske.component';
import { HanskeComponent } from './sesjoner/hanske/hanske.component';
import { SendteHanskeSesjonComponent } from './sesjoner/sendte-sesjoner/sendte-hanske-sesjon/sendte-hanske-sesjon.component';

const defaultPath = Urls.ProfilUrl;

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: defaultPath
  },
  { path: Urls.ProfilUrl, component: LoginsideComponent },
  {
    path: Urls.StartsideForObservasjonUrl,
    component: StartsideForObservasjonComponent,
    runGuardsAndResolvers: 'always',
  },
  { path: Urls.RegistrereFireIndikasjonerUrl, component: RegistrereFireIndikasjonerComponent },
  { path: Urls.IkkeSendteSesjonerUrl, component: IkkeSendteSesjonerComponent },
  { path: Urls.SendteSesjonerUrl, component: SendteSesjonerComponent },
  { path: Urls.FireIndikasjonerSesjonUrl, component: FireIndikasjonerComponent },
  { path: Urls.RegistrereHandsmykkerUrl, component: RegistrereHandsmykkerComponent },
  { path: Urls.HandsmykkeSesjonUrl, component: HandsmykkerComponent },
  { path: Urls.RegistrereBeskyttelsesutstyrUrl, component: RegistrereBeskyttelsesutstyrComponent },
  { path: Urls.BeskyttelsesutstyrSesjonUrl, component: BeskyttelsesutstyrComponent },
  { path: Urls.SendteFireIndikasjonerSesjonUrl, component: SendteFireIndikasjonerSesjonComponent },
  { path: Urls.SendteHandsmykkeSesjonUrl, component: SendteHandsmykkerSesjonComponent },
  { path: Urls.SendteBeskyttelsesutstyrSesjonUrl, component: SendteBeskyttelsesutstyrSesjonComponent },
  { path: Urls.RegistrereHanskeUrl, component: RegistrereHanskeComponent },
  { path: Urls.HanskeSesjonUrl, component: HanskeComponent },
  { path: Urls.SendteHanskeSesjonUrl, component: SendteHanskeSesjonComponent },
  {
    path: '**',
    redirectTo: defaultPath
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
