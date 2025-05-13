import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Institution } from '../../models/api/Institution';
import { Localstoragepaths } from '../../constants/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class InstitutionService {

  constructor(private readonly http: HttpClient) { }

  getInstitution(id: number): Observable<Institution> {
    return this.getInstitutions().pipe(map((p: Institution[]) => p.find(x => x.id === id)));
  }

  getSelectedInstitution(): Observable<Institution> {
    let selectedInstitutionId = this.getSelectedInstitutionId();
    return this.getInstitution(selectedInstitutionId);
  }

  getInstitutions(): Observable<Institution[]> {
    const url = `${environment.apiBaseUrl}/v1/institution/`;
    return this.http.get<Institution[]>(url);
  }

  getSelectedInstitutionId(): number | null {
    const selectedInstitutionIdString = localStorage.getItem(Localstoragepaths.SelectedInstitution);
    return selectedInstitutionIdString ? parseInt(selectedInstitutionIdString) : null;
  }

  updateSelectedInstitutionId(institutionId: number): number | null {
    localStorage.setItem(Localstoragepaths.SelectedInstitution, JSON.stringify(institutionId));
    return this.getSelectedInstitutionId();
  }
}
