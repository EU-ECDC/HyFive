import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { SessionReport } from '../../models/api/SessionReport';
import { FiveIndicationsSession } from '../../models/api/FiveIndicationsSession';
import { HandJewelrySession } from '../../models/api/HandJewelrySession';
import {ProtectiveEquipmentSession} from '../../models/api/ProtectiveEquipmentSession';
import { GloveSession } from '../../models/api/GloveSession';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class SentSessionsService {

  constructor(private readonly httpClient: HttpClient) { }

  getSessions(): Observable<SessionReport[]> {
    const url = `${environment.apiBaseUrl}/v1/session/`;
    return this.httpClient.get<SessionReport[]>(url);
  }

  getFiveIndicationsSession(sessionId : string): Observable<FiveIndicationsSession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<FiveIndicationsSession>(`${environment.apiBaseUrl}/v1/session/fiveIndications`, {params});
  }

  getHandJewelrySession(sessionId: string): Observable<HandJewelrySession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<HandJewelrySession>(`${environment.apiBaseUrl}/v1/session/handJewelry`, {params});
  }

  getGloveSession(sessionId: string): Observable<GloveSession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<GloveSession>(`${environment.apiBaseUrl}/v1/session/glove`, { params });
  }

  getProtectiveEquipmentSession(sessionId: string): Observable<ProtectiveEquipmentSession> {
    let params = new HttpParams();
    params = params.append("sessionId", sessionId);
    return this.httpClient.get<ProtectiveEquipmentSession>(`${environment.apiBaseUrl}/v1/session/protectiveEquipment`, {params});
  }


  public downloadGloveSessionAsExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/glove/myobservations/excel?institutionid=${institutionId}&sessionid=${sessionId}`;
    return this.downloadSessionAsExcelNew(url, "Glove");
  }

  public downloadFiveIndicationsSessionAsExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/fiveindications/myobservations/excel?institutionid=${institutionId}&sessionid=${sessionId}`;
    return this.downloadSessionAsExcelNew(url, "FiveIndications");
  }

  public downloadHandJewelrySessionAsExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/handjewelry/myobservations/excel?institutionid=${institutionId}&sessionid=${sessionId}`;
    return this.downloadSessionAsExcelNew(url, "Handjewelry");
  }

  public DownloadProtectiveEquipmentSessionAsExcel(institutionId: number, sessionId: string) : Observable<any> {
    const url = `${environment.apiBaseUrl}/v1/protectiveequipment/myobservations/excel?institutionid=${institutionId}&sessionid=${sessionId}`;
    return this.downloadSessionAsExcelNew(url, "ProtectiveEquipment");
  }

  downloadSessionAsExcelNew(url: string, observationType: string) : Observable<any> {
    return this.httpClient.get(url, {
      responseType: 'blob' })
      .pipe(map((response: Blob) => this.handleDownloadSuccess(response, observationType)),
    );
  }

  private async handleDownloadSuccess(response: Blob, observationType: string) {

    const downloadLink = document.createElement('a');
    let filename = this.saveExcelFilename(observationType);
    downloadLink.href = window.URL.createObjectURL(response);
    downloadLink.setAttribute('download', filename);
    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
  }

  private saveExcelFilename(observationType: string): string {
    let dateTime = new Date(),
      month = ('0' + (dateTime.getMonth() + 1)).slice(-2),
      day = ('0' + dateTime.getDate()).slice(-2),
      year = dateTime.getFullYear(),
      hours = ('0' + dateTime.getHours()).slice(-2),
      minutes = ('0' + dateTime.getMinutes()).slice(-2),
      seconds = ('0' + dateTime.getSeconds()).slice(-2);

    let timestamp = [day, month, year, hours, minutes, seconds].join('-');
    let filename = 'ECDC-' + observationType + '-' + timestamp + '.xlsx';

    return filename;
  }
}
