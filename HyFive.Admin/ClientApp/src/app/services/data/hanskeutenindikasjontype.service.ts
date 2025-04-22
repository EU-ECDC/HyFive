import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { GloveWithoutIndicationType } from '../../models/api/GloveWithoutIndicationType';

@Injectable({
  providedIn: 'root'
})

export class HanskeUtenIndikasjonTypeService {
  constructor(private httpClient: HttpClient) { }

  hentHanskeUtenIndikasjonTyper(): Observable<GloveWithoutIndicationType[]> {
    const url = `${environment.apiBaseUrl}/v1/hanskeutenindikasjontype`;
    return this.httpClient.get<GloveWithoutIndicationType[]>(url);
  }

  oppdaterHanskeUtenIndikasjonType(hanskeUtenIndikasjonType: GloveWithoutIndicationType): Observable<GloveWithoutIndicationType> {
    const url = `${environment.apiBaseUrl}/v1/hanskeUtenindikasjontype/oppdater`;
    return this.httpClient.put<GloveWithoutIndicationType>(url, hanskeUtenIndikasjonType);
  }
}
