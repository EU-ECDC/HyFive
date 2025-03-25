import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IndikasjonType } from '../../models/api/IndikasjonType';
import { environment } from 'src/environments/environment';
import { InstitusjonType } from '../../models/api/InstitusjonType';
import { OpprettInstitusjonstypeRequest } from '../../models/api/OpprettInstitusjonstypeRequest';

@Injectable({
  providedIn: 'root'
})

export class InstitusjonstyperService {

  constructor(private httpClient: HttpClient) {

  }

  hentInstitusjonstyper(): Observable<InstitusjonType[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjonstyper`;
    return this.httpClient.get<IndikasjonType[]>(url)
    .pipe();
  }

  oppdaterInstitusjonstype(institusjonstype: InstitusjonType): Observable<InstitusjonType> {
    const url = `${environment.apiBaseUrl}/v1/institusjonstyper/oppdater`;
    return this.httpClient.put<InstitusjonType>(url, institusjonstype)
    .pipe();
  }

  opprettInstitusjonstype(institusjonstype: OpprettInstitusjonstypeRequest): Observable<InstitusjonType> {
    const url = `${environment.apiBaseUrl}/v1/institusjonstyper/opprett`;
    return this.httpClient.post<InstitusjonType>(url, institusjonstype)
    .pipe();
  }

  slettInstitusjonstype(institusjonstypeId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/institusjonstyper/slett?institusjonstypeId=${institusjonstypeId}`;
    return this.httpClient.delete<boolean>(url)
    .pipe();
  }
}
