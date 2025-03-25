import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { AktivitetType } from '../../models/api/AktivitetType';

@Injectable({
  providedIn: 'root'
})
export class AktivitettypeService {

  constructor(private readonly http: HttpClient) { }

  hentAktivitettyper(): Observable<AktivitetType[]> {
    const url = `${environment.apiBaseUrl}/v1/aktivitettype/`;
    return this.http.get<AktivitetType[]>(url);
  }

  oppdaterAktivitettype(aktivitettype: AktivitetType): Observable<AktivitetType> {
    const url = `${environment.apiBaseUrl}/v1/aktivitettype/oppdater`;
    return this.http.put<AktivitetType>(url, aktivitettype);
  }
}
