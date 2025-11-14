import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IndicationType} from '../../models/api/IndicationType';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IndicationService {

  constructor(private readonly http: HttpClient) {
  }
  getIndicationTypes(): Observable<IndicationType[]> {
    return this.http.get<IndicationType[]>('api/v1/fiveindications/indicationTypes').pipe()
  }

}
