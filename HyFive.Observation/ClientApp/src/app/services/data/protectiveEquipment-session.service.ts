import {EventEmitter, Injectable} from "@angular/core";
import { BaseSessionService } from './base-session.service';
import { ProtectiveEquipmentSessionView } from '../../models/registration/protectiveEquipment-sessionView.model';
import { ProtectiveEquipmentSession } from "src/app/models/api/ProtectiveEquipmentSession";
import { ProtectiveEquipmentObservation } from "src/app/models/api/ProtectiveEquipmentObservation";
import { Role } from "src/app/models/api/Role";
import { Uuid } from "src/app/utils/uuid";
import { ProtectiveEquipmentSettingType } from '../../models/api/ProtectiveEquipmentSettingType';
import { ProtectiveEquipmentCard } from "src/app/models/registration/protectiveEquipment-card.model";
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { FacilityService } from "./FacilityService";
import {ProtectiveEquipmentType} from "../../models/api/ProtectiveEquipmentType";
import {ProtectiveEquipment} from "../../models/api/ProtectiveEquipment";
import { OrganisationUnit } from "src/app/models/api/OrganisationUnit";

@Injectable({
  providedIn: 'root'
})
export class ProtectiveEquipmentSessionService extends BaseSessionService<ProtectiveEquipmentSessionView, ProtectiveEquipmentSession, ProtectiveEquipmentObservation>{
  sessionShowLocalStoragePath: string = Localstoragepaths.ProtectiveEquipmentSessionViews;
  sessionLocalStoragePath: string = Localstoragepaths.ProtectiveEquipmentSessions;



  constructor(
    public facilityService: FacilityService,
    private readonly httpClient: HttpClient) {
    super(facilityService);
  }

  public sendToServer(sessionId: string): Observable<string> {
    let sessions = this.getSessions();
    let sessionIndex = sessions.map(s => s.id).indexOf(sessionId);
    let sessionToSend = sessions[sessionIndex];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/protectiveEquipment`, sessionToSend)
  }

  protectiveEquipmentUpdated: EventEmitter<ProtectiveEquipmentType[]> = new EventEmitter<ProtectiveEquipmentType[]>()

  createSessionView(
    rolesAsObserved: Role[],
    department: OrganisationUnit,
    setting: ProtectiveEquipmentSettingType): string {
    let id = Uuid.generateUUID();

    let sessionView: ProtectiveEquipmentSessionView = {
      sessionId: id,
      department: department,
      card: this.generateCard(rolesAsObserved, setting),
      setting: setting
    };

    let sessionViews = this.getSessionViews();
    sessionViews.push(sessionView);
    this.saveSessionViews(sessionViews);

    return id;
  }

  updateSessionEquipmentTypes(sessionId: string, equipmentTypes: ProtectiveEquipmentType[]){
    let sessionViews = this.getSessionViews();
    let sessionviewToBeUpdated = sessionViews.find(s => s.sessionId == sessionId);
    sessionviewToBeUpdated.setting.equipmentTypes = equipmentTypes;
    this.saveSessionViews(sessionViews);
    this.protectiveEquipmentUpdated.emit(equipmentTypes);
  }

  numberOfQualifiedEquipment(protectiveEquipmentList: ProtectiveEquipment[]) : number{
    return protectiveEquipmentList.reduce((qualifiedEquipment, curr) => {
        if (curr.isRequired || curr.wasUsed){
          return qualifiedEquipment + 1;
        }
        else {
          return qualifiedEquipment;
        }
      },
      0);
  }

  private generateCard(roles: Role[], setting: ProtectiveEquipmentSettingType): ProtectiveEquipmentCard[] {
    return roles.map((r, i) => {
      return { id: Uuid.generateUUID(), role: r, equipment: setting.equipmentTypes, isActive: i == 0 } as ProtectiveEquipmentCard
    });
  }
}
