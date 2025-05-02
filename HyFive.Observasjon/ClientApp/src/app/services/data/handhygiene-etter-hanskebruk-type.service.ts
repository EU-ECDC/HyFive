import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HandHygieneAfterGloveUseType } from '../../models/api/HandHygieneAfterGloveUseType';

@Injectable({
  providedIn: 'root'
})
export class HandhygieneEtterHanskebrukTypeService {

  constructor(private http: HttpClient) {
  }

  getHandhygieneEtterHanskebrukTyper(): Observable<HandHygieneAfterGloveUseType[]> {
    return this.http.get<HandHygieneAfterGloveUseType[]>('api/v1/hanske/handhygieneetterhanskebruktype').pipe()
  }

}
