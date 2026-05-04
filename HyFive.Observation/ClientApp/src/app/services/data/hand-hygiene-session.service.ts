import { Uuid } from '../../utils/uuid';
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { Role } from '../../models/api/Role';
import { HandHygieneObservation } from '../../models/api/HandHygieneObservation';
import { Card } from '../../models/registration/card.model';
import { BaseSessionService } from './base-session.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { FacilityService } from './FacilityService';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { HandHygieneSessionView } from 'src/app/models/registration/hand-hygiene-session-view.model';
import { HandHygieneSession } from 'src/app/models/api/HandHygieneSession';

@Injectable({
  providedIn: 'root'
})
export class HandHygieneSessionService extends BaseSessionService<HandHygieneSessionView, HandHygieneSession, HandHygieneObservation> {

  sessionLocalStoragePath = Localstoragepaths.HandHygieneSessions;
  sessionShowLocalStoragePath = Localstoragepaths.HandHygieneSessionView;

  constructor(
    public facilityService: FacilityService,
    private readonly httpClient: HttpClient) {
    super(facilityService);
  }

  public sendToServer(sessionId: string): Observable<string> {
    let sessions = this.getSessions();
    let sessionIndex = sessions.map(s => s.id).indexOf(sessionId);
    let sessionToSend = sessions[sessionIndex];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/fiveindications`, sessionToSend)
  }

  public createSessionView(
    gloveUseMustBeRegistered: boolean,
    timeShouldBeRegistred: boolean,
    rolesAsObserved: Role[],
    department: OrganisationUnit,
    unit: OrganisationUnit
  ): string {
    let id = Uuid.generateUUID();

    let handHygieneSessionView: HandHygieneSessionView = {
      sessionId: id,
      department: department,
      unit: unit,
      gloveUseMustBeRegistered: gloveUseMustBeRegistered,
      timeShouldBeRegistred: timeShouldBeRegistred,
      card: rolesAsObserved.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    let sessionViews = this.getSessionViews()
    sessionViews.push(handHygieneSessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }


}
