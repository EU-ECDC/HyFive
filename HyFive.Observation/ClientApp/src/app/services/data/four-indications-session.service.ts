import { Uuid } from '../../utils/uuid';
import { FourIndicationsSession } from '../../models/api/FourIndicationsSession';
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { FourIndicationsSessionView } from '../../models/registration/FourIndications-session-view.model';
import { Role } from '../../models/api/Role';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { Card } from '../../models/registration/card.model';
import { Department } from '../../models/api/Department';
import { BaseSessionService } from './base-session.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { InstitutionService } from './InstitutionService';

@Injectable({
  providedIn: 'root'
})
export class FourIndicationsSessionService extends BaseSessionService<FourIndicationsSessionView, FourIndicationsSession, FourIndicationsObservation> {

  sessionLocalStoragePath = Localstoragepaths.FourIndicationsSessions;
  sessionShowLocalStoragePath = Localstoragepaths.FourIndicationsSessionView;

  constructor(
    public institutionService: InstitutionService,
    private httpClient: HttpClient) {
    super(institutionService);
  }

  public sendToServer(sessionId: string): Observable<string> {
    var sessions = this.getSessions();
    var sessionIndex = sessions.map(s => s.id).indexOf(sessionId);
    var sessionToSend = sessions[sessionIndex];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/fiveindications`, sessionToSend)
  }

  public createSessionView(
    gloveUseMustBeRegistered: boolean,
    timeShouldBeRegistred: boolean,
    rolesAsObserved: Role[],
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let fourIndicationsSessionView: FourIndicationsSessionView = {
      sessionId: id,
      department: department,
      gloveUseMustBeRegistered: gloveUseMustBeRegistered,
      timeShouldBeRegistred: timeShouldBeRegistred,
      card: rolesAsObserved.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    var sessionViews = this.getSessionViews()
    sessionViews.push(fourIndicationsSessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }


}
