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
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class HandsmykkeSesjonService extends BaseSessionService<HandJewelrySessionView, HandJewelrySession, HandJewelryObservation> {

  sessionLocalStoragePath = Localstoragepaths.HandJewelrySessions;
  sessionShowLocalStoragePath = Localstoragepaths.HandJewelrySessionViews;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sessionId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sessionId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/handsmykke`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    rollerSomObserveres: Role[],
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let handsmykkerSesjonsvisning: HandJewelrySessionView = {
      sessionId: id,
      department: department,
      card: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    var sessionViews = this.hentSesjonsvisninger();
    sessionViews.push(handsmykkerSesjonsvisning);
    this.saveSessionViews(sessionViews);

    return id;
  }

  public hentSesjonFraServer(sessionId: string): Observable<HandJewelrySession> {
    if (navigator.onLine) {
      let params = new HttpParams();
      params = params.append("sessionId", sessionId);
      return this.httpClient.get<HandJewelrySession>(`${environment.apiBaseUrl}/v1/sesjon/handsmykke`, { params });
    }
    else {
      confirm("Ikke koblet til internet");
    }
  }
}
