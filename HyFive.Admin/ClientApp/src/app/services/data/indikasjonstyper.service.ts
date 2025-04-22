import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IndicationType } from '../../models/api/IndicationType';
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class IndikasjonstyperService {

  constructor(private httpClient: HttpClient) {

  }

  hentIndikasjonstyper(): Observable<IndicationType[]> {
    const url = `${environment.apiBaseUrl}/v1/indicationtypes`;
    return this.httpClient.get<IndicationType[]>(url)
    .pipe();
  }

  oppdaterIndikasjonstype(indicationtype: IndicationType): Observable<IndicationType> {
    const url = `${environment.apiBaseUrl}/v1/indicationtypes/update`;
    return this.httpClient.put<IndicationType>(url, indicationtype)
    .pipe();
  }
}
