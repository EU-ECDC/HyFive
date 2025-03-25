import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HanskeUtenIndikasjonType } from '../../models/api/HanskeUtenIndikasjonType';

@Injectable({
  providedIn: 'root'
})
export class HanskeUtenIndikasjonTypeService {

  constructor(private http: HttpClient) {
  }

  getHanskeUtenIndikasjonTyper(): Observable<HanskeUtenIndikasjonType[]> {
    return this.http.get<HanskeUtenIndikasjonType[]>('api/v1/hanske/hanskeutenindikasjontype').pipe()
  }

}
