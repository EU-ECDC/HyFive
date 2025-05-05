import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IndicationType} from '../../models/api/IndicationType';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IndikasjonService {

  constructor(private http: HttpClient) {
  }
  getIndikasjonstyper(): Observable<IndicationType[]> {
    return this.http.get<IndicationType[]>('api/v1/fourindications/indicationTypes').pipe()
  }

}
