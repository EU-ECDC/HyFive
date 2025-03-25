import { Uuid } from '../../utils/uuid';
import { FireIndikasjonerSesjon } from '../../models/api/FireIndikasjonerSesjon';
import { Localstoragepaths } from '../../konstanter/localstoragepaths';
import { FireIndikasjonerSesjonsvisning } from '../../models/registrering/fire-indikasjoner-sesjonsvisning.model';
import { Rolle } from '../../models/api/Rolle';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { Kort } from '../../models/registrering/kort.model';
import { Avdeling } from '../../models/api/Avdeling';
import { BaseSesjonService } from './base-sesjon.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class FireIndikasjonerSesjonService extends BaseSesjonService<FireIndikasjonerSesjonsvisning, FireIndikasjonerSesjon, FireIndikasjonerObservasjon> {

  sesjonLocalStoragePath = Localstoragepaths.FireIndikasjonerSesjoner;
  sesjonsvisningLocalStoragePath = Localstoragepaths.FireIndikasjonerSesjonsvisninger;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sesjonId: string): Observable<string> {
    var sesjoner = this.hentSesjoner();
    var sesjonIndeks = sesjoner.map(s => s.id).indexOf(sesjonId);
    var sesjonSomSkalSendes = sesjoner[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/fireindikasjoner`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    hanskebrukSkalRegistreres: boolean,
    tidtakingSkalRegistreres: boolean,
    rollerSomObserveres: Rolle[],
    avdeling: Avdeling
  ): string {
    let id = Uuid.generateUUID();

    let fireIndikasjonerSesjonsvisning: FireIndikasjonerSesjonsvisning = {
      sesjonId: id,
      avdeling: avdeling,
      hanskebrukSkalRegistreres: hanskebrukSkalRegistreres,
      tidtakingSkalRegistreres: tidtakingSkalRegistreres,
      kort: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), rolle: r, erAktivt: i == 0 } as Kort }),
    }
    var sesjonsvisninger = this.hentSesjonsvisninger()
    sesjonsvisninger.push(fireIndikasjonerSesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

    return id;
  }


}
