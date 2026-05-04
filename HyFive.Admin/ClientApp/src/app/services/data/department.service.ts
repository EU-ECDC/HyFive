import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../../environments/environment';
import {CreateDepartmentRequest} from '../../models/api/CreateDepartmentRequest';
import {Role} from "../../models/api/Role";
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';
import { UpdateDepartmentRequest } from 'src/app/models/api/UpdateDepartmentRequest';


@Injectable({
  providedIn: 'root'
})
export class DepartmentService {

  constructor(private readonly http: HttpClient) { }

  getDepartment(id: number): Observable<OrganisationUnit> {
    const url = `${environment.apiBaseUrl}/v1/department/${id}`;
    return this.http.get<OrganisationUnit>(url);
  }

  createDepartment(department: CreateDepartmentRequest): Observable<OrganisationUnit>{
    const url = `${environment.apiBaseUrl}/v1/department/create`;
    return this.http.post<OrganisationUnit>(url, department);
  }

  updateDepartment(department: UpdateDepartmentRequest): Observable<OrganisationUnit> {
    const url = `${environment.apiBaseUrl}/v1/department/update`;
    return this.http.put<OrganisationUnit>(url, department);
  }

  getDepartmentTypes(): Observable<OrganisationUnitType[]>{
    const url = `${environment.apiBaseUrl}/v1/department/departmenttypes/`;
    return this.http.get<OrganisationUnitType[]>(url);
  }

  createDepartmentType(departmenttype: OrganisationUnitType): Observable<OrganisationUnitType>{
    const url = `${environment.apiBaseUrl}/v1/department/departmentType/create`;
    return this.http.post<OrganisationUnitType>(url, departmenttype);
  }

  updateDepartmentType(departmenttype: OrganisationUnitType): Observable<OrganisationUnitType> {
    const url = `${environment.apiBaseUrl}/v1/department/departmenttypes/update`;
    return this.http.put<OrganisationUnitType>(url, departmenttype);
  }

  getRole(id: number): Observable<Role[]> {
    const url = `${environment.apiBaseUrl}/v1/department/${id}/roles`;
    return this.http.get<Role[]>(url);
  }

  deleteDepartment(id: number): Observable<void> {
    const url = `${environment.apiBaseUrl}/v1/department/delete/${id}`;
    return this.http.delete<void>(url);
  }

  hasTransferredSessionToFHI(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/department/hasTransferredSessionToFHI/${id}`;
    return this.http.get<boolean>(url);
  }
}
