import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HandhygieneEtterHanskebrukType } from '../../models/api/HandhygieneEtterHanskebrukType';

@Injectable({
  providedIn: 'root'
})
export class HandhygieneEtterHanskebrukTypeService {

  constructor(private http: HttpClient) {
  }

  getHandhygieneEtterHanskebrukTyper(): Observable<HandhygieneEtterHanskebrukType[]> {
    return this.http.get<HandhygieneEtterHanskebrukType[]>('api/v1/hanske/handhygieneetterhanskebruktype').pipe()
  }

}
