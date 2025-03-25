import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Bruker } from '../../models/api/Bruker';
import { OpprettFhiAdminRequest } from '../../models/api/OpprettFhiAdminRequest';
import { KoordinatorForHelseforetak } from '../../models/api/KoordinatorForHelseforetak';

@Injectable({
  providedIn: 'root'
})
export class BrukerService {

  constructor(private readonly http: HttpClient) { }

  // ---- Observator ----

  opprettObservator(bruker: Bruker): Observable<Bruker> {
    const url = `${environment.apiBaseUrl}/v1/bruker/observator/opprett`;
    return this.http.post<Bruker>(url, bruker);
  }

  oppdaterObservator(bruker: Bruker): Observable<Bruker> {
    const url = `${environment.apiBaseUrl}/v1/bruker/observator/oppdater`;
    return this.http.put<Bruker>(url, bruker);
  }

  slettObservator(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/bruker/observator/slett?observatorId=${id}`;
    return this.http.delete<boolean>(url);
  }

  harOverfortSesjonTilFHI(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/bruker/observator/harOverfortSesjonTilFHI?observatorId=${id}`;
    return this.http.get<boolean>(url);
  }

  // ---- Koordinator ----

  opprettKoordinator(bruker: Bruker): Observable<Bruker> {
    const url = `${environment.apiBaseUrl}/v1/bruker/koordinator/opprett`;
    return this.http.post<Bruker>(url, bruker);
  }

  oppdaterKoordinator(bruker: Bruker): Observable<Bruker> {
    const url = `${environment.apiBaseUrl}/v1/bruker/koordinator/oppdater`;
    return this.http.put<Bruker>(url, bruker);
  }

  slettKoordinator(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/bruker/koordinator/slett?koordinatorId=${id}`;
    return this.http.delete<boolean>(url);
  }

  // ---- FhiAdmin ----

  hentFhiAdmin(): Observable<Bruker[]> {
    const url = `${environment.apiBaseUrl}/v1/bruker/fhiadmin`;
    return this.http.get<Bruker[]>(url);
  }

  opprettFhiAdmin(bruker: OpprettFhiAdminRequest): Observable<Bruker> {
    const url = `${environment.apiBaseUrl}/v1/bruker/fhiadmin`;
    return this.http.post<Bruker>(url, bruker);
  }

  oppdaterFhiAdmin(bruker: Bruker): Observable<Bruker> {
    const url = `${environment.apiBaseUrl}/v1/bruker/fhiadmin`;
    return this.http.put<Bruker>(url, bruker);
  }

  harGyldigHprnummerEllerPseudonym(bruker: Bruker): boolean{
    return bruker?.hprNummer != null || this.erGyldigPseudonym(bruker.identPseudonym);
  }

  harKoordinatorGyldigHprnummerEllerPseudonym(koordinator: KoordinatorForHelseforetak): boolean {
    return koordinator?.hprNummer?.trim().length > 0 || this.erGyldigPseudonym(koordinator.identPseudonym);
  }

  erGyldigPseudonym(pseudonym: string): boolean{
    return pseudonym?.length === 44;
  }
}
