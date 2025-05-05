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
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class HanskeSesjonService extends BaseSessionService<GloveSessionView, GloveSession, GloveObservation> {

  sessionLocalStoragePath = Localstoragepaths.GloveSessions;
  sessionShowLocalStoragePath = Localstoragepaths.HanskeSesjonsvisninger;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sessionId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sessionId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/hanske`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    gloveUseMustBeRegistered: boolean,
    rollerSomObserveres: Role[],
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let gloveSessionView: GloveSessionView = {
      sessionId: id,
      department: department,
      gloveUseMustBeRegistered: gloveUseMustBeRegistered,
      card: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    var sessionViews = this.hentSesjonsvisninger()
    sessionViews.push(gloveSessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }
}
