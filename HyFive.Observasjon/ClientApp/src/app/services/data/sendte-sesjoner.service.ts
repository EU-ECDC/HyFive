import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { SesjonRapport } from '../../models/api/SesjonRapport';
import { FireIndikasjonerSesjon } from '../../models/api/FireIndikasjonerSesjon';
import { HandsmykkeSesjon } from '../../models/api/HandsmykkeSesjon';
import {BeskyttelsesutstyrSesjon} from '../../models/api/BeskyttelsesutstyrSesjon';
import { HanskeSesjon } from '../../models/api/HanskeSesjon';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SendteSesjonerService {

  constructor(private readonly httpClient: HttpClient) { }

  getSesjoner(): Observable<SesjonRapport[]> {
    const url = `${environment.apiBaseUrl}/v1/sesjon/`;
    return this.httpClient.get<SesjonRapport[]>(url);
  }

  hentFireIndikasjonerSesjon(sesjonId : string): Observable<FireIndikasjonerSesjon> {
    let params = new HttpParams();
    params = params.append("sesjonId", sesjonId);
    return this.httpClient.get<FireIndikasjonerSesjon>(`${environment.apiBaseUrl}/v1/sesjon/fireindikasjoner`, {params});
  }

  hentHandsmykkerSesjon(sesjonId: string): Observable<HandsmykkeSesjon> {
    let params = new HttpParams();
    params = params.append("sesjonId", sesjonId);
    return this.httpClient.get<HandsmykkeSesjon>(`${environment.apiBaseUrl}/v1/sesjon/handsmykker`, {params});
  }

  hentHanskeSesjon(sesjonId: string): Observable<HanskeSesjon> {
    let params = new HttpParams();
    params = params.append("sesjonId", sesjonId);
    return this.httpClient.get<HanskeSesjon>(`${environment.apiBaseUrl}/v1/sesjon/hanske`, { params });
  }

  hentBeskyttelsesutstyrSesjon(sesjonId: string): Observable<BeskyttelsesutstyrSesjon> {
    let params = new HttpParams();
    params = params.append("sesjonId", sesjonId);
    return this.httpClient.get<BeskyttelsesutstyrSesjon>(`${environment.apiBaseUrl}/v1/sesjon/beskyttelsesutstyr`, {params});
  }


  public lastNedHanskeSesjonSomExcel(institusjonId: number, sesjonId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/hanske/mineobservasjoner/excel?institusjonId=${institusjonId}&sesjonId=${sesjonId}`;
    return this.lastNedSesjonSomExcelNy(url, "Hanske");
  }

  public lastNedFireIndikasjonerSesjonSomExcel(institusjonId: number, sesjonId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/fireindikasjoner/mineobservasjoner/excel?institusjonId=${institusjonId}&sesjonId=${sesjonId}`;
    return this.lastNedSesjonSomExcelNy(url, "FireIndikasjoner");
  }

  public lastNedHandsmykkeSesjonSomExcel(institusjonId: number, sesjonId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/handsmykke/mineobservasjoner/excel?institusjonId=${institusjonId}&sesjonId=${sesjonId}`;
    return this.lastNedSesjonSomExcelNy(url, "Handsmykke");
  }

  public lastNedBeskyttelsesutstyrSesjonSomExcel(institusjonId: number, sesjonId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/beskyttelsesutstyr/mineobservasjoner/excel?institusjonId=${institusjonId}&sesjonId=${sesjonId}`;
    return this.lastNedSesjonSomExcelNy(url, "Beskyttelsesutstyr");
  }

  lastNedSesjonSomExcelNy(url: string, observasjonsType: string) : Observable<any> {
    return this.httpClient.get(url, {
      responseType: 'blob' })
      .pipe(map((response: Blob) => this.handleDownloadSuccess(response, observasjonsType)),
    );
  }

  private async handleDownloadSuccess(response: Blob, observasjonsType: string) {

    const downloadLink = document.createElement('a');
    let filename = this.lagExcelFilnavn(observasjonsType);
    downloadLink.href = window.URL.createObjectURL(response);
    downloadLink.setAttribute('download', filename);
    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
  }

  private lagExcelFilnavn(observasjonsType: string): string {
    let dateTime = new Date(),
      maned = ('0' + (dateTime.getMonth() + 1)).slice(-2),
      dag = ('0' + dateTime.getDate()).slice(-2),
      ar = dateTime.getFullYear(),
      timer = ('0' + dateTime.getHours()).slice(-2),
      minutter = ('0' + dateTime.getMinutes()).slice(-2),
      sekunder = ('0' + dateTime.getSeconds()).slice(-2);

    let timestamp = [dag, maned, ar, timer, minutter, sekunder].join('-');
    let filnavn = 'NOST-' + observasjonsType + '-' + timestamp + '.xlsx';

    return filnavn;
  }
}
