import { Uuid } from '../../utils/uuid';
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { Role } from '../../models/api/Role';
import { Card } from '../../models/registration/card.model';
import { Department } from '../../models/api/Department';
import { BaseSessionService } from './base-session.service';
import { HandJewelrySessionView } from '../../models/registration/handJewelry-session-view.model';
import { HandJewelrySession } from '../../models/api/HandJewelrySession';
import { HandJewelryObservation } from '../../models/api/HandJewelryObservation';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { FacilityService } from './FacilityService';

@Injectable({
  providedIn: 'root'
})
export class HandJewelrySessionService extends BaseSessionService<HandJewelrySessionView, HandJewelrySession, HandJewelryObservation> {

  sessionLocalStoragePath = Localstoragepaths.HandJewelrySessions;
  sessionShowLocalStoragePath = Localstoragepaths.HandJewelrySessionViews;

  constructor(
    public facilityService: FacilityService,
    private readonly httpClient: HttpClient) {
    super(facilityService);
  }

  public sendToServer(sessionId: string): Observable<string> {
    let sessions = this.getSessions();
    let sessionIndex = sessions.map(s => s.id).indexOf(sessionId);
    let sessionToSend = sessions[sessionIndex];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/handjewelry`, sessionToSend)
  }

  public createSessionView(
    rolesAsObserved: Role[],
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let handJewelrySessionView: HandJewelrySessionView = {
      sessionId: id,
      department: department,
      card: rolesAsObserved.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    let sessionViews = this.getSessionViews();
    sessionViews.push(handJewelrySessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }

  public getSessionFromServer(sessionId: string): Observable<HandJewelrySession> {
    if (navigator.onLine) {
      let params = new HttpParams();
      params = params.append("sessionId", sessionId);
      return this.httpClient.get<HandJewelrySession>(`${environment.apiBaseUrl}/v1/session/handjewelry`, { params });
    }
    else {
      confirm("Not connected to the internet");
    }
  }
}
