import { Uuid } from '../../utils/uuid';
import { Localstoragepaths } from '../../konstanter/localstoragepaths';
import { Rolle } from '../../models/api/Rolle';
import { Kort } from '../../models/registrering/kort.model';
import { Avdeling } from '../../models/api/Avdeling';
import { BaseSesjonService } from './base-sesjon.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HanskeSesjonsvisning } from '../../models/registrering/hansker-sesjonsvisning.model';
import { HanskeSesjon } from '../../models/api/HanskeSesjon';
import { HanskeObservasjon } from '../../models/api/HanskeObservasjon';
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class HanskeSesjonService extends BaseSesjonService<HanskeSesjonsvisning, HanskeSesjon, HanskeObservasjon> {

  sesjonLocalStoragePath = Localstoragepaths.HanskeSesjoner;
  sesjonsvisningLocalStoragePath = Localstoragepaths.HanskeSesjonsvisninger;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sesjonId: string): Observable<string> {
    var sesjoner = this.hentSesjoner();
    var sesjonIndeks = sesjoner.map(s => s.id).indexOf(sesjonId);
    var sesjonSomSkalSendes = sesjoner[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/hanske`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    hanskebrukSkalRegistreres: boolean,
    rollerSomObserveres: Rolle[],
    avdeling: Avdeling
  ): string {
    let id = Uuid.generateUUID();

    let hanskeSesjonsvisning: HanskeSesjonsvisning = {
      sesjonId: id,
      avdeling: avdeling,
      hanskebrukSkalRegistreres: hanskebrukSkalRegistreres,
      kort: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), rolle: r, erAktivt: i == 0 } as Kort }),
    }
    var sesjonsvisninger = this.hentSesjonsvisninger()
    sesjonsvisninger.push(hanskeSesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

    return id;
  }
}
