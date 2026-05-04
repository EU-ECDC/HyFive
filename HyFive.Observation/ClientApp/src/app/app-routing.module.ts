import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Urls } from './constants/urls';
import { LoginPageComponent } from './login-page/loginpage.component';
import { HomePageForObservationComponent } from './startside/home-page-observation.component';
import { RegisterBareBelowElbowsComponent } from './registrering/register-bare-below-elbows/register-bare-below-elbows.component';
import { RegisterProtectiveEquipmentComponent } from './registrering/register-protective-equipment/register-protective-equipment.component';
import { RegisterGloveComponent } from './registrering/register-glove/register-glove.component';
import { NotSentSessionsComponent } from './sessions/not-sent-sessions/not-sent-sessions.component';
import { SentSessionsComponent } from './sessions/sent-sessions/sent-sessions.component';
import { HandJewelryComponent } from './sessions/handJewelry/handJewelry.component';
import { ProtectiveEquipmentComponent } from './sessions/protection-equipment/protection-equipment.component';
import { SentHandJewelrySessionComponent } from './sessions/sent-sessions/sent-hand-jewelry-session/sent-hand-jewelry-session.component';
import { SentProtectiveEquipmentSessionComponent } from './sessions/sent-sessions/sent-protective-equipment-session/sent-protective-equipment-session.component';
import { GloveComponent } from './sessions/glove/glove.component';
import { SentGloveSessionComponent } from './sessions/sent-sessions/sent-glove-session/sent-glove-session.component';
import { RegisterHandHygieneComponent } from './registrering/register-hand-hygiene/register-hand-hygiene.component';
import { HandHygieneComponent } from './sessions/hand-hygiene/hand-hygiene.component';
import { SentHandHygieneSessionComponent } from './sessions/sent-sessions/sent-hand-hygiene-session/sent-hand-hygiene-session.component';

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
  { path: Urls.RegisterHandHygieneUrl, component: RegisterHandHygieneComponent },
  { path: Urls.NotSentSessionsUrl, component: NotSentSessionsComponent },
  { path: Urls.SentSessionsUrl, component: SentSessionsComponent },
  { path: Urls.HandHygieneSessionUrl, component: HandHygieneComponent },
  { path: Urls.RegisterBareBelowElbowsUrl, component: RegisterBareBelowElbowsComponent },
  { path: Urls.BareBelowElbowsSessionUrl, component: HandJewelryComponent },
  { path: Urls.RegisterProtectiveEquipmentUrl, component: RegisterProtectiveEquipmentComponent },
  { path: Urls.ProtectiveEquipmentSessionUrl, component: ProtectiveEquipmentComponent },
  { path: Urls.SentHandHygieneSessionUrl, component: SentHandHygieneSessionComponent },
  { path: Urls.SentBareBelowElbowsSessionUrl, component: SentHandJewelrySessionComponent },
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
