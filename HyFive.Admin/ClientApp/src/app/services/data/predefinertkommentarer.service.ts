import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { PredefinertKommentar } from '../../models/api/PredefinertKommentar';
import { environment } from "src/environments/environment";
import { Observable } from 'rxjs';
import { OpprettPredefinertKommentarRequest } from '../../models/api/OpprettPredefinertKommentarRequest';
import { InstitusjonService } from './institusjon.service';

@Injectable({
  providedIn: 'root'
})

export class PredefinertKommentarerService {

  constructor(
    private httpClient: HttpClient,
    private institusjonService: InstitusjonService
  ) { }

  hentPredefinertKommentarer(): Observable<PredefinertKommentar[]> {
    let valgtInstitusjonId = this.institusjonService.hentValgtInstitusjonId();
    const url = `${environment.apiBaseUrl}/v1/predefinertkommentar?institusjonId=${valgtInstitusjonId}`;
    return this.httpClient.get<PredefinertKommentar[]>(url);
  }

  oppdaterPredefinertKommentar(predefinertKommentar: PredefinertKommentar): Observable<boolean>{
    let valgtInstitusjonId = this.institusjonService.hentValgtInstitusjonId();
    const url = `${environment.apiBaseUrl}/v1/predefinertkommentar/${valgtInstitusjonId}/oppdater`;
    return this.httpClient.put<boolean>(url, predefinertKommentar);
  }

  opprettPredefinertKommentar(nypredefinertKommentar: OpprettPredefinertKommentarRequest): Observable<boolean> {
    let valgtInstitusjonId = this.institusjonService.hentValgtInstitusjonId();
    const url = `${environment.apiBaseUrl}/v1/predefinertkommentar/${valgtInstitusjonId}/opprett`;
    return this.httpClient.post<boolean>(url, nypredefinertKommentar);
  }
}
