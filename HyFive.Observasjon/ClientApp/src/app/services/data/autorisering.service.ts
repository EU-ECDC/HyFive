import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {LoggedInUser} from '../../models/api/LoggedInUser';
import {tap} from 'rxjs/operators';
import {Localstoragepaths} from '../../konstanter/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class AutoriseringService {


  constructor(private readonly http: HttpClient) { }

  erLoggetInn(): Observable<boolean> {
    const url = `/account/isloggedin`;
    return this.http.get<boolean>(url);
  }

  getBruker(): Observable<LoggedInUser> {
    return this.http.get<LoggedInUser>('/account').pipe(tap(bruker => {
      // TODO: midlertidig
      // let innloggetbruker = this.getLokalBrukerId();
      // if(innloggetbruker != bruker.id){
      //   localStorage.clear();
      // }
      this.setLokalBrukerId(bruker.id);
    }));
  }

  getLokalBrukerId(){
    return localStorage.getItem(Localstoragepaths.InnloggetBrukerId);
  }

  setLokalBrukerId(id: string) {
    localStorage.setItem(Localstoragepaths.InnloggetBrukerId, id)
  }
}
