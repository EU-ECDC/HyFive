import { Uuid } from '../../utils/uuid';
import { Localstoragepaths } from '../../konstanter/localstoragepaths';
import { Role } from '../../models/api/Role';
import { Kort } from '../../models/registrering/kort.model';
import { Department } from '../../models/api/Department';
import { BaseSesjonService } from './base-sesjon.service';
import { HandsmykkeSesjonsvisning } from '../../models/registrering/handsmykke-sesjonsvisning.model';
import { HandsmykkeSesjon } from '../../models/api/HandsmykkeSesjon';
import { HandsmykkeObservasjon } from '../../models/api/HandsmykkeObservasjon';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class HandsmykkeSesjonService extends BaseSesjonService<HandsmykkeSesjonsvisning, HandsmykkeSesjon, HandsmykkeObservasjon> {

  sesjonLocalStoragePath = Localstoragepaths.HandsmykkeSesjoner;
  sesjonsvisningLocalStoragePath = Localstoragepaths.HandsmykkeSesjonsvisninger;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sesjonId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sesjonId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/handsmykke`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    rollerSomObserveres: Role[],
    avdeling: Department
  ): string {
    let id = Uuid.generateUUID();

    let handsmykkerSesjonsvisning: HandsmykkeSesjonsvisning = {
      sesjonId: id,
      avdeling: avdeling,
      kort: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), role: r, erAktivt: i == 0 } as Kort }),
    }
    var sesjonsvisninger = this.hentSesjonsvisninger();
    sesjonsvisninger.push(handsmykkerSesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

    return id;
  }

  public hentSesjonFraServer(sesjonId: string): Observable<HandsmykkeSesjon> {
    if (navigator.onLine) {
      let params = new HttpParams();
      params = params.append("sesjonId", sesjonId);
      return this.httpClient.get<HandsmykkeSesjon>(`${environment.apiBaseUrl}/v1/sesjon/handsmykke`, { params });
    }
    else {
      confirm("Ikke koblet til internet");
    }
  }
}
