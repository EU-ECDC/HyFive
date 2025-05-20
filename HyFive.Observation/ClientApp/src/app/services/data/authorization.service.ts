import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {LoggedInUser} from '../../models/api/LoggedInUser';
import {tap} from 'rxjs/operators';
import {Localstoragepaths} from '../../constants/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {


  constructor(private readonly http: HttpClient) { }

  isLoggedIn(): Observable<boolean> {
    const url = `/account/isloggedin`;
    return this.http.get<boolean>(url);
  }

  getUser(): Observable<LoggedInUser> {
    return this.http.get<LoggedInUser>('/account').pipe(tap(user => {
      // TODO: midlertidig
      // let innloggetbruker = this.getLocalUserId();
      // if(innloggetbruker != user.id){
      //   localStorage.clear();
      // }
      this.setLocalUserId(user.id);
    }));
  }

  getLocalUserId(){
    return localStorage.getItem(Localstoragepaths.LoggedInUserId);
  }

  setLocalUserId(id: string) {
    localStorage.setItem(Localstoragepaths.LoggedInUserId, id)
  }
}
