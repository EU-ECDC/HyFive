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
  constructor(private readonly httpClient: HttpClient) { }

  getGloveWithIndicationTypes(): Observable<GloveWithIndicationType[]> {
    const url = `${environment.apiBaseUrl}/v1/gloveWithIndicationType`;
    return this.httpClient.get<GloveWithIndicationType[]>(url);
  }

  updateGloveWithIndicationType(gloveWithIndicationType: GloveWithIndicationType): Observable<GloveWithIndicationType> {
    const url = `${environment.apiBaseUrl}/v1/gloveWithIndicationType/update`;
    return this.httpClient.put<GloveWithIndicationType>(url, gloveWithIndicationType);
  }
}
