import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { HandHygieneAfterGloveUseType } from '../../models/api/HandHygieneAfterGloveUseType';

@Injectable({
  providedIn: 'root'
})

export class HandHygieneAfterGloveUseTypeService {
  constructor(private readonly httpClient: HttpClient) { }

  getHandHygieneAfterGloveUseTypes(): Observable<HandHygieneAfterGloveUseType[]> {
    const url = `${environment.apiBaseUrl}/v1/handHygieneAfterGloveUseTypes`;
    return this.httpClient.get<HandHygieneAfterGloveUseType[]>(url);
  }

  updateHandHygieneAfterGloveUseType(handHygieneAfterGloveuseType: HandHygieneAfterGloveUseType): Observable<HandHygieneAfterGloveUseType> {
    const url = `${environment.apiBaseUrl}/v1/handHygieneAfterGloveUseTypes/update`;
    return this.httpClient.put<HandHygieneAfterGloveUseType>(url, handHygieneAfterGloveuseType);
  }
}
