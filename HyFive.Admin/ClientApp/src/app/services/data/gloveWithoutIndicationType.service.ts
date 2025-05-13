import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { GloveWithoutIndicationType } from '../../models/api/GloveWithoutIndicationType';

@Injectable({
  providedIn: 'root'
})

export class GloveWithoutIndicationTypeService {
  constructor(private httpClient: HttpClient) { }

  getGloveWithoutIndicationTypes(): Observable<GloveWithoutIndicationType[]> {
    const url = `${environment.apiBaseUrl}/v1/glovewithoutindicationtype`;
    return this.httpClient.get<GloveWithoutIndicationType[]>(url);
  }

  updateGloveWithoutIndicationType(gloveWithoutIndicationType: GloveWithoutIndicationType): Observable<GloveWithoutIndicationType> {
    const url = `${environment.apiBaseUrl}/v1/glovewithoutindicationtype/update`;
    return this.httpClient.put<GloveWithoutIndicationType>(url, gloveWithoutIndicationType);
  }
}
