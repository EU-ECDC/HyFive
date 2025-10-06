import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Unit } from '../../models/api/Unit';


@Injectable({
  providedIn: 'root'
})
export class UnitService {

  constructor(private readonly http: HttpClient) { }

  getUnit(id: number, facilityId: number): Observable<Unit> {
    const url = `${environment.apiBaseUrl}/v1/unit/${id}?facilityId=${facilityId}`;
    return this.http.get<Unit>(url);
  }

  getUnitsForFacility(facilityId: number): Observable<Unit[]> {
    const url = `${environment.apiBaseUrl}/v1/unit/facility/${facilityId}`;
    return this.http.get<Unit[]>(url);
  }

  createUnit(unit: Unit): Observable<Unit> {
    const url = `${environment.apiBaseUrl}/v1/unit/create`;
    return this.http.post<Unit>(url, unit);
  }

  updateUnit(unit: Unit): Observable<Unit> {
    const url = `${environment.apiBaseUrl}/v1/unit/update`;
    return this.http.put<Unit>(url, unit);
  }
}
