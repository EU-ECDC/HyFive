import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { HanskeUtenIndikasjonType } from '../../models/api/HanskeUtenIndikasjonType';

@Injectable({
  providedIn: 'root'
})

export class HanskeUtenIndikasjonTypeService {
  constructor(private httpClient: HttpClient) { }

  hentHanskeUtenIndikasjonTyper(): Observable<HanskeUtenIndikasjonType[]> {
    const url = `${environment.apiBaseUrl}/v1/hanskeutenindikasjontype`;
    return this.httpClient.get<HanskeUtenIndikasjonType[]>(url);
  }

  oppdaterHanskeUtenIndikasjonType(hanskeUtenIndikasjonType: HanskeUtenIndikasjonType): Observable<HanskeUtenIndikasjonType> {
    const url = `${environment.apiBaseUrl}/v1/hanskeUtenindikasjontype/oppdater`;
    return this.httpClient.put<HanskeUtenIndikasjonType>(url, hanskeUtenIndikasjonType);
  }
}
