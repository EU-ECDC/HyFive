import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import {UpdateSessionRequest} from "../../models/api/UpdateSessionRequest";

@Injectable({
  providedIn: 'root'
})
export class SessionService {

  constructor(private readonly http: HttpClient) { }

  deleteSession(sessionId: string, institutionId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/session/delete/${sessionId}?institutionId=${institutionId}`;
    return this.http.delete<boolean>(url);
  }

  updateSession(session: Partial<UpdateSessionRequest>): Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/session/update`;
    return this.http.put<Partial<UpdateSessionRequest>>(url, session);
  }
}
