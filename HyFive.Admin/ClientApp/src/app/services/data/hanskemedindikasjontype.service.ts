import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HanskeMedIndikasjonType } from '../../models/api/HanskeMedIndikasjonType';
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class HanskeMedIndikasjonTypeService {
  constructor(private httpClient: HttpClient) { }

  hentHanskeMedIndikasjonTyper(): Observable<HanskeMedIndikasjonType[]> {
    const url = `${environment.apiBaseUrl}/v1/hanskemedindikasjontype`;
    return this.httpClient.get<HanskeMedIndikasjonType[]>(url);
  }

  oppdaterHanskeMedIndikasjonType(hanskeMedIndikasjonType: HanskeMedIndikasjonType): Observable<HanskeMedIndikasjonType> {
    const url = `${environment.apiBaseUrl}/v1/hanskemedindikasjontype/oppdater`;
    return this.httpClient.put<HanskeMedIndikasjonType>(url, hanskeMedIndikasjonType);
  }
}
