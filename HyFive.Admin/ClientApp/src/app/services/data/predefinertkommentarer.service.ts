import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { PredefinertKommentar } from '../../models/api/PredefinertKommentar';
import { environment } from "src/environments/environment";
import { Observable } from 'rxjs';
import { OpprettPredefinertKommentarRequest } from '../../models/api/OpprettPredefinertKommentarRequest';
import { InstitutionService } from './institution.service';

@Injectable({
  providedIn: 'root'
})

export class PredefinertKommentarerService {

  constructor(
    private httpClient: HttpClient,
    private institutionService: InstitutionService
  ) { }

  hentPredefinertKommentarer(): Observable<PredefinertKommentar[]> {
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    const url = `${environment.apiBaseUrl}/v1/predefinertkommentar?institutionId=${selectedInstitutionId}`;
    return this.httpClient.get<PredefinertKommentar[]>(url);
  }

  oppdaterPredefinertKommentar(predefinertKommentar: PredefinertKommentar): Observable<boolean>{
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    const url = `${environment.apiBaseUrl}/v1/predefinertkommentar/${selectedInstitutionId}/oppdater`;
    return this.httpClient.put<boolean>(url, predefinertKommentar);
  }

  opprettPredefinertKommentar(nypredefinertKommentar: OpprettPredefinertKommentarRequest): Observable<boolean> {
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    const url = `${environment.apiBaseUrl}/v1/predefinertkommentar/${selectedInstitutionId}/opprett`;
    return this.httpClient.post<boolean>(url, nypredefinertKommentar);
  }
}
