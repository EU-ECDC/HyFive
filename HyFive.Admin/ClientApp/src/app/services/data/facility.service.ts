import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { map, Observable, of } from 'rxjs';
import { Facility } from '../../models/api/Facility';
import { CreateFacilityRequest } from '../../models/api/CreateFacilityRequest';
import { FacilityType } from '../../models/api/FacilityType';
import { FacilityReport } from '../../models/api/FacilityReport';
import { User } from '../../models/api/User';
import { Department} from "../../models/api/Department";
import { Localstoragepaths } from '../../_common/constants/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class FacilityService {

  constructor(private readonly http: HttpClient) { }

  getFacilities(): Observable<FacilityReport[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/`;
    return this.http.get<Facility[]>(url).pipe(
                                                map(data => data.filter(x => x != null)),
                                                map(data =>  [...new Map(data.map(item => [item.id, item])).values()])
                                              );
  }


  getFacilitiesPaginated(offset, limit): Observable<FacilityReport[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/getfacilitiesPaginated`;

    let params = new HttpParams();
    if (offset !== null) {
      params = params.append("offset", offset);
    }
    if (limit !== null) {
      params = params.append("limit", limit);
    }

    return this.http.get<Facility[]>(url, { params: params });
  }

   getComplianceFacilities(facilityIds: number[]): Observable<Facility[]> {
    const params = new HttpParams({
    fromObject: {
      facilityIds: facilityIds.map(id => id.toString()) // repeat the key
    }
  });
    const url = `${environment.apiBaseUrl}/v1/facility/getCompliancefacilities`;
    return this.http.get<Facility[]>(url, { params });
  }

  getFacilitiesForCoordinator(): Observable<FacilityReport[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/getfacilitiesforcoordinator/`;
    return this.http.get<FacilityReport[]>(url);
  }

  getFacility(id: number): Observable<Facility> {
    const url = `${environment.apiBaseUrl}/v1/facility/${id}`;
    return this.http.get<Facility>(url);
  }

  getSelectedFacilityId(): number | null {
    const SelectedFacilityIdString = localStorage.getItem(Localstoragepaths.SelectedFacility);
    return SelectedFacilityIdString ? parseInt(SelectedFacilityIdString) : null;
  }

  updateSelectedFacilityId(facilityId: number): number | null {
    localStorage.setItem(Localstoragepaths.SelectedFacility, JSON.stringify(facilityId));
    return this.getSelectedFacilityId();
  }

  getObservers(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/${id}/observers`;
    return this.http.get<User[]>(url);
  }

  getCoordinators(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/${id}/coordinators`;
    return this.http.get<User[]>(url);
  }

  getDepartments(id: number): Observable<Department[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/${id}/departments/`;
    return this.http.get<Department[]>(url);
  }

  getDepartmentsByFacilities(ids: number[]): Observable<Department[]> {
    const params = new HttpParams({ fromObject: { ids: ids.map(String) } });
    const url = `${environment.apiBaseUrl}/v1/facility/departments`;
    return this.http.get<Department[]>(url, {params});
  }

  getFacilityTypes(): Observable<FacilityType[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/types`;
    return this.http.get<FacilityType[]>(url);
  }

  createFacility(request: CreateFacilityRequest): Observable<Facility> {
    const url = `${environment.apiBaseUrl}/v1/facility/create`;
    return this.http.post<Facility>(url, request);
  }

  updateFacility(facility: Facility): Observable<Facility> {
    const url = `${environment.apiBaseUrl}/v1/facility/update`;
    return this.http.put<Facility>(url, facility);
  }

  deleteFacility(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/facility/delete?facilityId=${id}`;
    return this.http.delete<boolean>(url);
  }
}
