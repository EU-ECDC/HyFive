import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { map, Observable } from 'rxjs';
import { CreateFacilityRequest } from '../../models/api/CreateFacilityRequest';
import { FacilityReport } from '../../models/api/FacilityReport';
import { User } from '../../models/api/User';
import { Localstoragepaths } from '../../_common/constants/localstoragepaths';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';

@Injectable({
  providedIn: 'root'
})
export class FacilityService {

  constructor(private readonly http: HttpClient) { }

  getFacilities(): Observable<OrganisationUnit[]> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/facilities`;
    return this.http.get<OrganisationUnit[]>(url).pipe(
                                                map(data => data.filter(x => x != null)),
                                                map(data =>  [...new Map(data.map(item => [item.id, item])).values()])
                                              );
  }


  getFacilitiesPaginated(offset, limit): Observable<OrganisationUnit[]> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/getfacilitiesPaginated`;

    let params = new HttpParams();
    if (offset !== null) {
      params = params.append("offset", offset);
    }
    if (limit !== null) {
      params = params.append("limit", limit);
    }

    return this.http.get<OrganisationUnit[]>(url, { params: params });
  }

   getComplianceFacilities(facilityIds: number[]): Observable<OrganisationUnit[]> {
    const params = new HttpParams({
    fromObject: {
      facilityIds: facilityIds.map(id => id.toString()) // repeat the key
    }
  });
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/getCompliancefacilities`;
    return this.http.get<OrganisationUnit[]>(url, { params });
  }

  getFacilitiesForCoordinator(): Observable<FacilityReport[]> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/getfacilitiesforcoordinator/`;
    return this.http.get<FacilityReport[]>(url);
  }

  getFacility(id: number): Observable<OrganisationUnit> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/${id}`;
    return this.http.get<OrganisationUnit>(url);
  }

  getSelectedFacilityId(): number | null {
    const SelectedFacilityIdString = localStorage.getItem(Localstoragepaths.SelectedFacility);
    return SelectedFacilityIdString ? Number.parseInt(SelectedFacilityIdString) : null;
  }

  updateSelectedFacilityId(facilityId: number): number | null {
    localStorage.setItem(Localstoragepaths.SelectedFacility, JSON.stringify(facilityId));
    return this.getSelectedFacilityId();
  }

  getObservers(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/${id}/observers`;
    return this.http.get<User[]>(url);
  }

  getCoordinators(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/${id}/coordinators`;
    return this.http.get<User[]>(url);
  }

  getDepartments(id: number): Observable<OrganisationUnit[]> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/${id}/departments/`;
    return this.http.get<OrganisationUnit[]>(url);
  }

  getDepartmentsByFacilities(ids: number[]): Observable<OrganisationUnit[]> {
    const params = new HttpParams({ fromObject: { ids: ids.map(String) } });
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/departments`;
    return this.http.get<OrganisationUnit[]>(url, {params});
  }

  getFacilityTypes(): Observable<OrganisationUnitType[]> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/types`;
    return this.http.get<OrganisationUnitType[]>(url);
  }

  createFacility(request: CreateFacilityRequest): Observable<OrganisationUnit> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/create`;
    return this.http.post<OrganisationUnit>(url, request);
  }

  updateFacility(facility: OrganisationUnit): Observable<OrganisationUnit> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/update`;
    return this.http.put<OrganisationUnit>(url, facility);
  }

  deleteFacility(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/organisationUnits/delete?facilityId=${id}`;
    return this.http.delete<boolean>(url);
  }
}
