import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OpprettHelseforetakRequest } from 'src/app/models/api/OpprettHelseforetakRequest';
import { environment } from 'src/environments/environment';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';
import { KoordinatorForHelseforetak } from '../../models/api/KoordinatorForHelseforetak';
import { Status } from 'src/app/models/api/Status';
import { Helseforetak } from '../../models/api/Helseforetak';

@Injectable({
  providedIn: 'root'
})
export class HelseforetakService {

  constructor(private httpClient: HttpClient) { }

  opprettHelseforetak(helseforetak: OpprettHelseforetakRequest) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/opprett`;
    return this.httpClient.post<boolean>(url, helseforetak);
  }

  hentAlleHelseforetak() : Observable<Helseforetak[]> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak`;
    return this.httpClient.get<Helseforetak[]>(url);
  }

  oppdaterHelseforetak(helseforetak: Helseforetak) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/oppdater`;
    return this.httpClient.put<boolean>(url, helseforetak);
  }

  hentKoordinatorer(id: number): Observable<KoordinatorForHelseforetak[]> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/koordinatorer`;
    return this.httpClient.get<KoordinatorForHelseforetak[]>(url);
  }

  hentInstitusjoner(id: number): Observable<InstitusjonRapport[]> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/institusjoner`;
    return this.httpClient.get<InstitusjonRapport[]>(url);
  }

  oppdaterKoordinator(id: number, koordinator: KoordinatorForHelseforetak): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/oppdaterkoordinator`;
    return this.httpClient.put<Status>(url, koordinator);
  }

  opprettKoordinator(id: number, koordinator: KoordinatorForHelseforetak): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/opprettkoordinator`;
    return this.httpClient.post<Status>(url, koordinator);
  }
}
