import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProtectiveEquipmentTypeq } from "src/app/models/api/ProtectiveEquipmentTypeq";
import { environment } from "src/environments/environment";
import { MisuseType } from '../../models/api/MisuseType';
import { CreateMisueTypeRequest } from '../../models/api/CreateMisueTypeRequest';

@Injectable({
  providedIn: 'root'
})

export class ProtectiveEquipmentTypeqsService {

  constructor(private readonly httpClient: HttpClient) { }

  getProtectiveEquipmentTypes(): Observable<ProtectiveEquipmentTypeq[]> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentTypes`;
    return this.httpClient.get<ProtectiveEquipmentTypeq[]>(url);
  }

  updateProtectiveEquipmentTypes(equipmentType: ProtectiveEquipmentTypeq): Observable<ProtectiveEquipmentTypeq> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentTypes/update`;
    return this.httpClient.put<ProtectiveEquipmentTypeq>(url, equipmentType);
  }

  getMisuseTypes(equipmentTypeId: number): Observable<MisuseType[]> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentTypes/misuseTypes?equipmentTypeId=${equipmentTypeId}`;
    return this.httpClient.get<MisuseType[]>(url);
  }

  updateMisuseType(equipmentTypeId: number, misuseType: MisuseType): Observable<MisuseType> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentTypes/misuseTypes/update?equipmentTypeId=${equipmentTypeId}`;
    return this.httpClient.put<MisuseType>(url, misuseType);
  }

  deleteMisuseType(equipmentTypeId: number, misuseTypeId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentTypes/misuseTypes/delete?equipmentTypeId=${equipmentTypeId}&misusetypeid=${misuseTypeId}`;
    return this.httpClient.delete<boolean>(url);
  }

  createMisuseType(equipmentTypeId: number, misuse: CreateMisueTypeRequest): Observable<MisuseType> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipmentTypes/misuseTypes/create?equipmentTypeId=${equipmentTypeId}`;
    return this.httpClient.post<MisuseType>(url, misuse);
  }

}
