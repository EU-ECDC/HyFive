import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { InnloggetBruker } from '../../models/api/InnloggetBruker';
import { Observable } from 'rxjs';
import { AuthorizedRole } from '../authorization/authorized-role';
import { map, tap } from 'rxjs/operators';
import { Localstoragepaths } from '../konstanter/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  constructor(private http: HttpClient) {
  }

  getBruker(): Observable<InnloggetBruker> {
    return this.http.get<InnloggetBruker>('/account').pipe(tap(user => {
      let valgtRolle = this.hentValgtRolle();
      if (!valgtRolle) {
        if (user.erFhiAdmin) {
          this.lagreValgtRolle(AuthorizedRole.Administrator);
        } else if (user.erKoordinator) {
          this.lagreValgtRolle(AuthorizedRole.Coordinator);
        }
      }
    }));
  }

  getRoller(): Observable<AuthorizedRole[]> {
    return this.getBruker().pipe(map(user => {
      const authorizedRoles: AuthorizedRole[] = [];
      if (user.erFhiAdmin) {
        authorizedRoles.push(AuthorizedRole.Administrator);
      }
      
      if (user.erKoordinator) {
        authorizedRoles.push(AuthorizedRole.Coordinator);
      }

      return authorizedRoles;
    }));
  }

  loggUt() {
    localStorage.clear();
    window.location.href = '/account/logout';
  }

  lagreValgtRolle(role: AuthorizedRole): AuthorizedRole | null {
    localStorage.setItem(Localstoragepaths.SelectedRole, JSON.stringify(role));
    return this.hentValgtRolle();
  }

  hentValgtRolle(): AuthorizedRole {
    const valgtRolleString = localStorage.getItem(Localstoragepaths.SelectedRole);
    return valgtRolleString ? parseInt(valgtRolleString) : null;
  }

}
