import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Clinic } from '../../models/api/Clinic';


@Injectable({
  providedIn: 'root'
})
export class ClinicService {

  constructor(private readonly http: HttpClient) { }

  getClinic(id: number, facilityId: number): Observable<Clinic> {
    const url = `${environment.apiBaseUrl}/v1/clinic/${id}?facilityId=${facilityId}`;
    return this.http.get<Clinic>(url);
  }

  getClinicsForFacility(facilityId: number): Observable<Clinic[]> {
    const url = `${environment.apiBaseUrl}/v1/clinic/facility/${facilityId}`;
    return this.http.get<Clinic[]>(url);
  }

  createClinic(clinic: Clinic): Observable<Clinic> {
    const url = `${environment.apiBaseUrl}/v1/clinic/create`;
    return this.http.post<Clinic>(url, clinic);
  }

  updateClinic(clinic: Clinic): Observable<Clinic> {
    const url = `${environment.apiBaseUrl}/v1/clinic/update`;
    return this.http.put<Clinic>(url, clinic);
  }
}
