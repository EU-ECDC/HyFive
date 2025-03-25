import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HanskeMedIndikasjonType } from '../../models/api/HanskeMedIndikasjonType';

@Injectable({
  providedIn: 'root'
})
export class HanskeMedIndikasjonTypeService {

  constructor(private http: HttpClient) {
  }

  getHanskeMedIndikasjonTyper(): Observable<HanskeMedIndikasjonType[]> {
    return this.http.get<HanskeMedIndikasjonType[]>('api/v1/hanske/hanskemedindikasjontype').pipe()
  }

}
