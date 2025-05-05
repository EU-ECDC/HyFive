import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { SesjonRapport } from '../../models/api/SesjonRapport';
import { FourIndicationsSession } from '../../models/api/FourIndicationsSession';
import { HandJewelrySession } from '../../models/api/HandJewelrySession';
import {ProtectiveEquipmentSession} from '../../models/api/ProtectiveEquipmentSession';
import { GloveSession } from '../../models/api/GloveSession';
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

  hentFireIndikasjonerSesjon(sessionId : string): Observable<FourIndicationsSession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<FourIndicationsSession>(`${environment.apiBaseUrl}/v1/sesjon/fireindikasjoner`, {params});
  }

  hentHandsmykkerSesjon(sessionId: string): Observable<HandJewelrySession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<HandJewelrySession>(`${environment.apiBaseUrl}/v1/sesjon/handJewelry`, {params});
  }

  hentHanskeSesjon(sessionId: string): Observable<GloveSession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<GloveSession>(`${environment.apiBaseUrl}/v1/sesjon/hanske`, { params });
  }

  hentBeskyttelsesutstyrSesjon(sessionId: string): Observable<ProtectiveEquipmentSession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<ProtectiveEquipmentSession>(`${environment.apiBaseUrl}/v1/sesjon/protectiveEquipment`, {params});
  }


  public lastNedHanskeSesjonSomExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/hanske/mineobservasjoner/excel?institutionId=${institutionId}&sessionId=${sessionId}`;
    return this.lastNedSesjonSomExcelNy(url, "Hanske");
  }

  public lastNedFireIndikasjonerSesjonSomExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/fireindikasjoner/mineobservasjoner/excel?institutionId=${institutionId}&sessionId=${sessionId}`;
    return this.lastNedSesjonSomExcelNy(url, "FireIndikasjoner");
  }

  public lastNedHandsmykkeSesjonSomExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/handsmykke/mineobservasjoner/excel?institutionId=${institutionId}&sessionId=${sessionId}`;
    return this.lastNedSesjonSomExcelNy(url, "Handsmykke");
  }

  public lastNedBeskyttelsesutstyrSesjonSomExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/protectiveEquipment/mineobservasjoner/excel?institutionId=${institutionId}&sessionId=${sessionId}`;
    return this.lastNedSesjonSomExcelNy(url, "ProtectiveEquipment");
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
