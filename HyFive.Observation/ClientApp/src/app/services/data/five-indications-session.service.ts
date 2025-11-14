import { Uuid } from '../../utils/uuid';
import { FiveIndicationsSession } from '../../models/api/FiveIndicationsSession';
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { FiveIndicationsSessionView } from '../../models/registration/FiveIndications-session-view.model';
import { Role } from '../../models/api/Role';
import { FiveIndicationsObservation } from '../../models/api/FiveIndicationsObservation';
import { Card } from '../../models/registration/card.model';
import { Department } from '../../models/api/Department';
import { BaseSessionService } from './base-session.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { FacilityService } from './FacilityService';

@Injectable({
  providedIn: 'root'
})
export class FiveIndicationsSessionService extends BaseSessionService<FiveIndicationsSessionView, FiveIndicationsSession, FiveIndicationsObservation> {

  sessionLocalStoragePath = Localstoragepaths.FiveIndicationsSessions;
  sessionShowLocalStoragePath = Localstoragepaths.FiveIndicationsSessionView;

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
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let fiveIndicationsSessionView: FiveIndicationsSessionView = {
      sessionId: id,
      department: department,
      gloveUseMustBeRegistered: gloveUseMustBeRegistered,
      timeShouldBeRegistred: timeShouldBeRegistred,
      card: rolesAsObserved.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    let sessionViews = this.getSessionViews()
    sessionViews.push(fiveIndicationsSessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }


}
