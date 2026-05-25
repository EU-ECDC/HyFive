import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { FacilityReport } from '../../models/api/FacilityReport';
import { CoordinatorForCity } from '../../models/api/CoordinatorForCity';
import { Status } from 'src/app/models/api/Status';
import { City } from 'src/app/models/api/City';

@Injectable({
  providedIn: 'root'
})
export class CityService {

  constructor(private readonly httpClient: HttpClient) { }

  getAllCities() : Observable<City[]> {
    const url = `${environment.apiBaseUrl}/v1/cities`;
    return this.httpClient.get<City[]>(url);
  }

  getCoordinatorsForCity(city: string): Observable<CoordinatorForCity[]> {
    const url = `${environment.apiBaseUrl}/v1/cities/${city}/coordinators`;
    return this.httpClient.get<CoordinatorForCity[]>(url);
  }

  getFacilitiesForCity(city: string): Observable<FacilityReport[]> {
    const url = `${environment.apiBaseUrl}/v1/cities/${city}/facilities`;
    return this.httpClient.get<FacilityReport[]>(url).pipe(
                                                          map(data => {
                                                            return data.sort((a,b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" }))
                                                          })
                                                      );
  }

  updateCoordinatorForCity(city: string, coordinator: CoordinatorForCity): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/cities/${city}/updatecoordinator`;
    return this.httpClient.put<Status>(url, coordinator);
  }

  createCoordinatorForCity(city: string, coordinator: CoordinatorForCity): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/cities/${city}/createcoordinator`;
    return this.httpClient.post<Status>(url, coordinator);
  }
}
