import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthorizedRole } from '../authorization/authorized-role';
import { map, tap } from 'rxjs/operators';
import { Localstoragepaths } from '../konstanter/localstoragepaths';
import { LoggedInUser } from 'src/app/models/api/LoggedInUser';
@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  constructor(private http: HttpClient) {
  }

  getUser(): Observable<LoggedInUser> {
    return this.http.get<LoggedInUser>('/account').pipe(tap(user => {
      let selectedRole = this.getSelectedRole();
      if (!selectedRole) {
        if (user.isFhiAdmin) {
          this.saveSelectedRole(AuthorizedRole.Administrator);
        } else if (user.isCoordinator) {
          this.saveSelectedRole(AuthorizedRole.Coordinator);
        }
      }
    }));
  }

  getRoles(): Observable<AuthorizedRole[]> {
    return this.getUser().pipe(map(user => {
      const authorizedRoles: AuthorizedRole[] = [];
      if (user.isFhiAdmin) {
        authorizedRoles.push(AuthorizedRole.Administrator);
      }
      
      if (user.isCoordinator) {
        authorizedRoles.push(AuthorizedRole.Coordinator);
      }

      return authorizedRoles;
    }));
  }

  logout() {
    localStorage.clear();
    window.location.href = '/account/logout';
  }

  saveSelectedRole(role: AuthorizedRole): AuthorizedRole | null {
    localStorage.setItem(Localstoragepaths.SelectedRole, JSON.stringify(role));
    return this.getSelectedRole();
  }

  getSelectedRole(): AuthorizedRole {
    const selectedRoleString = localStorage.getItem(Localstoragepaths.SelectedRole);
    return selectedRoleString ? parseInt(selectedRoleString) : null;
  }

}
