import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IndikasjonType } from '../../models/api/IndikasjonType';
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class IndikasjonstyperService {

  constructor(private httpClient: HttpClient) {

  }

  hentIndikasjonstyper(): Observable<IndikasjonType[]> {
    const url = `${environment.apiBaseUrl}/v1/indikasjonstyper`;
    return this.httpClient.get<IndikasjonType[]>(url)
    .pipe();
  }

  oppdaterIndikasjonstype(indikasjonstype: IndikasjonType): Observable<IndikasjonType> {
    const url = `${environment.apiBaseUrl}/v1/indikasjonstyper/oppdater`;
    return this.httpClient.put<IndikasjonType>(url, indikasjonstype)
    .pipe();
  }
}
