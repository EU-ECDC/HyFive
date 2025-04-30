import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateHealthcareOrganizationRequest } from 'src/app/models/api/CreateHealthcareOrganizationRequest';
import { environment } from 'src/environments/environment';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { CoordinatorForHealthcareOrganization } from '../../models/api/CoordinatorForHealthcareOrganization';
import { Status } from 'src/app/models/api/Status';
import { HealthcareOrganization } from '../../models/api/HealthcareOrganization';

@Injectable({
  providedIn: 'root'
})
export class HealthcareOrganizationService {

  constructor(private httpClient: HttpClient) { }

  createHealthcareOrganization(healthcareOrganization: CreateHealthcareOrganizationRequest) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/create`;
    return this.httpClient.post<boolean>(url, healthcareOrganization);
  }

  getAllHealthcareOrganizations() : Observable<HealthcareOrganization[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization`;
    return this.httpClient.get<HealthcareOrganization[]>(url);
  }

  updateHealthcareOrganization(healthcareOrganization: HealthcareOrganization) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/update`;
    return this.httpClient.put<boolean>(url, healthcareOrganization);
  }

  getCoordinators(id: number): Observable<CoordinatorForHealthcareOrganization[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/coordinators`;
    return this.httpClient.get<CoordinatorForHealthcareOrganization[]>(url);
  }

  getInstitutions(id: number): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/institutions`;
    return this.httpClient.get<InstitutionReport[]>(url);
  }

  updateCoordinator(id: number, coordinator: CoordinatorForHealthcareOrganization): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/updatecoordinator`;
    return this.httpClient.put<Status>(url, coordinator);
  }

  createCoordinator(id: number, coordinator: CoordinatorForHealthcareOrganization): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/createcoordinator`;
    return this.httpClient.post<Status>(url, coordinator);
  }
}
