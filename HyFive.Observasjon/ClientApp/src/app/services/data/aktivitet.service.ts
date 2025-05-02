import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivityType } from '../../models/api/ActivityType';

@Injectable({
  providedIn: 'root'
})
export class AktivitetService {

  constructor(private http: HttpClient) {
  }

  getAktivitetTyper(): Observable<ActivityType[]> {
    return this.http.get<ActivityType[]>('api/v1/fireindikasjoner/aktivitettyper').pipe()
  }

}
