import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IndicationType } from '../../models/api/IndicationType';
import { environment } from 'src/environments/environment';
import { InstitutionType } from '../../models/api/InstitutionType';
import { CreateInstitutionTypeRequest } from '../../models/api/CreateInstitutionTypeRequest';

@Injectable({
  providedIn: 'root'
})

export class InstitutionTypesService {

  constructor(private httpClient: HttpClient) {

  }

  getInstitutionTypes(): Observable<InstitutionType[]> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes`;
    return this.httpClient.get<IndicationType[]>(url)
    .pipe();
  }

  updateInstitutionType(institutiontype: InstitutionType): Observable<InstitutionType> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes/update`;
    return this.httpClient.put<InstitutionType>(url, institutiontype)
    .pipe();
  }

  createInstitutionType(institutiontype: CreateInstitutionTypeRequest): Observable<InstitutionType> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes/create`;
    return this.httpClient.post<InstitutionType>(url, institutiontype)
    .pipe();
  }

  deleteInstitutionType(institutiontypeId: number): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/institutiontypes/delete?institutiontypeId=${institutiontypeId}`;
    return this.httpClient.delete<boolean>(url)
    .pipe();
  }
}
