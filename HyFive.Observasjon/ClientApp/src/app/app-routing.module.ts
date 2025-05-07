import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Urls } from './constants/urls';
import { LoginPageComponent } from './login-page/loginpage.component';
import { StartsideForObservasjonComponent } from './startside/startside-for-observasjon.component';
import { RegisterFourIndicationsComponent } from './registrering/register-four-indications/register-four-indications.component';
import { NotSentSessionsComponent } from './sessions/ikke-sendte-sessions/ikke-sendte-sessions.component';
import { SendteSesjonerComponent } from './sessions/sendte-sessions/sendte-sessions.component';
import { FourIndicationsComponent } from './sessions/fire-indikasjoner/fire-indikasjoner.component';
import { RegisterHandjewelryComponent } from './registrering/register-hand-jewelry/register-hand-jewelry.component';
import { HandJewelryComponent } from './sessions/handsmykker/handsmykker.component';
import { RegisterProtectiveEquipmentComponent } from './registrering/register-protective-equipment/register-protective-equipment.component';
import { ProtectiveEquipmentComponent } from './sessions/beskyttelsesutstyr/beskyttelsesutstyr.component';
import { SentFourIndicationsSessionComponent } from './sessions/sendte-sessions/sendte-fire-indikasjoner-sesjon/sendte-fire-indikasjoner-sesjon.component';
import { SentHandJewelrySessionComponent } from './sessions/sendte-sessions/sendte-handsmykker-sesjon/sendte-handsmykker-sesjon.component';
import { SentProtectiveEquipmentSessionComponent } from './sessions/sendte-sessions/sendte-beskyttelsesutstyr-sesjon/sendte-beskyttelsesutstyr-sesjon.component';
import { RegisterGloveComponent } from './registrering/register-glove/register-glove.component';
import { GloveComponent } from './sessions/hanske/hanske.component';
import { SentGloveSessionComponent } from './sessions/sendte-sessions/sendte-hanske-sesjon/sendte-hanske-sesjon.component';

const defaultPath = Urls.ProfileUrl;

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: defaultPath
  },
  { path: Urls.ProfileUrl, component: LoginPageComponent },
  {
    path: Urls.HomePageForObservationUrl,
    component: StartsideForObservasjonComponent,
    runGuardsAndResolvers: 'always',
  },
  { path: Urls.RegisterFourndicationsUrl, component: RegisterFourIndicationsComponent },
  { path: Urls.NotSentSessionsUrl, component: NotSentSessionsComponent },
  { path: Urls.SentSessionsUrl, component: SendteSesjonerComponent },
  { path: Urls.FourIndicationsSessionUrl, component: FourIndicationsComponent },
  { path: Urls.RegisterHandJewelryUrl, component: RegisterHandjewelryComponent },
  { path: Urls.HandJewelrySessionUrl, component: HandJewelryComponent },
  { path: Urls.RegisterProtectiveEquipmentUrl, component: RegisterProtectiveEquipmentComponent },
  { path: Urls.ProtectiveEquipmentSessionUrl, component: ProtectiveEquipmentComponent },
  { path: Urls.SentFourIndicationsSessionUrl, component: SentFourIndicationsSessionComponent },
  { path: Urls.SentHandJewelrySessionUrl, component: SentHandJewelrySessionComponent },
  { path: Urls.SendProtectiveEquipmentSessionUrl, component: SentProtectiveEquipmentSessionComponent },
  { path: Urls.RegisterGloveUrl, component: RegisterGloveComponent },
  { path: Urls.GloveSessionUrl, component: GloveComponent },
  { path: Urls.SentGloveSessionUrl, component: SentGloveSessionComponent },
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
