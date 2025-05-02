import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GloveWithoutIndicationType } from '../../models/api/GloveWithoutIndicationType';

@Injectable({
  providedIn: 'root'
})
export class HanskeUtenIndikasjonTypeService {

  constructor(private http: HttpClient) {
  }

  getHanskeUtenIndikasjonTyper(): Observable<GloveWithoutIndicationType[]> {
    return this.http.get<GloveWithoutIndicationType[]>('api/v1/hanske/hanskeutenindikasjontype').pipe()
  }

}
