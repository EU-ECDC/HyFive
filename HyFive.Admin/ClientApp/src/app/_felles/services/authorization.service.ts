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
    return this.http.get<InnloggetBruker>('/account').pipe(tap(bruker => {
      let valgtRolle = this.hentValgtRolle();
      if (!valgtRolle) {
        if (bruker.erFhiAdmin) {
          this.lagreValgtRolle(AuthorizedRole.Administrator);
        } else if (bruker.erKoordinator) {
          this.lagreValgtRolle(AuthorizedRole.Koordinator);
        }
      }
    }));
  }

  getRoller(): Observable<AuthorizedRole[]> {
    return this.getBruker().pipe(map(bruker => {
      const authorizedRoles: AuthorizedRole[] = [];
      if (bruker.erFhiAdmin) {
        authorizedRoles.push(AuthorizedRole.Administrator);
      }
      
      if (bruker.erKoordinator) {
        authorizedRoles.push(AuthorizedRole.Koordinator);
      }

      return authorizedRoles;
    }));
  }

  loggUt() {
    localStorage.clear();
    window.location.href = '/account/logout';
  }

  lagreValgtRolle(rolle: AuthorizedRole): AuthorizedRole | null {
    localStorage.setItem(Localstoragepaths.ValgtRolle, JSON.stringify(rolle));
    return this.hentValgtRolle();
  }

  hentValgtRolle(): AuthorizedRole {
    const valgtRolleString = localStorage.getItem(Localstoragepaths.ValgtRolle);
    return valgtRolleString ? parseInt(valgtRolleString) : null;
  }

}
