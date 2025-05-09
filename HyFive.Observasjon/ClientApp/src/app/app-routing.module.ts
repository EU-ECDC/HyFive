import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Urls } from './constants/urls';
import { LoginPageComponent } from './login-page/loginpage.component';
import { HomePageForObservationComponent } from './startside/home-page-observation.component';
import { RegisterFourIndicationsComponent } from './registrering/register-four-indications/register-four-indications.component';
import { RegisterHandjewelryComponent } from './registrering/register-hand-jewelry/register-hand-jewelry.component';
import { RegisterProtectiveEquipmentComponent } from './registrering/register-protective-equipment/register-protective-equipment.component';
import { RegisterGloveComponent } from './registrering/register-glove/register-glove.component';
import { NotSentSessionsComponent } from './sesjoner/not-sent-sessions/not-sent-sessions.component';
import { SentSessionsComponent } from './sesjoner/sent-sessions/sent-sessions.component';
import { FourIndicationsComponent } from './sesjoner/fire-indikasjoner/four-indications.component';
import { HandJewelryComponent } from './sesjoner/handJewelry/handJewelry.component';
import { ProtectiveEquipmentComponent } from './sesjoner/protection-equipment/protection-equipment.component';
import { SentFourIndicationsSessionComponent } from './sesjoner/sent-sessions/sent-four-indications-session/sent-four-indications-session.component';
import { SentHandJewelrySessionComponent } from './sesjoner/sent-sessions/sent-hand-jewelry-session/sent-hand-jewelry-session.component';
import { SentProtectiveEquipmentSessionComponent } from './sesjoner/sent-sessions/sent-protective-equipment-session/sent-protective-equipment-session.component';
import { GloveComponent } from './sesjoner/glove/glove.component';
import { SentGloveSessionComponent } from './sesjoner/sent-sessions/sent-glove-session/sent-glove-session.component';

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
    component: HomePageForObservationComponent,
    runGuardsAndResolvers: 'always',
  },
  { path: Urls.RegisterFourndicationsUrl, component: RegisterFourIndicationsComponent },
  { path: Urls.NotSentSessionsUrl, component: NotSentSessionsComponent },
  { path: Urls.SentSessionsUrl, component: SentSessionsComponent },
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
