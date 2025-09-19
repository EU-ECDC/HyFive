import { Uuid } from '../../utils/uuid';
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { Role } from '../../models/api/Role';
import { Card } from '../../models/registration/card.model';
import { Department } from '../../models/api/Department';
import { BaseSessionService } from './base-session.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { GloveSessionView } from '../../models/registration/glove-session-view.model';
import { GloveSession } from '../../models/api/GloveSession';
import { GloveObservation } from '../../models/api/GloveObservation';
import { FacilityService } from './FacilityService';

@Injectable({
  providedIn: 'root'
})
export class GloveSessionService extends BaseSessionService<GloveSessionView, GloveSession, GloveObservation> {

  sessionLocalStoragePath = Localstoragepaths.GloveSessions;
  sessionShowLocalStoragePath = Localstoragepaths.GloveSessionViews;

  constructor(
    public facilityService: FacilityService,
    private httpClient: HttpClient) {
    super(facilityService);
  }

  public sendToServer(sessionId: string): Observable<string> {
    var sessions = this.getSessions();
    var sessionIndex = sessions.map(s => s.id).indexOf(sessionId);
    var sessionToSend = sessions[sessionIndex];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/glove`, sessionToSend)
  }

  public createSessionView(
    gloveUseMustBeRegistered: boolean,
    rolesAsObserved: Role[],
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let gloveSessionView: GloveSessionView = {
      sessionId: id,
      department: department,
      gloveUseMustBeRegistered: gloveUseMustBeRegistered,
      card: rolesAsObserved.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    var sessionViews = this.getSessionViews()
    sessionViews.push(gloveSessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }
}
