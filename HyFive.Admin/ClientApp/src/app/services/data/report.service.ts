import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { ReportForSessionTypeHasDataModel } from "src/app/models/api/reportForSessionTypeHasDataModel";

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  constructor(private readonly httpClient: HttpClient) { }

  getComplianceForHandHygiene(payload: {
    facilityIds: number[];
    facilityTypeIds: number[];
    interval: string;
    fromMonth: number;
    fromYear: number;
    fromQuarter: number;
    toMonth: number;
    toYear: number;
    toQuarter: number;
    role: number;
    roleIds: number[];
    departmentIds: number[];
    departmentTypeIds: number[];
    unitIds: number[];
    transferredTo: number}): Observable<any[]> {
    const url = `${environment.apiBaseUrl}/v1/report/fiveIndications/compliance`;

    return this.httpClient.post<any[]>(url, payload);
  }

  reportForSessionTypeHasData(
      payload: ReportForSessionTypeHasDataModel): Observable<boolean> {
      const url = `${environment.apiBaseUrl}/v1/report/reportForSessionTypeHasData`;
      return this.httpClient.post<boolean>(url, payload);

 }
}
