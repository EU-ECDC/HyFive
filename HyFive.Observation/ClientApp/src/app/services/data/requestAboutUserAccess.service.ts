import { Injectable } from '@angular/core';
import { UserAccessRequest } from '../../models/api/UserAccessRequest';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Facility } from 'src/app/models/api/Facility';
import { CreateUserAccessRequest } from '../../models/api/CreateUserAccessRequest';
@Injectable({
  'providedIn': 'root'
})
export class RequestAboutUserAccessService {

  constructor(private readonly httpClient: HttpClient){  }

  getFacilities(): Observable<Facility[]> {
    const url = `${environment.apiBaseUrl}/v1/userAccessRequest/facilities`;
    return this.httpClient.get<Facility[]>(url);
  }

  sendRequestAboutUserAccess(newRequestAboutUserAccess: CreateUserAccessRequest): Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/userAccessRequest/send`;
    return this.httpClient.post<boolean>(url, newRequestAboutUserAccess);
  }

  fetchRequestSentAlready(): Observable<UserAccessRequest> {
    const url = `${environment.apiBaseUrl}/v1/userAccessRequest`;
    return this.httpClient.get<UserAccessRequest>(url);
  }

  getFacility(id: number): Observable<Facility> {
    const url = `${environment.apiBaseUrl}/v1/requestaboutuseraccess/facility`;
    let params = new HttpParams();
    params = params.append("facilityId", id.toString());
    return this.httpClient.get<Facility>(url, {params});
  }
}
