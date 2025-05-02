import {EventEmitter, Injectable} from "@angular/core";
import { BaseSesjonService } from './base-sesjon.service';
import { BeskyttelsesutstyrSesjonsvisning } from '../../models/registrering/beskyttelsesutstyr-sesjonsvisning.model';
import { BeskyttelsesutstyrSesjon } from "src/app/models/api/BeskyttelsesutstyrSesjon";
import { ProtectiveEquipmentObservation } from "src/app/models/api/ProtectiveEquipmentObservation";
import { Role } from "src/app/models/api/Role";
import { Department } from "src/app/models/api/Department";
import { Uuid } from "src/app/utils/uuid";
import { ProtectiveEquipmentSettingType } from '../../models/api/ProtectiveEquipmentSettingType';
import { Kort } from '../../models/registrering/kort.model';
import { BeskyttelsesutstyrKort } from "src/app/models/registrering/beskyttelsesutstyr-kort.model";
import { Localstoragepaths } from '../../konstanter/localstoragepaths';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { InstitusjonService } from "./institusjon.service";
import {ProtectiveEquipmentType} from "../../models/api/ProtectiveEquipmentType";
import {ProtectiveEquipment} from "../../models/api/ProtectiveEquipment";

@Injectable({
  providedIn: 'root'
})
export class BeskyttelsesutstyrSesjonService extends BaseSesjonService<BeskyttelsesutstyrSesjonsvisning, BeskyttelsesutstyrSesjon, ProtectiveEquipmentObservation>{
  sesjonsvisningLocalStoragePath: string = Localstoragepaths.BeskyttelsesutstyrSesjonsvisninger;
  sesjonLocalStoragePath: string = Localstoragepaths.BeskyttelsesutstyrSesjoner;



  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sesjonId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sesjonId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/beskyttelsesutstyr`, sesjonSomSkalSendes)
  }

  beskyttelsesutstyrOppdatert: EventEmitter<ProtectiveEquipmentType[]> = new EventEmitter<ProtectiveEquipmentType[]>()

  lagSesjonsvisning(
    rollerSomObserveres: Role[],
    department: Department,
    setting: ProtectiveEquipmentSettingType): string {
    let id = Uuid.generateUUID();

    let sesjonsvisning: BeskyttelsesutstyrSesjonsvisning = {
      sesjonId: id,
      department: department,
      kort: this.genererKort(rollerSomObserveres, setting),
      setting: setting
    };

    var sesjonsvisninger = this.hentSesjonsvisninger();
    sesjonsvisninger.push(sesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

    return id;
  }

  oppdaterSesjonUtstyrstyper(sesjonId: string, equipmentTypes: ProtectiveEquipmentType[]){
    var sesjonsvisninger = this.hentSesjonsvisninger();
    var sesjonsvisningSomSkalOppdateres = sesjonsvisninger.find(s => s.sesjonId == sesjonId);
    sesjonsvisningSomSkalOppdateres.setting.equipmentTypes = equipmentTypes;
    this.lagreSesjonsvisninger(sesjonsvisninger);
    this.beskyttelsesutstyrOppdatert.emit(equipmentTypes);
  }

  antallKvalifisertUtstyr(beskyttelsesutstyrListe: ProtectiveEquipment[]) : number{
    return beskyttelsesutstyrListe.reduce((kvalifiserteUtstyr, curr) => {
        if (curr.isRequired || curr.wasUsed){
          return kvalifiserteUtstyr + 1;
        }
        else {
          return kvalifiserteUtstyr;
        }
      },
      0);
  }

  private genererKort(roles: Role[], setting: ProtectiveEquipmentSettingType): BeskyttelsesutstyrKort[] {
    return roles.map((r, i) => {
      return { id: Uuid.generateUUID(), role: r, utstyr: setting.equipmentTypes, erAktivt: i == 0 } as BeskyttelsesutstyrKort
    });
  }
}
