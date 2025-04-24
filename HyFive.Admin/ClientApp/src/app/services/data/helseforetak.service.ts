import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OpprettHelseforetakRequest } from 'src/app/models/api/OpprettHelseforetakRequest';
import { environment } from 'src/environments/environment';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { CoordinatorForHealthcareCompanies } from '../../models/api/CoordinatorForHealthcareCompanies';
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

  getCoordinators(id: number): Observable<CoordinatorForHealthcareCompanies[]> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/koordinatorer`;
    return this.httpClient.get<CoordinatorForHealthcareCompanies[]>(url);
  }

  getInstitutions(id: number): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/institusjoner`;
    return this.httpClient.get<InstitutionReport[]>(url);
  }

  updateCoordinator(id: number, koordinator: CoordinatorForHealthcareCompanies): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/oppdaterkoordinator`;
    return this.httpClient.put<Status>(url, koordinator);
  }

  createCoordinator(id: number, koordinator: CoordinatorForHealthcareCompanies): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/helseforetak/${id}/opprettkoordinator`;
    return this.httpClient.post<Status>(url, koordinator);
  }
}
