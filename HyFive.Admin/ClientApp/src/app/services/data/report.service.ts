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

  getComplianceForFourIndications(institutionId: number, intervall: string, fromMonth: number, fromYear: number, toMonth: number, toYear: number, roleId: number, departmentId): Observable<any[]> {
    const url = `${environment.apiBaseUrl}/v1/report/fourindications/compliance`;

    let params = new HttpParams();
    params = params.append("institutionId", institutionId.toString());
    params = params.append("intervall", intervall);
    params = params.append("fromMonth", fromMonth);
    params = params.append("fromYear", fromYear);
    params = params.append("toMonth", toMonth);
    params = params.append("toYear", toYear);
    params = params.append("roleId", roleId);
    params = params.append("departmentId", departmentId);

    return this.httpClient.get<any[]>(url, { params: params });
  }

  reportForSessionTypeHasData(sessionType: SessionType, institutionId: number, departmentId: number,
    fromDate: Date, toDate: Date, roleId: AuthorizedRole): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/report/reportforsessiontypehasdata`;

    let params = new HttpParams();
    params = params.append("sesionType", sessionType);
    params = params.append("institutionId", institutionId.toString());
    if (departmentId != null)
      params = params.append("departmentId", departmentId);
    params = params.append("fromDate", fromDate.toString());
    params = params.append("toDate", toDate.toString());
    params = params.append("roleId", roleId);

    return this.httpClient.get<boolean>(url, { params: params });
 }
}
