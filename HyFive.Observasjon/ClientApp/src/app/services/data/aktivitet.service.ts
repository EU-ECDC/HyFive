import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AktivitetType } from '../../models/api/AktivitetType';

@Injectable({
  providedIn: 'root'
})
export class AktivitetService {

  constructor(private http: HttpClient) {
  }

  getAktivitetTyper(): Observable<AktivitetType[]> {
    return this.http.get<AktivitetType[]>('api/v1/fireindikasjoner/aktivitettyper').pipe()
  }

}
