import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { FacilityReport } from '../../models/api/FacilityReport';
import { CoordinatorForCity } from '../../models/api/CoordinatorForCity';
import { Status } from 'src/app/models/api/Status';
import { City } from '../../models/api/City';

@Injectable({
  providedIn: 'root'
})
export class CityService {

  constructor(private httpClient: HttpClient) { }

  getAllCities() : Observable<City[]> {
    const url = `${environment.apiBaseUrl}/v1/city`;
    return this.httpClient.get<City[]>(url);
  }

  getCoordinators(id: number): Observable<CoordinatorForCity[]> {
    const url = `${environment.apiBaseUrl}/v1/city/${id}/coordinators`;
    return this.httpClient.get<CoordinatorForCity[]>(url);
  }

  getFacilities(id: number): Observable<FacilityReport[]> {
    const url = `${environment.apiBaseUrl}/v1/city/${id}/facilities`;
    return this.httpClient.get<FacilityReport[]>(url).pipe(
                                                          map(data => data.sort((a,b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" })))
                                                          );
  }

  updateCoordinator(id: number, coordinator: CoordinatorForCity): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/city/${id}/updatecoordinator`;
    return this.httpClient.put<Status>(url, coordinator);
  }

  createCoordinator(id: number, coordinator: CoordinatorForCity): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/city/${id}/createcoordinator`;
    return this.httpClient.post<Status>(url, coordinator);
  }
}
