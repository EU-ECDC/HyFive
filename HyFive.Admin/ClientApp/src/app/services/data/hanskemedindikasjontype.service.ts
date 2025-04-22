import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class HanskeMedIndikasjonTypeService {
  constructor(private httpClient: HttpClient) { }

  hentHanskeMedIndikasjonTyper(): Observable<GloveWithIndicationType[]> {
    const url = `${environment.apiBaseUrl}/v1/hanskemedindikasjontype`;
    return this.httpClient.get<GloveWithIndicationType[]>(url);
  }

  oppdaterHanskeMedIndikasjonType(hanskeMedIndikasjonType: GloveWithIndicationType): Observable<GloveWithIndicationType> {
    const url = `${environment.apiBaseUrl}/v1/hanskemedindikasjontype/oppdater`;
    return this.httpClient.put<GloveWithIndicationType>(url, hanskeMedIndikasjonType);
  }
}
