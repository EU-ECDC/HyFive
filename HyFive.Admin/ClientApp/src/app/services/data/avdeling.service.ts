import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../../environments/environment';
import {Avdeling} from '../../models/api/Avdeling';
import {OpprettAvdelingRequest} from '../../models/api/OpprettAvdelingRequest';
import {Rolle} from "../../models/api/Rolle";
import {AvdelingType} from "../../models/api/AvdelingType";


@Injectable({
  providedIn: 'root'
})
export class AvdelingService {

  constructor(private readonly http: HttpClient) { }

  hentAvdeling(id: number): Observable<Avdeling> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/${id}`;
    return this.http.get<Avdeling>(url);
  }

  opprettAvdeling(avdeling: OpprettAvdelingRequest): Observable<Avdeling>{
    const url = `${environment.apiBaseUrl}/v1/avdeling/opprett`;
    return this.http.post<Avdeling>(url, avdeling);
  }

  oppdaterAvdeling(avdeling: Avdeling): Observable<Avdeling> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/oppdater`;
    return this.http.put<Avdeling>(url, avdeling);
  }

  hentAvdelingstyper(): Observable<AvdelingType[]>{
    const url = `${environment.apiBaseUrl}/v1/avdeling/avdelingstyper/`;
    return this.http.get<AvdelingType[]>(url);
  }

  opprettAvdelingType(avdelingstype: AvdelingType): Observable<AvdelingType>{
    const url = `${environment.apiBaseUrl}/v1/avdeling/avdelingstyper/opprett`;
    return this.http.post<AvdelingType>(url, avdelingstype);
  }

  oppdaterAvdelingType(avdelingstype: AvdelingType): Observable<AvdelingType> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/avdelingstyper/oppdater`;
    return this.http.put<AvdelingType>(url, avdelingstype);
  }

  hentRoller(id: number): Observable<Rolle[]> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/${id}/roller`;
    return this.http.get<Rolle[]>(url);
  }

  slettAvdeling(id: number): Observable<void> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/slett/${id}`;
    return this.http.delete<void>(url);
  }

  harOverfortSesjonTilFHI(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/harOverfortSesjonTilFHI/${id}`;
    return this.http.get<boolean>(url);
  }
}
