import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { InstitutionOverviewReport } from '../../models/api/InstitutionOverviewReport';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { SessionType } from '../../models/api/SessionType';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { User } from '../../models/api/User';
import {BraceletObservation} from "../../models/api/BraceletObservation";
import {HanskeObservasjon} from "../../models/api/HanskeObservasjon";
import {BeskyttelsesutstyrObservasjon} from "../../models/api/BeskyttelsesutstyrObservasjon";
import { AuthorizedRole } from '../../_felles/authorization/authorized-role';

@Injectable({
  providedIn: 'root'
})
export class ObservationService {

  constructor(private readonly http: HttpClient) { }

  getInstitutionsWithSessions(institutionId: string, sessionType: SessionType, fromDate: Date, toDate: Date, selectedRole: AuthorizedRole): Observable<InstitutionOverviewReport[]> {
    let url = `${environment.apiBaseUrl}/v1/observation/institutionsWithSessions`;

    let params = new HttpParams();
    if (sessionType && sessionType.toString() !== 'null')
      params = params.append("sessiontype", sessionType?.toString());

    if (fromDate !== null)
      params = params.append("fromdate", fromDate?.toString());

    if (toDate !== null)
      params = params.append("todate", toDate?.toString());

    if (institutionId !== null)
      params = params.append("institutionid", institutionId);

    params = params.append("role", selectedRole.toString());

    return this.http.get<InstitutionOverviewReport[]>(url, { params: params });
  }

  getSessionsForDepartment(departmentid: number, sessiontype: SessionType, fromDate: Date, toDate: Date, selectedRole: AuthorizedRole): Observable<SessionOverviewReport[]> {
    const url = `${environment.apiBaseUrl}/v1/observation/avdeling`;
    let params = new HttpParams();

    if (departmentid !== null)
      params = params.append("departmentid", departmentid?.toString());

    if (sessiontype && sessiontype.toString() !== 'null')
      params = params.append("sessiontype", sessiontype?.toString());

    if (fromDate !== null)
      params = params.append("fromdate", fromDate?.toString());

    if (toDate !== null)
      params = params.append("todate", toDate?.toString());

    params = params.append("role", selectedRole.toString());

    return this.http.get<SessionOverviewReport[]>(url, { params: params });
  }

  getSessionsForInstitution(institutionId: number, observer?: User, sessiontype?: SessionType, fromDate?: Date, toDate?: Date): Observable<SessionOverviewReport[]> {
    const url = `${environment.apiBaseUrl}/v1/observation/institusjon`;
    let params = new HttpParams();

    params = params.append("institutionid", institutionId?.toString());

    if (observer)
      params = params.append("observerid", observer?.toString());

    if (sessiontype && sessiontype.toString() !== 'null' )
      params = params.append("sessiontype", sessiontype?.toString());

    if (fromDate)
      params = params.append("fromdate", fromDate?.toString());

    if (toDate)
      params = params.append("todate", toDate?.toString());

    return this.http.get<SessionOverviewReport[]>(url, { params: params });
  }

  oppositeSessionToFHI(institutionId: number, sessionId: any) {
    const url = `${environment.apiBaseUrl}/v1/observation/opposite`;
    let params = new HttpParams();

    params = params.append("institutionid", institutionId?.toString());
    params = params.append("sessionid", sessionId.toString());

    return this.http.get<SessionOverviewReport>(url, { params: params });
  }

  updateFourIndicationsObservation(observation: FourIndicationsObservation) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observation/fourindications/update`;
    return this.http.put<boolean>(url, observation);
  }

  deleteFourIndicationsObservation(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/fourindications/delete?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  updateHandJewelryObservation(observation: BraceletObservation) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observation/handjewelry/update`;
    return this.http.put<boolean>(url, observation);
  }

  deleteHandJewelryObservation(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/handjewelry/delete?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  oppdaterHanskeObservasjon(observation: HanskeObservasjon) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observation/hanske/oppdater`;
    return this.http.put<boolean>(url, observation);
  }

  slettHanskeObservasjon(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/hanske/slett?observajonId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  oppdaterBeskyttelsesutstyrObservasjon(observation: BeskyttelsesutstyrObservasjon) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/beskyttelsesutstyr/oppdater`;
    return this.http.put<boolean>(url, observation);
  }

  slettBeskyttelsesutstyrObservasjon(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/beskyttelsesutstyr/slett?observajonId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  hentBeskyttelsesutstyrObservasjon(observationId: string, sessionId: string) : Observable<BeskyttelsesutstyrObservasjon> {
    const url = `${environment.apiBaseUrl}/v1/observation/beskyttelsesutstyr?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.get<BeskyttelsesutstyrObservasjon>(url);
  }
}
