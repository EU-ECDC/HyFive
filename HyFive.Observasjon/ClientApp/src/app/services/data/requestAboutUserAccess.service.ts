import { Injectable } from '@angular/core';
import { UserAccessRequest } from '../../models/api/UserAccessRequest';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Institution } from 'src/app/models/api/Institution';
import { CreateUserAccessRequest } from '../../models/api/CreateUserAccessRequest';
@Injectable({
  'providedIn': 'root'
})
export class RequestAboutUserAccessService {

  constructor(private readonly httpClient: HttpClient){  }

  getInstitutions(): Observable<Institution[]> {
    const url = `${environment.apiBaseUrl}/v1/requestaboutuseraccess/institutions`;
    return this.httpClient.get<Institution[]>(url);
  }

  sendRequestAboutUserAccess(newRequestAboutUserAccess: CreateUserAccessRequest): Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/requestaboutuseraccess/send`;
    return this.httpClient.post<boolean>(url, newRequestAboutUserAccess);
  }

  fetchRequestSentAlready(): Observable<UserAccessRequest> {
    const url = `${environment.apiBaseUrl}/v1/requestaboutuseraccess`;
    return this.httpClient.get<UserAccessRequest>(url);
  }

  getInstitution(id: number): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/requestaboutuseraccess/institution`;
    let params = new HttpParams();
    params = params.append("institutionId", id.toString());
    return this.httpClient.get<Institution>(url, {params});
  }
}
