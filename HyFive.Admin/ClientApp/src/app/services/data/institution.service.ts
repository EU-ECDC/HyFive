import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { Institution } from '../../models/api/Institution';
import { CreateInstitutionRequest } from '../../models/api/CreateInstitutionRequest';
import { InstitutionType } from '../../models/api/InstitutionType';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { User } from '../../models/api/User';
import { Department} from "../../models/api/Department";
import { Localstoragepaths } from '../../_common/konstanter/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class InstitutionService {

  constructor(private readonly http: HttpClient) { }

  getInstitutions(): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/`;
    return this.http.get<Institution[]>(url);
  }


    getInstitutionsPaginated(offset, limit): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/`;

    let params = new HttpParams();
    if (offset) {
      params.append("offset", offset?.toString());
    }
    if (limit) {
      params.append("limit", limit?.toString());
    }

    console.log("offset", offset, "limit", limit);

    return this.http.get<Institution[]>(url);
    // return this.http.get<Institution[]>(url, { params });
  }

  getInstitutionsForCoordinator(): Observable<InstitutionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/getinstitutionsforcoordinator/`;
    return this.http.get<InstitutionReport[]>(url);
  }

  getInstitution(id: number): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}`;
    return this.http.get<Institution>(url);
  }

  getSelectedInstitutionId(): number | null {
    const selectedInstitutionIdString = localStorage.getItem(Localstoragepaths.SelectedInstitution);
    return selectedInstitutionIdString ? parseInt(selectedInstitutionIdString) : null;
  }

  updateSelectedInstitutionId(institutionId: number): number | null {
    localStorage.setItem(Localstoragepaths.SelectedInstitution, JSON.stringify(institutionId));
    return this.getSelectedInstitutionId();
  }

  getObservers(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}/observers`;
    return this.http.get<User[]>(url);
  }

  getCoordinators(id: number): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}/coordinators`;
    return this.http.get<User[]>(url);
  }

  getDepartments(id: number): Observable<Department[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/${id}/departments/`;
    return this.http.get<Department[]>(url);
  }

  getInstitutionTypes(): Observable<InstitutionType[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/types`;
    return this.http.get<InstitutionType[]>(url);
  }

  createInstitution(request: CreateInstitutionRequest): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/institution/create`;
    return this.http.post<Institution>(url, request);
  }

  updateInstitution(institution: Institution): Observable<Institution> {
    const url = `${environment.apiBaseUrl}/v1/institution/update`;
    return this.http.put<Institution>(url, institution);
  }

  deleteInstitution(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/institution/delete?institutionId=${id}`;
    return this.http.delete<boolean>(url);
  }
}
