import { Injectable } from '@angular/core';
import { ForesporselOmBrukertilgang } from '../../models/api/ForesporselOmBrukertilgang';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Institusjon } from 'src/app/models/api/Institusjon';
import { OpprettForesporselOmBrukertilgangRequest } from '../../models/api/OpprettForesporselOmBrukertilgangRequest';
@Injectable({
  'providedIn': 'root'
})
export class ForesporselOmBrukertilgangService {

  constructor(private readonly httpClient: HttpClient){  }

  hentInstitusjoner(): Observable<Institusjon[]> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/institusjoner`;
    return this.httpClient.get<Institusjon[]>(url);
  }

  sendForesporselOmBrukertilgang(nyForsporselOmBrukertilgang: OpprettForesporselOmBrukertilgangRequest): Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/send`;
    return this.httpClient.post<boolean>(url, nyForsporselOmBrukertilgang);
  }

  hentForesporselSomSendtAllerede(): Observable<ForesporselOmBrukertilgang> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang`;
    return this.httpClient.get<ForesporselOmBrukertilgang>(url);
  }

  hentInstitusjon(id: number): Observable<Institusjon> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/institusjon`;
    let params = new HttpParams();
    params = params.append("institusjonId", id.toString());
    return this.httpClient.get<Institusjon>(url, {params});
  }
}
