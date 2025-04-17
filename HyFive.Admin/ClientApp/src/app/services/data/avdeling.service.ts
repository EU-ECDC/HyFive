import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../../environments/environment';
import {Department} from '../../models/api/Department';
import {OpprettAvdelingRequest} from '../../models/api/OpprettAvdelingRequest';
import {Role} from "../../models/api/Role";
import {AvdelingType} from "../../models/api/AvdelingType";


@Injectable({
  providedIn: 'root'
})
export class AvdelingService {

  constructor(private readonly http: HttpClient) { }

  hentAvdeling(id: number): Observable<Department> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/${id}`;
    return this.http.get<Department>(url);
  }

  opprettAvdeling(avdeling: OpprettAvdelingRequest): Observable<Department>{
    const url = `${environment.apiBaseUrl}/v1/avdeling/opprett`;
    return this.http.post<Department>(url, avdeling);
  }

  oppdaterAvdeling(avdeling: Department): Observable<Department> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/oppdater`;
    return this.http.put<Department>(url, avdeling);
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

  hentRoller(id: number): Observable<Role[]> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/${id}/roles`;
    return this.http.get<Role[]>(url);
  }

  slettAvdeling(id: number): Observable<void> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/slett/${id}`;
    return this.http.delete<void>(url);
  }

  hasTransferredSessionToFHI(id: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/avdeling/hasTransferredSessionToFHI/${id}`;
    return this.http.get<boolean>(url);
  }
}
