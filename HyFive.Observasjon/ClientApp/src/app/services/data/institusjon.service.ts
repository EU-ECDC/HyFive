import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Institusjon } from '../../models/api/Institusjon';
import { Localstoragepaths } from '../../konstanter/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class InstitusjonService {

  constructor(private readonly http: HttpClient) { }

  getInstitusjon(id: number): Observable<Institusjon> {
    return this.getInstitusjoner().pipe(map((p: Institusjon[]) => p.find(x => x.id === id)));
  }

  getValgtInstitusjon(): Observable<Institusjon> {
    let valgtInstitusjonId = this.hentValgtInstitusjonId();
    return this.getInstitusjon(valgtInstitusjonId);
  }

  getInstitusjoner(): Observable<Institusjon[]> {
    const url = `${environment.apiBaseUrl}/v1/institusjon/`;
    return this.http.get<Institusjon[]>(url);
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
