import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { UpdateUnitRequest } from 'src/app/models/api/UpdateUnitRequest';
import { CreateUnitRequest } from 'src/app/models/api/CreateUnitRequest';
import { UnitResponse } from 'src/app/models/api/UnitResponse';
import { OrganisationUnitResponse } from 'src/app/models/api/OrganisationUnitResponse';


@Injectable({
  providedIn: 'root'
})
export class UnitService {

  constructor(private readonly http: HttpClient) { }

  getUnit(unitId: number, facilityId: number): Observable<OrganisationUnitResponse> {
    const url = `${environment.apiBaseUrl}/v1/unit/${unitId}?facilityId=${facilityId}`;
    return this.http.get<OrganisationUnitResponse>(url);
  }

  getUnitsForFacility(facilityId: number): Observable<UnitResponse[]> {
    const url = `${environment.apiBaseUrl}/v1/unit/facility/${facilityId}`;
    return this.http.get<UnitResponse[]>(url);
  }

  createUnit(unit: CreateUnitRequest): Observable<OrganisationUnit> {
    const url = `${environment.apiBaseUrl}/v1/unit/create`;
    return this.http.post<OrganisationUnit>(url, unit);
  }

  updateUnit(unit: UpdateUnitRequest): Observable<UnitResponse> {
    const url = `${environment.apiBaseUrl}/v1/unit/update`;
    return this.http.put<UnitResponse>(url, unit);
  }
}
