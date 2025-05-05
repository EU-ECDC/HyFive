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
export class ForesporselOmBrukertilgangService {

  constructor(private readonly httpClient: HttpClient){  }

  hentInstitusjoner(): Observable<Institution[]> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/institusjoner`;
    return this.httpClient.get<Institution[]>(url);
  }

  sendForesporselOmBrukertilgang(nyForsporselOmBrukertilgang: CreateUserAccessRequest): Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/send`;
    return this.httpClient.post<boolean>(url, nyForsporselOmBrukertilgang);
  }

  hentForesporselSomSendtAllerede(): Observable<UserAccessRequest> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang`;
    return this.httpClient.get<UserAccessRequest>(url);
  }

  hentInstitusjon(id: number): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/institusjon`;
    let params = new HttpParams();
    params = params.append("institutionId", id.toString());
    return this.httpClient.get<Institution>(url, {params});
  }
}
