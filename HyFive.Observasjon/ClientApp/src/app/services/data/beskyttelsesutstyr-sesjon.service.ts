import {EventEmitter, Injectable} from "@angular/core";
import { BaseSessionService } from './base-session.service';
import { ProtectiveEquipmentSessionView } from '../../models/registration/protectiveEquipment-sessionView.model';
import { ProtectiveEquipmentSession } from "src/app/models/api/ProtectiveEquipmentSession";
import { ProtectiveEquipmentObservation } from "src/app/models/api/ProtectiveEquipmentObservation";
import { Role } from "src/app/models/api/Role";
import { Department } from "src/app/models/api/Department";
import { Uuid } from "src/app/utils/uuid";
import { ProtectiveEquipmentSettingType } from '../../models/api/ProtectiveEquipmentSettingType';
import { Card } from '../../models/registration/card.model';
import { ProtectiveEquipmentCard } from "src/app/models/registration/protectiveEquipment-card.model";
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { InstitusjonService } from "./institusjon.service";
import {ProtectiveEquipmentType} from "../../models/api/ProtectiveEquipmentType";
import {ProtectiveEquipment} from "../../models/api/ProtectiveEquipment";

@Injectable({
  providedIn: 'root'
})
export class BeskyttelsesutstyrSesjonService extends BaseSessionService<ProtectiveEquipmentSessionView, ProtectiveEquipmentSession, ProtectiveEquipmentObservation>{
  sessionShowLocalStoragePath: string = Localstoragepaths.ProtectiveEquipmentSessionViews;
  sessionLocalStoragePath: string = Localstoragepaths.ProtectiveEquipmentSessions;



  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sessionId: string): Observable<string> {
    var sessions = this.hentSesjoner();
    var sesjonIndeks = sessions.map(s => s.id).indexOf(sessionId);
    var sesjonSomSkalSendes = sessions[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/protectiveEquipment`, sesjonSomSkalSendes)
  }

  beskyttelsesutstyrOppdatert: EventEmitter<ProtectiveEquipmentType[]> = new EventEmitter<ProtectiveEquipmentType[]>()

  lagSesjonsvisning(
    rollerSomObserveres: Role[],
    department: Department,
    setting: ProtectiveEquipmentSettingType): string {
    let id = Uuid.generateUUID();

    let sessionView: ProtectiveEquipmentSessionView = {
      sessionId: id,
      department: department,
      card: this.genererKort(rollerSomObserveres, setting),
      setting: setting
    };

    var sessionViews = this.hentSesjonsvisninger();
    sessionViews.push(sessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }

  oppdaterSesjonUtstyrstyper(sessionId: string, equipmentTypes: ProtectiveEquipmentType[]){
    var sessionViews = this.hentSesjonsvisninger();
    var sesjonsvisningSomSkalOppdateres = sessionViews.find(s => s.sessionId == sessionId);
    sesjonsvisningSomSkalOppdateres.setting.equipmentTypes = equipmentTypes;
    this.saveSessionViews(sessionViews);
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

  private genererKort(roles: Role[], setting: ProtectiveEquipmentSettingType): ProtectiveEquipmentCard[] {
    return roles.map((r, i) => {
      return { id: Uuid.generateUUID(), role: r, utstyr: setting.equipmentTypes, isActive: i == 0 } as ProtectiveEquipmentCard
    });
  }
}
