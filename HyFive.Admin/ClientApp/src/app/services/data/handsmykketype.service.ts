import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { HandsmykkeType } from '../../models/api/HandsmykkeType';

@Injectable({
  providedIn: 'root'
})
export class HandsmykketypeService {

  constructor(private readonly http: HttpClient) { }

  hentHandsmykketyper(): Observable<HandsmykkeType[]> {
    const url = `${environment.apiBaseUrl}/v1/handsmykketype/`;
    return this.http.get<HandsmykkeType[]>(url);
  }

  oppdaterHandsmykketype(handsmykketype: HandsmykkeType): Observable<HandsmykkeType> {
    const url = `${environment.apiBaseUrl}/v1/handsmykketype/oppdater`;
    return this.http.put<HandsmykkeType>(url, handsmykketype);
  }
}
