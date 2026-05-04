import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { User } from '../../models/api/User';
import { CreateAdminRequest } from '../../models/api/CreateAdminRequest';
import { CoordinatorForCity } from '../../models/api/CoordinatorForCity';
import { CreateCoordinatorRequest } from 'src/app/models/api/CreateCoordinatorRequest';
import { CreateObserverRequest } from 'src/app/models/api/CreateObserverRequest';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private readonly http: HttpClient) { }

  // ---- Observator ----

  createObserver(user: CreateObserverRequest): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/create`;
    return this.http.post<User>(url, user);
  }

  updateObserver(user: CreateObserverRequest): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/update`;
    return this.http.put<User>(url, user);
  }

  deleteObserver(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/delete?observerId=${id}`;
    return this.http.delete<boolean>(url);
  }

  hasTransferredSessionToFHI(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/HasTransferredSessionToAdmin?observatorId=${id}`;
    return this.http.get<boolean>(url);
  }

  // ---- Coordinator ----

  createCoordinator(user: CreateCoordinatorRequest): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/coordinator/create`;
    return this.http.post<User>(url, user);
  }

  updateCoordinator(user: CreateCoordinatorRequest): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/coordinator/update`;
    return this.http.put<User>(url, user);
  }

  deleteCoordinator(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/coordinator/delete?coordinatorId=${id}`;
    return this.http.delete<boolean>(url);
  }

  // ---- Admin ----

  getAdmin(): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/user/admin`;
    return this.http.get<User[]>(url);
  }

  createAdmin(user: CreateAdminRequest): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/admin`;
    return this.http.post<User>(url, user);
  }

  updateAdmin(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/admin`;
    return this.http.put<User>(url, user);
  }

  hasValidHprnumberOrPseudonym(user: User): boolean{
    return this.isValidPseudonym(user.identityPseudonym);
  }

  hasCoordinatorValidHprnumberOrPseudonym(coordinator: CoordinatorForCity): boolean {
    return this.isValidPseudonym(coordinator.identityPseudonym);
  }

  isValidPseudonym(pseudonym: string): boolean{
    return (!pseudonym?.length || pseudonym?.length === 44);
  }
}
