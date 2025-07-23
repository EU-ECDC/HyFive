import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { User } from '../../models/api/User';
import { CreateFhiAdminRequest } from '../../models/api/CreateFhiAdminRequest';
import { CoordinatorForHealthcareOrganization } from '../../models/api/CoordinatorForHealthcareOrganization';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private readonly http: HttpClient) { }

  // ---- Observator ----

  createObserver(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/create`;
    return this.http.post<User>(url, user);
  }

  updateObserver(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/update`;
    return this.http.put<User>(url, user);
  }

  deleteObserver(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/delete?observerId=${id}`;
    return this.http.delete<boolean>(url);
  }

  hasTransferredSessionToFHI(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/observer/hasTransferredSessionToFHI?observerId=${id}`;
    return this.http.get<boolean>(url);
  }

  // ---- Coordinator ----

  createCoordinator(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/coordinator/create`;
    return this.http.post<User>(url, user);
  }

  updateCoordinator(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/coordinator/update`;
    return this.http.put<User>(url, user);
  }

  deleteCoordinator(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/user/coordinator/delete?coordinatorId=${id}`;
    return this.http.delete<boolean>(url);
  }

  // ---- FhiAdmin ----

  getFhiAdmin(): Observable<User[]> {
    const url = `${environment.apiBaseUrl}/v1/user/fhiadmin`;
    return this.http.get<User[]>(url);
  }

  createAdmin(user: CreateFhiAdminRequest): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/fhiadmin`;
    return this.http.post<User>(url, user);
  }

  updateFhiAdmin(user: User): Observable<User> {
    const url = `${environment.apiBaseUrl}/v1/user/fhiadmin`;
    return this.http.put<User>(url, user);
  }

  hasValidHprnumberOrPseudonym(user: User): boolean{
    return user?.hprNumber != null || this.isValidPseudonym(user.identityPseudonym);
  }

  hasCoordinatorValidHprnumberOrPseudonym(coordinator: CoordinatorForHealthcareOrganization): boolean {
    return coordinator?.hprNumber?.trim().length > 0 || this.isValidPseudonym(coordinator.identityPseudonym);
  }

  isValidPseudonym(pseudonym: string): boolean{
    return (!pseudonym?.length || pseudonym?.length === 44);
  }
}
