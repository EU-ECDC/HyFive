import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateHealthOrganisationRequest } from 'src/app/models/api/CreateHealthOrganisationRequest';
import { environment } from 'src/environments/environment';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { CoordinatorForHealthcareCompanies } from '../../models/api/CoordinatorForHealthcareCompanies';
import { Status } from 'src/app/models/api/Status';
import { HealthcareOrganisation } from '../../models/api/HealthcareOrganisation';

@Injectable({
  providedIn: 'root'
})
export class HealthcareOrganizationService {

  constructor(private httpClient: HttpClient) { }

  createHealthcareOrganization(healthcareorganization: CreateHealthOrganisationRequest) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/create`;
    return this.httpClient.post<boolean>(url, healthcareorganization);
  }

  getAllHealthcareOrganizations() : Observable<HealthcareOrganisation[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization`;
    return this.httpClient.get<HealthcareOrganisation[]>(url);
  }

  updateHealthcareOrganization(healthcareorganization: HealthcareOrganisation) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/update`;
    return this.httpClient.put<boolean>(url, healthcareorganization);
  }

  getCoordinators(id: number): Observable<CoordinatorForHealthcareCompanies[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/coordinators`;
    return this.httpClient.get<CoordinatorForHealthcareCompanies[]>(url);
  }

  getInstitutions(id: number): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/institutions`;
    return this.httpClient.get<InstitutionReport[]>(url);
  }

  updateCoordinator(id: number, coordinator: CoordinatorForHealthcareCompanies): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/updatecoordinator`;
    return this.httpClient.put<Status>(url, coordinator);
  }

  createCoordinator(id: number, coordinator: CoordinatorForHealthcareCompanies): Observable<Status> {
    const url = `${environment.apiBaseUrl}/v1/healthcareorganization/${id}/createcoordinator`;
    return this.httpClient.post<Status>(url, coordinator);
  }
}
