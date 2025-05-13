import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { PredefinedComment } from '../../models/api/PredefinedComment';
import { environment } from "src/environments/environment";
import { Observable } from 'rxjs';
import { OpprettPredefinertKommentarRequest } from '../../models/api/OpprettPredefinertKommentarRequest';
import { InstitutionService } from './institution.service';

@Injectable({
  providedIn: 'root'
})

export class PredefinedCommentsService {

  constructor(
    private httpClient: HttpClient,
    private institutionService: InstitutionService
  ) { }

  getPredefinedComments(): Observable<PredefinedComment[]> {
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    const url = `${environment.apiBaseUrl}/v1/predefinedcomment?institutionId=${selectedInstitutionId}`;
    return this.httpClient.get<PredefinedComment[]>(url);
  }

  updatePredefinedComment(predefinedComment: PredefinedComment): Observable<boolean>{
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    const url = `${environment.apiBaseUrl}/v1/predefinedcomment/${selectedInstitutionId}/update`;
    return this.httpClient.put<boolean>(url, predefinedComment);
  }

  createPredefinedComment(newPredefinedComment: OpprettPredefinertKommentarRequest): Observable<boolean> {
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    const url = `${environment.apiBaseUrl}/v1/predefinedcomment/${selectedInstitutionId}/create`;
    return this.httpClient.post<boolean>(url, newPredefinedComment);
  }
}
