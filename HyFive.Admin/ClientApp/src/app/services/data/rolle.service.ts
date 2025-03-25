import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Rolle } from '../../models/api/Rolle';

@Injectable({
  providedIn: 'root'
})
export class RolleService {

  constructor(private readonly http: HttpClient) { }

  opprettRolle(rolle: Rolle): Observable<Rolle> {
    const url = `${environment.apiBaseUrl}/v1/rolle`;
    return this.http.post<Rolle>(url, rolle);
  }

  oppdaterRolle(rolle: Rolle): Observable<Rolle> {
    const url = `${environment.apiBaseUrl}/v1/rolle`;
    return this.http.put<Rolle>(url, rolle);
  }

  hentRoller(): Observable<Rolle[]> {
    const url = `${environment.apiBaseUrl}/v1/rolle`;
    return this.http.get<Rolle[]>(url);
  }
}
