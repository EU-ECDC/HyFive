import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { HandHygieneAfterGloveUseType } from '../../models/api/HandHygieneAfterGloveUseType';

@Injectable({
  providedIn: 'root'
})

export class HandhygieneEtterHanskebrukTypeService {
  constructor(private httpClient: HttpClient) { }

  hentHandhygieneEtterHanskebrukTyper(): Observable<HandHygieneAfterGloveUseType[]> {
    const url = `${environment.apiBaseUrl}/v1/handhygieneetterhanskebruktype`;
    return this.httpClient.get<HandHygieneAfterGloveUseType[]>(url);
  }

  oppdaterHandhygieneEtterHanskebrukType(handhygieneEtterHanskebrukType: HandHygieneAfterGloveUseType): Observable<HandHygieneAfterGloveUseType> {
    const url = `${environment.apiBaseUrl}/v1/handhygieneetterhanskebruktype/oppdater`;
    return this.httpClient.put<HandHygieneAfterGloveUseType>(url, handhygieneEtterHanskebrukType);
  }
}
