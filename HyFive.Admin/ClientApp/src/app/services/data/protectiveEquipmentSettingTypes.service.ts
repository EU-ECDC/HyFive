import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProtectiveEquipmentSettingType } from "src/app/models/api/ProtectiveEquipmentSettingType";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class ProtectiveEquipmentSettingTypesService {
  constructor(private httpClient: HttpClient) { }

  getProtectiveEquipmentSettingTypes(): Observable<ProtectiveEquipmentSettingType[]> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentSettingTypes`;
    return this.httpClient.get<ProtectiveEquipmentSettingType[]>(url);
  }

  updateProtectiveEquipmentSettingType(settingType: ProtectiveEquipmentSettingType): Observable<ProtectiveEquipmentSettingType> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentSettingTypes/update`;
    return this.httpClient.put<ProtectiveEquipmentSettingType>(url, settingType);
  }
}
