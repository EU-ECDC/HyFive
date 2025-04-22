import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { BraceletType } from '../../models/api/BraceletType';


@Injectable({
  providedIn: 'root'
})
export class HandsmykketypeService {

  constructor(private readonly http: HttpClient) { }

  hentHandsmykketyper(): Observable<BraceletType[]> {
    const url = `${environment.apiBaseUrl}/v1/handsmykketype/`;
    return this.http.get<BraceletType[]>(url);
  }

  oppdaterHandsmykketype(handsmykketype: BraceletType): Observable<BraceletType> {
    const url = `${environment.apiBaseUrl}/v1/handsmykketype/oppdater`;
    return this.http.put<BraceletType>(url, handsmykketype);
  }
}
