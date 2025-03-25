import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BeskyttelsesutstyrsettingType } from "src/app/models/api/BeskyttelsesutstyrsettingType";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class BeskyttelsesutstyrsettingtyperService {
  constructor(private httpClient: HttpClient) { }

  hentBeskyttelsesutstyrsettingtyper(): Observable<BeskyttelsesutstyrsettingType[]> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrsettingtyper`;
    return this.httpClient.get<BeskyttelsesutstyrsettingType[]>(url);
  }

  oppdaterBeskyttelsesutstyrsettingType(settingType: BeskyttelsesutstyrsettingType): Observable<BeskyttelsesutstyrsettingType> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyrsettingtyper/oppdater`;
    return this.httpClient.put<BeskyttelsesutstyrsettingType>(url, settingType);
  }
}
