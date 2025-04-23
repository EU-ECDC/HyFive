import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProtectiveEquipmentType } from "src/app/models/api/ProtectiveEquipmentType";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class ProtectiveEquipmentSettingTypesService {
  constructor(private httpClient: HttpClient) { }

  getProtectiveEquipmentTypes(): Observable<ProtectiveEquipmentType[]> {
    const url = `${environment.apiBaseUrl}/v1/protectiveequipmentsettingtypes`;
    return this.httpClient.get<ProtectiveEquipmentType[]>(url);
  }

  updateProtectiveEquipmentSettingType(settingType: ProtectiveEquipmentType): Observable<ProtectiveEquipmentType> {
    const url = `${environment.apiBaseUrl}/v1/protectiveequipmentsettingtypes/update`;
    return this.httpClient.put<ProtectiveEquipmentType>(url, settingType);
  }
}
