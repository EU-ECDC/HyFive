import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { environment } from "src/environments/environment";
import { Observable } from 'rxjs';
import { CreatePredefinedCommentRequest } from '../../models/api/CreatePredefinedCommentRequest';
import { FacilityService } from './facility.service';
import { PredefinedComment } from "src/app/models/api/PredefinedComment";

@Injectable({
  providedIn: 'root'
})

export class PredefinedCommentsService {

  constructor(
    private readonly httpClient: HttpClient,
    private readonly facilityService: FacilityService
  ) { }

  getPredefinedComments(): Observable<PredefinedComment[]> {
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    const url = `${environment.apiBaseUrl}/v1/predefinedComment?facilityId=${selectedFacilityId}`;
    return this.httpClient.get<PredefinedComment[]>(url);
  }

  updatePredefinedComment(predefinedComment: PredefinedComment): Observable<boolean>{
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    const url = `${environment.apiBaseUrl}/v1/predefinedComment/${selectedFacilityId}/update`;
    return this.httpClient.put<boolean>(url, predefinedComment);
  }

  createPredefinedComment(newPredefinedComment: CreatePredefinedCommentRequest): Observable<boolean> {
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    const url = `${environment.apiBaseUrl}/v1/predefinedComment/${selectedFacilityId}/create`;
    return this.httpClient.post<boolean>(url, newPredefinedComment);
  }
}
