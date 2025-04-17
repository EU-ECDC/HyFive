import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Role } from '../../models/api/Role';

@Injectable({
  providedIn: 'root'
})
export class RolleService {

  constructor(private readonly http: HttpClient) { }

  opprettRolle(rolle: Role): Observable<Role> {
    const url = `${environment.apiBaseUrl}/v1/rolle`;
    return this.http.post<Role>(url, rolle);
  }

  oppdaterRolle(rolle: Role): Observable<Role> {
    const url = `${environment.apiBaseUrl}/v1/rolle`;
    return this.http.put<Role>(url, rolle);
  }

  hentRoller(): Observable<Role[]> {
    const url = `${environment.apiBaseUrl}/v1/rolle`;
    return this.http.get<Role[]>(url);
  }
}
