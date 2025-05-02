import { Uuid } from '../../utils/uuid';
import { FourIndicationsSession } from '../../models/api/FourIndicationsSession';
import { Localstoragepaths } from '../../konstanter/localstoragepaths';
import { FireIndikasjonerSesjonsvisning } from '../../models/registrering/fire-indikasjoner-sesjonsvisning.model';
import { Role } from '../../models/api/Role';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { Kort } from '../../models/registrering/kort.model';
import { Department } from '../../models/api/Department';
import { BaseSesjonService } from './base-sesjon.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})
export class FireIndikasjonerSesjonService extends BaseSesjonService<FireIndikasjonerSesjonsvisning, FourIndicationsSession, FourIndicationsObservation> {

  sesjonLocalStoragePath = Localstoragepaths.FireIndikasjonerSesjoner;
  sesjonsvisningLocalStoragePath = Localstoragepaths.FireIndikasjonerSesjonsvisninger;

  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sessionId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sessionId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/fireindikasjoner`, sesjonSomSkalSendes)
  }

  public lagSesjonsvisning(
    hanskebrukSkalRegistreres: boolean,
    tidtakingSkalRegistreres: boolean,
    rollerSomObserveres: Role[],
    department: Department
  ): string {
    let id = Uuid.generateUUID();

    let fireIndikasjonerSesjonsvisning: FireIndikasjonerSesjonsvisning = {
      sessionId: id,
      department: department,
      hanskebrukSkalRegistreres: hanskebrukSkalRegistreres,
      tidtakingSkalRegistreres: tidtakingSkalRegistreres,
      kort: rollerSomObserveres.map((r, i) => { return { id: Uuid.generateUUID(), role: r, erAktivt: i == 0 } as Kort }),
    }
    var sesjonsvisninger = this.hentSesjonsvisninger()
    sesjonsvisninger.push(fireIndikasjonerSesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

    return id;
  }


}
