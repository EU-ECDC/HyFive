import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Klinikk } from '../../models/api/Klinikk';


@Injectable({
  providedIn: 'root'
})
export class KlinikkService {

  constructor(private readonly http: HttpClient) { }

  hentKlinikk(id: number, institusjonId: number): Observable<Klinikk> {
    const url = `${environment.apiBaseUrl}/v1/klinikk/${id}?institusjonId=${institusjonId}`;
    return this.http.get<Klinikk>(url);
  }

  hentKlinikkerForInstitusjon(institusjonId: number): Observable<Klinikk[]> {
    const url = `${environment.apiBaseUrl}/v1/klinikk/institusjon/${institusjonId}`;
    return this.http.get<Klinikk[]>(url);
  }

  opprettKlinikk(klinikk: Klinikk): Observable<Klinikk> {
    const url = `${environment.apiBaseUrl}/v1/klinikk/opprett`;
    return this.http.post<Klinikk>(url, klinikk);
  }

  oppdaterKlinikk(klinikk: Klinikk): Observable<Klinikk> {
    const url = `${environment.apiBaseUrl}/v1/klinikk/oppdater`;
    return this.http.put<Klinikk>(url, klinikk);
  }
}
