import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class GloveWithIndicationTypeService
 {
  constructor(private httpClient: HttpClient) { }

  getGloveWithIndicationTypes(): Observable<GloveWithIndicationType[]> {
    const url = `${environment.apiBaseUrl}/v1/gloveindicationtype`;
    return this.httpClient.get<GloveWithIndicationType[]>(url);
  }

  updateGloveWithIndicationType(gloveWithIndicationType: GloveWithIndicationType): Observable<GloveWithIndicationType> {
    const url = `${environment.apiBaseUrl}/v1/gloveindicationtype/update`;
    return this.httpClient.put<GloveWithIndicationType>(url, gloveWithIndicationType);
  }
}
