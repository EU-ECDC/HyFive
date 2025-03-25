import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ForesporselOmBrukertilgang } from "../../models/api/ForesporselOmBrukertilgang";
import { ForesporselStatus } from 'src/app/models/api/ForesporselStatus';

@Injectable({
  providedIn: 'root'
})
export class ForesporselOmBrukertilgangService {

  constructor(private readonly http: HttpClient) { }

  hentAlleForesporsler(institusjonId: number): Observable<ForesporselOmBrukertilgang[]> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/alleforesporsler`;

    let params = new HttpParams();
    params = params.append("institusjonId", institusjonId.toString());

    return this.http.get<ForesporselOmBrukertilgang[]>(url, {params: params});
  }

  hentForesporslerSomVenterPaGodkjenning(institusjonId: number): Observable<ForesporselOmBrukertilgang[]> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/foresporslersomventerpagodkjenning`;

    let params = new HttpParams();
    params = params.append("institusjonId", institusjonId.toString());

    return this.http.get<ForesporselOmBrukertilgang[]>(url, {params: params});
  }

  godkjennForesporsel(foresporselId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/godkjennforesporsel`;

    let params = new HttpParams();
    params = params.append("foresporselId", foresporselId.toString());

    return this.http.get<boolean>(url, { params: params });
  }

  avvisForesporsel(foresporselId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/foresporselombrukertilgang/avvisforesporsel`;

    let params = new HttpParams();
    params = params.append("foresporselId", foresporselId.toString());

    return this.http.get<boolean>(url, { params: params });
  }
}
