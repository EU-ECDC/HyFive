import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import {OppdaterSesjonRequest} from "../../models/api/OppdaterSesjonRequest";

@Injectable({
  providedIn: 'root'
})
export class SesjonService {

  constructor(private readonly http: HttpClient) { }

  slettSesjon(sesjonId: string, institusjonId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/sesjon/slett/${sesjonId}?institusjonId=${institusjonId}`;
    return this.http.delete<boolean>(url);
  }

  oppdaterSesjon(sesjon: Partial<OppdaterSesjonRequest>): Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/sesjon/oppdater`;
    return this.http.put<Partial<OppdaterSesjonRequest>>(url, sesjon);
  }
}
