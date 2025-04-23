import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Institution } from '../../models/api/Institution';
import { OpprettInstitusjonRequest } from '../../models/api/OpprettInstitusjonRequest';
import { InstitusjonType } from '../../models/api/InstitusjonType';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { User } from '../../models/api/User';
import { Department} from "../../models/api/Department";
import { Localstoragepaths } from '../../_felles/konstanter/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class InstitutionService {

  constructor(private readonly http: HttpClient) { }

  getInstitutions(): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/`;
    return this.http.get<Institution[]>(url);
  }

  hentInstitusjonerForKoordinator(): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/hentInstitusjonerForKoordinator/`;
    return this.http.get<InstitutionReport[]>(url);
  }

  hentInstitusjon(id: number): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}`;
    return this.http.get<Institution>(url);
  }

  hentValgtInstitusjonId(): number | null {
    const valgtInstitusjonIdString = localStorage.getItem(Localstoragepaths.SelectedInstitution);
    return valgtInstitusjonIdString ? parseInt(valgtInstitusjonIdString) : null;
  }

  oppdaterValgtInstitusjonId(institutionId: number): number | null {
    localStorage.setItem(Localstoragepaths.SelectedInstitution, JSON.stringify(institutionId));
    return this.hentValgtInstitusjonId();
  }

  getObservers(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}/observers`;
    return this.http.get<User[]>(url);
  }

  hentKoordinatorer(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}/koordinatorer`;
    return this.http.get<User[]>(url);
  }

  hentAvdelinger(id: number): Observable<Department[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}/avdelinger/`;
    return this.http.get<Department[]>(url);
  }

  hentInstitusjontyper(): Observable<InstitusjonType[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/typer`;
    return this.http.get<InstitusjonType[]>(url);
  }

  opprettInstitusjon(request: OpprettInstitusjonRequest): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/institution/opprett`;
    return this.http.post<Institution>(url, request);
  }

  oppdaterInstitusjon(institution: Institution): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/institution/oppdater`;
    return this.http.put<Institution>(url, institution);
  }

  slettInstitusjon(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/institution/slett?institutionId=${id}`;
    return this.http.delete<boolean>(url);
  }
}
