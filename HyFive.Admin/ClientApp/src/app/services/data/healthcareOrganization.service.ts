import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateHealthEnterpriseRequest } from 'src/app/models/api/CreateHealthEnterpriseRequest';
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

  createHealthcareEnterprise(healthcareOrganization: CreateHealthEnterpriseRequest) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/create`;
    return this.httpClient.post<boolean>(url, healthcareOrganization);
  }

  getAllHealthcareEnterprises() : Observable<HealthcareOrganization[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise`;
    return this.httpClient.get<HealthcareOrganization[]>(url);
  }

  updateHealthcareEnterprise(healthcareOrganization: HealthcareOrganization) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/update`;
    return this.httpClient.put<boolean>(url, healthcareOrganization);
  }

  getCoordinators(id: number): Observable<CoordinatorForHealthcareOrganization[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/coordinators`;
    return this.httpClient.get<CoordinatorForHealthcareOrganization[]>(url);
  }

  getInstitutions(id: number): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/institutions`;
    return this.httpClient.get<InstitutionReport[]>(url);
  }

  updateCoordinator(id: number, coordinator: CoordinatorForHealthcareOrganization): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/updatecoordinator`;
    return this.httpClient.put<Status>(url, coordinator);
  }

  createCoordinator(id: number, coordinator: CoordinatorForHealthcareOrganization): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/createcoordinator`;
    return this.httpClient.post<Status>(url, coordinator);
  }
}
