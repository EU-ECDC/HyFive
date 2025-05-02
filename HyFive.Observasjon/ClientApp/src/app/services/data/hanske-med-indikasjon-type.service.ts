import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';

@Injectable({
  providedIn: 'root'
})
export class HanskeMedIndikasjonTypeService {

  constructor(private http: HttpClient) {
  }

  getHanskeMedIndikasjonTyper(): Observable<GloveWithIndicationType[]> {
    return this.http.get<GloveWithIndicationType[]>('api/v1/hanske/hanskemedindikasjontype').pipe()
  }

}
