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

  deleteSession(sessionId: string, facilityId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/session/delete/${sessionId}?facilityId=${facilityId}`;
    return this.http.delete<boolean>(url);
  }

  updateSession(session: Partial<UpdateSessionRequest>): Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/session/update`;
    return this.http.put<Partial<UpdateSessionRequest>>(url, session);
  }
}
