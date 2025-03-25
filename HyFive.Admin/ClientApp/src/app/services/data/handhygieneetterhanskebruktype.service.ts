import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { HandhygieneEtterHanskebrukType } from '../../models/api/HandhygieneEtterHanskebrukType';

@Injectable({
  providedIn: 'root'
})

export class HandhygieneEtterHanskebrukTypeService {
  constructor(private httpClient: HttpClient) { }

  hentHandhygieneEtterHanskebrukTyper(): Observable<HandhygieneEtterHanskebrukType[]> {
    const url = `${environment.apiBaseUrl}/v1/handhygieneetterhanskebruktype`;
    return this.httpClient.get<HandhygieneEtterHanskebrukType[]>(url);
  }

  oppdaterHandhygieneEtterHanskebrukType(handhygieneEtterHanskebrukType: HandhygieneEtterHanskebrukType): Observable<HandhygieneEtterHanskebrukType> {
    const url = `${environment.apiBaseUrl}/v1/handhygieneetterhanskebruktype/oppdater`;
    return this.httpClient.put<HandhygieneEtterHanskebrukType>(url, handhygieneEtterHanskebrukType);
  }
}
