import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HandHygieneAfterGloveUseType } from '../../models/api/HandHygieneAfterGloveUseType';

@Injectable({
  providedIn: 'root'
})
export class HandHygieneAfterGloveUseTypeService {

  constructor(private http: HttpClient) {
  }

  getHandhygieneAfterGloveUseTypes(): Observable<HandHygieneAfterGloveUseType[]> {
    return this.http.get<HandHygieneAfterGloveUseType[]>('api/v1/glove/handhygieneaftergloveusetype').pipe()
  }

}
