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

  getRegions(): Observable<Region[]> {
    const url = `${environment.apiBaseUrl}/v1/regions`;
    return this.httpClient.get<Region[]>(url);
  }

  createRegion(region: Region): Observable<Region> {
    const url = `${environment.apiBaseUrl}/v1/regions/create`;
    return this.httpClient.post<Region>(url, region);
  }

  updateRegion(region: Region): Observable<Region> {
    const url = `${environment.apiBaseUrl}/v1/regions/update`;
    return this.httpClient.put<Region>(url, region);
  }
}
