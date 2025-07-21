import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { SessionType } from "src/app/models/api/SessionType";
import { AuthorizedRole } from "src/app/_common/authorization/authorized-role";

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  constructor(private httpClient: HttpClient) { }

  getComplianceForFiveIndications(payload: {
    institutionIds: number[];
    institutionTypeIds: number[];
    interval: string;
    fromMonth: number;
    fromYear: number;
    fromQuarter: number;
    toMonth: number;
    toYear: number;
    toQuarter: number;
    roleIds: number[];
    departmentIds: number[];
    departmentTypeIds: number[];
    transferredTo: number}): Observable<any[]> {
    const url = `${environment.apiBaseUrl}/v1/report/fiveΙndications/compliance`;

    return this.httpClient.post<any[]>(url, payload);
  }

  reportForSessionTypeHasData(sessionType: SessionType, institutionId: number, departmentId: number,
    fromDate: Date, toDate: Date, roleId: AuthorizedRole): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/report/reportForSessionTypeHasData`;

    let params = new HttpParams();
    params = params.append("sessionType", sessionType);
    params = params.append("institutionId", institutionId.toString());
    if (departmentId != null)
      params = params.append("departmentId", departmentId);
    params = params.append("fromDate", fromDate.toString());
    params = params.append("toDate", toDate.toString());
    params = params.append("roleId", roleId);

    return this.httpClient.get<boolean>(url, { params: params });
 }
}
