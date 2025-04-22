import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ActivityType } from '../../models/api/ActivityType';

@Injectable({
  providedIn: 'root'
})
export class AktivitettypeService {

  constructor(private readonly http: HttpClient) { }

  hentAktivitettyper(): Observable<ActivityType[]> {
    const url = `${environment.apiBaseUrl}/v1/aktivitettype/`;
    return this.http.get<ActivityType[]>(url);
  }

  oppdaterAktivitettype(aktivitettype: ActivityType): Observable<ActivityType> {
    const url = `${environment.apiBaseUrl}/v1/aktivitettype/oppdater`;
    return this.http.put<ActivityType>(url, aktivitettype);
  }
}
