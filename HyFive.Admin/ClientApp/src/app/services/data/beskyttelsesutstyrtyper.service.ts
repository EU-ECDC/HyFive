import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BeskyttelsesutstyrType } from "src/app/models/api/BeskyttelsesutstyrType";
import { environment } from "src/environments/environment";
import { tap } from 'rxjs/operators';
import { FeilbrukType } from '../../models/api/FeilbrukType';
import { OpprettFeilbrukTypeRequest } from '../../models/api/OpprettFeilbrukTypeRequest';

@Injectable({
  providedIn: 'root'
})

export class BeskyttelsesutstyrtyperService {

  constructor(private httpClient: HttpClient) { }

  hentBeskyttelsesutstyrtyper(): Observable<BeskyttelsesutstyrType[]> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrtyper`;
    return this.httpClient.get<BeskyttelsesutstyrType[]>(url);
  }

  oppdaterBeskyttelsesutstyrtyper(utstyrtype: BeskyttelsesutstyrType): Observable<BeskyttelsesutstyrType> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrtyper/oppdater`;
    return this.httpClient.put<BeskyttelsesutstyrType>(url, utstyrtype);
  }

  hentFeilbruktyper(utstytypeId: number): Observable<FeilbrukType[]> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrtyper/feilbruktyper?utstyrTypeId=${utstytypeId}`;
    return this.httpClient.get<FeilbrukType[]>(url);
  }

  oppdaterFeilbruktype(utstyrtypeId: number, feilbruktype: FeilbrukType): Observable<FeilbrukType> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrtyper/feilbruktyper/oppdater?utstyrTypeId=${utstyrtypeId}`;
    return this.httpClient.put<FeilbrukType>(url, feilbruktype);
  }

  slettFeilbruktype(utstyrtypeId: number, feilbruktypeId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrtyper/feilbruktyper/slett?utstyrTypeId=${utstyrtypeId}&feilbrukTypeId=${feilbruktypeId}`;
    return this.httpClient.delete<boolean>(url);
  }

  opprettFeilbruktype(utstytypeId: number, feilbruk: OpprettFeilbrukTypeRequest): Observable<FeilbrukType> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrtyper/feilbruktyper/opprett?utstyrTypeId=${utstytypeId}`;
    return this.httpClient.post<FeilbrukType>(url, feilbruk);
  }

}
