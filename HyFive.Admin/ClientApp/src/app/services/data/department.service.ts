import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../../environments/environment';
import {Department} from '../../models/api/Department';
import {CreateDepartmentRequest} from '../../models/api/CreateDepartmentRequest';
import {Role} from "../../models/api/Role";
import {DepartmentType} from "../../models/api/DepartmentType";


@Injectable({
  providedIn: 'root'
})
export class DepartmentService {

  constructor(private readonly http: HttpClient) { }

  getDepartment(id: number): Observable<Department> {
    const url = `${environment.apiBaseUrl}/v1/department/${id}`;
    return this.http.get<Department>(url);
  }

  createDepartment(department: CreateDepartmentRequest): Observable<Department>{
    const url = `${environment.apiBaseUrl}/v1/department/create`;
    return this.http.post<Department>(url, department);
  }

  updateDepartment(department: Department): Observable<Department> {
    const url = `${environment.apiBaseUrl}/v1/department/update`;
    return this.http.put<Department>(url, department);
  }

  getDepartmentTypes(): Observable<DepartmentType[]>{
    const url = `${environment.apiBaseUrl}/v1/department/departmenttypes/`;
    return this.http.get<DepartmentType[]>(url);
  }

  createDepartmentType(departmenttype: DepartmentType): Observable<DepartmentType>{
    const url = `${environment.apiBaseUrl}/v1/department/departmenttypes/create`;
    return this.http.post<DepartmentType>(url, departmenttype);
  }

  updateDepartmentType(departmenttype: DepartmentType): Observable<DepartmentType> {
    const url = `${environment.apiBaseUrl}/v1/department/departmenttypes/update`;
    return this.http.put<DepartmentType>(url, departmenttype);
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
