import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IndikasjonType} from '../../models/api/IndikasjonType';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class IndikasjonService {

  constructor(private http: HttpClient) {
  }
  getIndikasjonstyper(): Observable<IndikasjonType[]> {
    return this.http.get<IndikasjonType[]>('api/v1/fireindikasjoner/indikasjonstyper').pipe()
  }

}
