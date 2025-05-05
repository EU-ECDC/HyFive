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
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class FireIndikasjonerSesjonService extends BaseSessionService<FourIndicationsSessionView, FourIndicationsSession, FourIndicationsObservation> {

  sessionLocalStoragePath = Localstoragepaths.FourIndicationsSessions;
  sessionShowLocalStoragePath = Localstoragepaths.FireIndicationsSessionViews;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sessionId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sessionId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/fourindications`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    gloveUseMustBeRegistered: boolean,
    timeShouldBeRegistred: boolean,
    rollerSomObserveres: Role[],
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let fireIndikasjonerSesjonsvisning: FourIndicationsSessionView = {
      sessionId: id,
      department: department,
      gloveUseMustBeRegistered: gloveUseMustBeRegistered,
      timeShouldBeRegistred: timeShouldBeRegistred,
      card: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), role: r, isActive: i == 0 } as Card }),
    }
    var sessionViews = this.hentSesjonsvisninger()
    sessionViews.push(fireIndikasjonerSesjonsvisning);
    this.saveSessionViews(sessionViews);

    return id;
  }


}
