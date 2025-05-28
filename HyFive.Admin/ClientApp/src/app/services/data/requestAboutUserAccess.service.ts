import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { RequestAboutUserAccess } from "../../models/api/RequestAboutUserAccess";
import { RequestStatus } from 'src/app/models/api/RequestStatus';

@Injectable({
  providedIn: 'root'
})
export class RequestAboutUserAccessService {
  
  constructor(private readonly http: HttpClient) { }

  getAllRequests(institutionId: number): Observable<RequestAboutUserAccess[]> {
    const url = `${environment.apiBaseUrl}/v1/userAccessRequest/allrequests`;

    let params = new HttpParams();
    params = params.append("institutionId", institutionId.toString());

    return this.http.get<RequestAboutUserAccess[]>(url, {params: params});
  }

  getRequestsAwaitingApproval(institutionId: number): Observable<RequestAboutUserAccess[]> {
    const url = `${environment.apiBaseUrl}/v1/userAccessRequest/pendingApprovalRequests`;

    let params = new HttpParams();
    params = params.append("institutionId", institutionId.toString());

    return this.http.get<RequestAboutUserAccess[]>(url, {params: params});
  }

  approveRequest(requestId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/userAccessRequest/approveRequest`;

    let params = new HttpParams();
    params = params.append("requestId", requestId.toString());

    return this.http.get<boolean>(url, { params: params });
  }

  rejectRequest(requestId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/userAccessRequest/rejectRequest`;

    let params = new HttpParams();
    params = params.append("requestId", requestId.toString());

    return this.http.get<boolean>(url, { params: params });
  }
}
