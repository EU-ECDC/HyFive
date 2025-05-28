import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Role } from '../../models/api/Role';

@Injectable({
  providedIn: 'root'
})
export class RoleService {

  constructor(private readonly http: HttpClient) { }

  createRole(role: Role): Observable<Role> {
    const url = `${environment.apiBaseUrl}/v1/role`;
    return this.http.post<Role>(url, role);
  }

  updateRole(role: Role): Observable<Role> {
    const url = `${environment.apiBaseUrl}/v1/role`;
    return this.http.put<Role>(url, role);
  }

  getRoles(): Observable<Role[]> {
    const url = `${environment.apiBaseUrl}/v1/role`;
    return this.http.get<Role[]>(url);
  }
}
