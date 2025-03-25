import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Institusjon } from '../../models/api/Institusjon';
import { OpprettInstitusjonRequest } from '../../models/api/OpprettInstitusjonRequest';
import { InstitusjonType } from '../../models/api/InstitusjonType';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';
import { Bruker } from '../../models/api/Bruker';
import { Avdeling } from "../../models/api/Avdeling";
import { Localstoragepaths } from '../../_felles/konstanter/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class InstitusjonService {

  constructor(private readonly http: HttpClient) { }

  hentInstitusjoner(): Observable<InstitusjonRapport[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/`;
    return this.http.get<Institusjon[]>(url);
  }

  hentInstitusjonerForKoordinator(): Observable<InstitusjonRapport[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/hentInstitusjonerForKoordinator/`;
    return this.http.get<InstitusjonRapport[]>(url);
  }

  hentInstitusjon(id: number): Observable<Institusjon> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/${id}`;
    return this.http.get<Institusjon>(url);
  }

  hentValgtInstitusjonId(): number | null {
    const valgtInstitusjonIdString = localStorage.getItem(Localstoragepaths.ValgtInstitusjon);
    return valgtInstitusjonIdString ? parseInt(valgtInstitusjonIdString) : null;
  }

  oppdaterValgtInstitusjonId(institusjonId: number): number | null {
    localStorage.setItem(Localstoragepaths.ValgtInstitusjon, JSON.stringify(institusjonId));
    return this.hentValgtInstitusjonId();
  }

  hentObservatorer(id: number): Observable<Bruker[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/${id}/observatorer`;
    return this.http.get<Bruker[]>(url);
  }

  hentKoordinatorer(id: number): Observable<Bruker[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/${id}/koordinatorer`;
    return this.http.get<Bruker[]>(url);
  }

  hentAvdelinger(id: number): Observable<Avdeling[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/${id}/avdelinger/`;
    return this.http.get<Avdeling[]>(url);
  }

  hentInstitusjontyper(): Observable<InstitusjonType[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/typer`;
    return this.http.get<InstitusjonType[]>(url);
  }

  opprettInstitusjon(request: OpprettInstitusjonRequest): Observable<Institusjon> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/opprett`;
    return this.http.post<Institusjon>(url, request);
  }

  oppdaterInstitusjon(institusjon: Institusjon): Observable<Institusjon> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/oppdater`;
    return this.http.put<Institusjon>(url, institusjon);
  }

  slettInstitusjon(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/slett?institusjonId=${id}`;
    return this.http.delete<boolean>(url);
  }
}
