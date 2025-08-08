import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { SessionType } from "src/app/models/api/SessionType";
import { AuthorizedRole } from "src/app/_common/authorization/authorized-role";
import { ReportForSessionTypeHasDataModel } from "src/app/models/api/reportForSessionTypeHasDataModel";

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

  reportForSessionTypeHasData(
      payload: ReportForSessionTypeHasDataModel): Observable<boolean> {
      const url = `${environment.apiBaseUrl}/v1/report/reportForSessionTypeHasData`;
      return this.httpClient.post<boolean>(url, payload);

 }
}
