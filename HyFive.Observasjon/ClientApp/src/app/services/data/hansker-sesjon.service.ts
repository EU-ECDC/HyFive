import { Uuid } from '../../utils/uuid';
import { Localstoragepaths } from '../../konstanter/localstoragepaths';
import { Role } from '../../models/api/Role';
import { Kort } from '../../models/registrering/kort.model';
import { Department } from '../../models/api/Department';
import { BaseSesjonService } from './base-sesjon.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HanskeSesjonsvisning } from '../../models/registrering/hansker-sesjonsvisning.model';
import { GloveSession } from '../../models/api/GloveSession';
import { HanskeObservasjon } from '../../models/api/HanskeObservasjon';
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class HanskeSesjonService extends BaseSesjonService<HanskeSesjonsvisning, GloveSession, HanskeObservasjon> {

  sesjonLocalStoragePath = Localstoragepaths.HanskeSesjoner;
  sesjonsvisningLocalStoragePath = Localstoragepaths.HanskeSesjonsvisninger;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sesjonId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sesjonId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/hanske`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    hanskebrukSkalRegistreres: boolean,
    rollerSomObserveres: Role[],
    avdeling: Department
  ): string {
    let id = Uuid.generateUUID();

    let hanskeSesjonsvisning: HanskeSesjonsvisning = {
      sesjonId: id,
      avdeling: avdeling,
      hanskebrukSkalRegistreres: hanskebrukSkalRegistreres,
      kort: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), role: r, erAktivt: i == 0 } as Kort }),
    }
    var sesjonsvisninger = this.hentSesjonsvisninger()
    sesjonsvisninger.push(hanskeSesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

    return id;
  }
}
