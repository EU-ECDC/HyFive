import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { SessionType } from "src/app/models/api/SessionType";
import { AuthorizedRole } from "src/app/_felles/authorization/authorized-role";

@Injectable({
  providedIn: 'root'
})
export class RapportService {
  constructor(private httpClient: HttpClient) { }

  hentEtterlevelseForFireindikasjoner(institutionId: number, intervall: string, fraManed: number, fraAr: number, tilManed: number, tilAr: number, rolleId: number, avdelingId): Observable<any[]> {
    const url = `${environment.apiBaseUrl}/v1/rapport/fireindikasjoner/etterlevelse`;

    let params = new HttpParams();
    params = params.append("institutionId", institutionId.toString());
    params = params.append("intervall", intervall);
    params = params.append("fraManed", fraManed);
    params = params.append("fraAr", fraAr);
    params = params.append("tilManed", tilManed);
    params = params.append("tilAr", tilAr);
    params = params.append("rolleId", rolleId);
    params = params.append("avdelingId", avdelingId);

    return this.httpClient.get<any[]>(url, { params: params });
  }

  rapportForSessionTypeHarData(sesjonType: SessionType, institutionId: number, avdelingId: number,
    fromDate: Date, toDate: Date, rolleId: AuthorizedRole): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/rapport/rapportforsesjontypehardata`;

    let params = new HttpParams();
    params = params.append("sesjonType", sesjonType);
    params = params.append("institutionId", institutionId.toString());
    if (avdelingId != null)
      params = params.append("avdelingId", avdelingId);
    params = params.append("fromDate", fromDate.toString());
    params = params.append("toDate", toDate.toString());
    params = params.append("rolleId", rolleId);

    return this.httpClient.get<boolean>(url, { params: params });
 }
}
