import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Urls } from './constants/urls';
import { LoginPageComponent } from './login-page/loginpage.component';
import { HomePageForObservationComponent } from './startside/home-page-observation.component';
import { RegisterFiveIndicationsComponent } from './registrering/register-five-indications/register-five-indications.component';
import { RegisterHandjewelryComponent } from './registrering/register-hand-jewelry/register-hand-jewelry.component';
import { RegisterProtectiveEquipmentComponent } from './registrering/register-protective-equipment/register-protective-equipment.component';
import { RegisterGloveComponent } from './registrering/register-glove/register-glove.component';
import { NotSentSessionsComponent } from './sessions/not-sent-sessions/not-sent-sessions.component';
import { SentSessionsComponent } from './sessions/sent-sessions/sent-sessions.component';
import { FiveIndicationsComponent } from './sessions/five-indications/five-indications.component';
import { HandJewelryComponent } from './sessions/handJewelry/handJewelry.component';
import { ProtectiveEquipmentComponent } from './sessions/protection-equipment/protection-equipment.component';
import { SentFiveIndicationsSessionComponent } from './sessions/sent-sessions/sent-five-indications-session/sent-five-indications-session.component';
import { SentHandJewelrySessionComponent } from './sessions/sent-sessions/sent-hand-jewelry-session/sent-hand-jewelry-session.component';
import { SentProtectiveEquipmentSessionComponent } from './sessions/sent-sessions/sent-protective-equipment-session/sent-protective-equipment-session.component';
import { GloveComponent } from './sessions/glove/glove.component';
import { SentGloveSessionComponent } from './sessions/sent-sessions/sent-glove-session/sent-glove-session.component';

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
  { path: Urls.RegisterFiveIndicationsUrl, component: RegisterFiveIndicationsComponent },
  { path: Urls.NotSentSessionsUrl, component: NotSentSessionsComponent },
  { path: Urls.SentSessionsUrl, component: SentSessionsComponent },
  { path: Urls.FiveIndicationsSessionUrl, component: FiveIndicationsComponent },
  { path: Urls.RegisterHandJewelryUrl, component: RegisterHandjewelryComponent },
  { path: Urls.HandJewelrySessionUrl, component: HandJewelryComponent },
  { path: Urls.RegisterProtectiveEquipmentUrl, component: RegisterProtectiveEquipmentComponent },
  { path: Urls.ProtectiveEquipmentSessionUrl, component: ProtectiveEquipmentComponent },
  { path: Urls.SentFiveIndicationsSessionUrl, component: SentFiveIndicationsSessionComponent },
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
