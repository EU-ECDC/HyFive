import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IndicationType } from '../../models/api/IndicationType';
import { environment } from 'src/environments/environment';
import { FacilityType } from '../../models/api/FacilityType';
import { CreateFacilityTypeRequest } from '../../models/api/CreateFacilityTypeRequest';

@Injectable({
  providedIn: 'root'
})

export class FacilitiesTypesService {

  constructor(private httpClient: HttpClient) {

  }

  getFacilityTypes(): Observable<FacilityType[]> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes`;
    return this.httpClient.get<IndicationType[]>(url)
    .pipe();
  }

  updateFacilityType(facilitytype: FacilityType): Observable<FacilityType> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes/update`;
    return this.httpClient.put<FacilityType>(url, facilitytype)
    .pipe();
  }

  createFacilityType(facilitytype: CreateFacilityTypeRequest): Observable<FacilityType> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes/create`;
    return this.httpClient.post<FacilityType>(url, facilitytype)
    .pipe();
  }

  deleteFacilityType(facilitytypeId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes/delete?facilitytypeId=${facilitytypeId}`;
    return this.httpClient.delete<boolean>(url)
    .pipe();
  }
}
