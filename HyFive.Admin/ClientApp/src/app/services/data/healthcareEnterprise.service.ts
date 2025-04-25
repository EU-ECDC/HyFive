import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateHealthEnterpriseRequest } from 'src/app/models/api/CreateHealthEnterpriseRequest';
import { environment } from 'src/environments/environment';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { CoordinatorForHealthcareEnterprises } from '../../models/api/CoordinatorForHealthcareEnterprises';
import { Status } from 'src/app/models/api/Status';
import { HealthcareEnterprise } from '../../models/api/HealthcareEnterprise';

@Injectable({
  providedIn: 'root'
})
export class HealthcareEnterpriseService {

  constructor(private httpClient: HttpClient) { }

  createHealthcareEnterprise(healthcareEnterprise: CreateHealthEnterpriseRequest) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/create`;
    return this.httpClient.post<boolean>(url, healthcareEnterprise);
  }

  getAllHealthcareEnterprises() : Observable<HealthcareEnterprise[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise`;
    return this.httpClient.get<HealthcareEnterprise[]>(url);
  }

  updateHealthcareEnterprise(healthcareEnterprise: HealthcareEnterprise) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/update`;
    return this.httpClient.put<boolean>(url, healthcareEnterprise);
  }

  getCoordinators(id: number): Observable<CoordinatorForHealthcareEnterprises[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/coordinators`;
    return this.httpClient.get<CoordinatorForHealthcareEnterprises[]>(url);
  }

  getInstitutions(id: number): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/institutions`;
    return this.httpClient.get<InstitutionReport[]>(url);
  }

  updateCoordinator(id: number, coordinator: CoordinatorForHealthcareEnterprises): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/updatecoordinator`;
    return this.httpClient.put<Status>(url, coordinator);
  }

  createCoordinator(id: number, coordinator: CoordinatorForHealthcareEnterprises): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareenterprise/${id}/createcoordinator`;
    return this.httpClient.post<Status>(url, coordinator);
  }
}
