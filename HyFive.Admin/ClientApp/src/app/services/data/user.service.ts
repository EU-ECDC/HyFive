import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { User } from '../../models/api/User';
import { OpprettFhiAdminRequest } from '../../models/api/OpprettFhiAdminRequest';
import { KoordinatorForHelseforetak } from '../../models/api/KoordinatorForHelseforetak';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private readonly http: HttpClient) { }

  // ---- Observator ----

  createObserver(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/observator/opprett`;
    return this.http.post<User>(url, user);
  }

  oppdaterObservator(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/observator/oppdater`;
    return this.http.put<User>(url, user);
  }

  slettObservator(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/observator/slett?observatorId=${id}`;
    return this.http.delete<boolean>(url);
  }

  harOverfortSesjonTilFHI(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/observator/harOverfortSesjonTilFHI?observatorId=${id}`;
    return this.http.get<boolean>(url);
  }

  // ---- Koordinator ----

  opprettKoordinator(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/koordinator/opprett`;
    return this.http.post<User>(url, user);
  }

  oppdaterKoordinator(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/koordinator/oppdater`;
    return this.http.put<User>(url, user);
  }

  slettKoordinator(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/koordinator/slett?koordinatorId=${id}`;
    return this.http.delete<boolean>(url);
  }

  // ---- FhiAdmin ----

  hentFhiAdmin(): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/user/fhiadmin`;
    return this.http.get<User[]>(url);
  }

  opprettFhiAdmin(user: OpprettFhiAdminRequest): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/fhiadmin`;
    return this.http.post<User>(url, user);
  }

  oppdaterFhiAdmin(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/fhiadmin`;
    return this.http.put<User>(url, user);
  }

  harGyldigHprnummerEllerPseudonym(user: User): boolean{
    return user?.hprNummer != null || this.isValidPseudonym(user.identPseudonym);
  }

  harKoordinatorGyldigHprnummerEllerPseudonym(koordinator: KoordinatorForHelseforetak): boolean {
    return koordinator?.hprNummer?.trim().length > 0 || this.isValidPseudonym(koordinator.identPseudonym);
  }

  isValidPseudonym(pseudonym: string): boolean{
    return pseudonym?.length === 44;
  }
}
