import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Institution } from '../../models/api/Institution';
import { Localstoragepaths } from '../../konstanter/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class InstitusjonService {

  constructor(private readonly http: HttpClient) { }

  getInstitusjon(id: number): Observable<Institution> {
    return this.getInstitusjoner().pipe(map((p: Institution[]) => p.find(x => x.id === id)));
  }

  getValgtInstitusjon(): Observable<Institution> {
    let valgtInstitusjonId = this.hentValgtInstitusjonId();
    return this.getInstitusjon(valgtInstitusjonId);
  }

  getInstitusjoner(): Observable<Institution[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/`;
    return this.http.get<Institution[]>(url);
  }

  hentValgtInstitusjonId(): number | null {
    const valgtInstitusjonIdString = localStorage.getItem(Localstoragepaths.ValgtInstitusjon);
    return valgtInstitusjonIdString ? parseInt(valgtInstitusjonIdString) : null;
  }

  oppdaterValgtInstitusjonId(institusjonId: number): number | null {
    localStorage.setItem(Localstoragepaths.ValgtInstitusjon, JSON.stringify(institusjonId));
    return this.hentValgtInstitusjonId();
  }
}
