import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IndicationType } from '../../models/api/IndicationType';
import { environment } from 'src/environments/environment';
import { InstitutionType } from '../../models/api/InstitutionType';
import { OpprettInstitusjonstypeRequest } from '../../models/api/OpprettInstitusjonstypeRequest';

@Injectable({
  providedIn: 'root'
})

export class InstitusjonstyperService {

  constructor(private httpClient: HttpClient) {

  }

  hentInstitusjonstyper(): Observable<InstitutionType[]> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes`;
    return this.httpClient.get<IndicationType[]>(url)
    .pipe();
  }

  oppdaterInstitusjonstype(institutiontype: InstitutionType): Observable<InstitutionType> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes/oppdater`;
    return this.httpClient.put<InstitutionType>(url, institutiontype)
    .pipe();
  }

  opprettInstitusjonstype(institutiontype: OpprettInstitusjonstypeRequest): Observable<InstitutionType> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes/opprett`;
    return this.httpClient.post<InstitutionType>(url, institutiontype)
    .pipe();
  }

  slettInstitusjonstype(institusjonstypeId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes/slett?institusjonstypeId=${institusjonstypeId}`;
    return this.httpClient.delete<boolean>(url)
    .pipe();
  }
}
