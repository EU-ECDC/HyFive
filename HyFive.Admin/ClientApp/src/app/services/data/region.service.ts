import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Region } from '../../models/api/Region';
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class RegionService {

  constructor(private httpClient: HttpClient) { }

  hentRegioner(): Observable<Region[]> {
    const url = `${environment.apiBaseUrl}/v1/regioner`;
    return this.httpClient.get<Region[]>(url);
  }

  opprettRegion(region: Region): Observable<Region> {
    const url = `${environment.apiBaseUrl}/v1/regioner/opprett`;
    return this.httpClient.post<Region>(url, region);
  }

  oppdaterRegion(region: Region): Observable<Region> {
    const url = `${environment.apiBaseUrl}/v1/regioner/oppdater`;
    return this.httpClient.put<Region>(url, region);
  }
}
