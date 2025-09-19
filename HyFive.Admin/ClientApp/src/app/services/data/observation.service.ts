import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { FacilityOverviewReport } from '../../models/api/FacilityOverviewReport';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { SessionType } from '../../models/api/SessionType';
import { User } from '../../models/api/User';
import {HandJewelryObservation} from "../../models/api/HandJewelryObservation";
import {GloveObservation} from "../../models/api/GloveObservation";
import {ProtectiveEquipmentObservation} from "../../models/api/ProtectiveEquipmentObservation";
import { AuthorizedRole } from '../../_common/authorization/authorized-role';
import { FiveIndicatorsObservation } from 'src/app/models/api/FiveIndicatorsObservation';

@Injectable({
  providedIn: 'root'
})
export class ObservationService {

  constructor(private readonly http: HttpClient) { }

  getFacilitiesWithSessions(facilityId: string, sessionType: SessionType, fromDate: Date, toDate: Date, selectedRole: AuthorizedRole): Observable<FacilityOverviewReport[]> {
    let url = `${environment.apiBaseUrl}/v1/observation/facilitiesWithSessions`;

    let params = new HttpParams();
    if (sessionType && sessionType.toString() !== 'null')
      params = params.append("sessiontype", sessionType?.toString());

    if (fromDate !== null)
      params = params.append("fromdate", fromDate?.toString());

    if (toDate !== null)
      params = params.append("todate", toDate?.toString());

    if (facilityId !== null)
      params = params.append("facilityid", facilityId);

    params = params.append("role", selectedRole.toString());

    return this.http.get<FacilityOverviewReport[]>(url, { params: params });
  }

  getSessionsForDepartment(departmentid: number, sessiontype: SessionType, fromDate: Date, toDate: Date, selectedRole: AuthorizedRole): Observable<SessionOverviewReport[]> {
    const url = `${environment.apiBaseUrl}/v1/observation/department`;
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

  getSessionsForFacility(facilityId: number, observer?: User, sessiontype?: SessionType, fromDate?: Date, toDate?: Date): Observable<SessionOverviewReport[]> {
    const url = `${environment.apiBaseUrl}/v1/observation/facility`;
    let params = new HttpParams();

    params = params.append("facilityid", facilityId?.toString());

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

  transferSessionToFHI(facilityId: number, sessionId: any) {
    const url = `${environment.apiBaseUrl}/v1/observation/transfer`;
    let params = new HttpParams();

    params = params.append("facilityid", facilityId?.toString());
    params = params.append("sessionid", sessionId.toString());

    return this.http.get<SessionOverviewReport>(url, { params: params });
  }

  updateFiveIndicationsObservation(observation: FiveIndicatorsObservation) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observation/fiveindications/update`;
    return this.http.put<boolean>(url, observation);
  }

  deleteFiveIndicationsObservation(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/fiveindications/delete?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  updateHandJewelryObservation(observation: HandJewelryObservation) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observation/handjewelry/update`;
    return this.http.put<boolean>(url, observation);
  }

  deleteHandJewelryObservation(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/handjewelry/delete?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  updateGloveObservation(observation: GloveObservation) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observation/glove/update`;
    return this.http.put<boolean>(url, observation);
  }

  deleteGloveObservation(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/glove/delete?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  updateProtectiveEquipmentObservation(observation: ProtectiveEquipmentObservation) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/protectiveequipment/update`;
    return this.http.put<boolean>(url, observation);
  }

  deleteProtectiveEquipmentObservation(observationId: string, sessionId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observation/protectiveequipment/delete?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.delete<boolean>(url);
  }

  getProtectiveEquipmentObservation(observationId: string, sessionId: string) : Observable<ProtectiveEquipmentObservation> {
    const url = `${environment.apiBaseUrl}/v1/observation/protectiveequipment?observationId=${observationId}&sessionId=${sessionId}`;
    return this.http.get<ProtectiveEquipmentObservation>(url);
  }
}
