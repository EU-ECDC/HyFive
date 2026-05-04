import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IndicationType } from '../../models/api/IndicationType';
import { environment } from 'src/environments/environment';
import { CreateFacilityTypeRequest } from '../../models/api/CreateFacilityTypeRequest';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';

@Injectable({
  providedIn: 'root'
})

export class FacilitiesTypesService {

  constructor(private readonly httpClient: HttpClient) {

  }

  getFacilityTypes(): Observable<OrganisationUnitType[]> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes`;
    return this.httpClient.get<OrganisationUnitType[]>(url)
    .pipe();
  }

  updateFacilityType(facilitytype: OrganisationUnitType): Observable<OrganisationUnitType> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes/update`;
    return this.httpClient.put<OrganisationUnitType>(url, facilitytype)
    .pipe();
  }

  createFacilityType(facilitytype: CreateFacilityTypeRequest): Observable<OrganisationUnitType> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes/create`;
    return this.httpClient.post<OrganisationUnitType>(url, facilitytype)
    .pipe();
  }

  deleteFacilityType(facilitytypeId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/facilitytypes/delete?facilitytypeId=${facilitytypeId}`;
    return this.httpClient.delete<boolean>(url)
    .pipe();
  }
}
