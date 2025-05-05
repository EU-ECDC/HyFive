import { Uuid } from '../../utils/uuid';
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { Role } from '../../models/api/Role';
import { Kort } from '../../models/registrering/kort.model';
import { Department } from '../../models/api/Department';
import { BaseSesjonService } from './base-sesjon.service';
import { HandsmykkeSesjonsvisning } from '../../models/registrering/handsmykke-sesjonsvisning.model';
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
export class HandsmykkeSesjonService extends BaseSesjonService<HandsmykkeSesjonsvisning, HandJewelrySession, HandJewelryObservation> {

  sesjonLocalStoragePath = Localstoragepaths.HandJewelrySessions;
  sesjonsvisningLocalStoragePath = Localstoragepaths.HandJewelrySessionViews;

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

    let handsmykkerSesjonsvisning: HandsmykkeSesjonsvisning = {
      sessionId: id,
      department: department,
      kort: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), role: r, erAktivt: i == 0 } as Kort }),
    }
    var sesjonsvisninger = this.hentSesjonsvisninger();
    sesjonsvisninger.push(handsmykkerSesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

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
