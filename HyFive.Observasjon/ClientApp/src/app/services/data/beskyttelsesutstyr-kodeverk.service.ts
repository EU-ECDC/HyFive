import { Injectable } from "@angular/core";
import { BeskyttelsesutstyrsettingType } from "src/app/models/api/BeskyttelsesutstyrsettingType";
import { HttpClient } from '@angular/common/http';
import { environment } from "src/environments/environment";
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class BeskyttelsesutstyrKodeverkService {

  constructor(private httpClient: HttpClient){}

  hentBeskyttelsesutstyrSettinger() : Observable<BeskyttelsesutstyrsettingType[]> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyr/`;
    return this.httpClient.get<BeskyttelsesutstyrsettingType[]>(url);
  }
}
