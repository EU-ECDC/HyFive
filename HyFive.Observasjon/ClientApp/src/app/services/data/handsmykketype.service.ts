import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HandsmykkeType } from '../../models/api/HandsmykkeType';

@Injectable({
  providedIn: 'root'
})
export class HandsmykkeTypeService {

  constructor(private http: HttpClient) {
  }

  getHandsmykkeTyper(): Observable<HandsmykkeType[]> {
    return this.http.get<HandsmykkeType[]>('api/v1/handsmykke/handsmykketyper').pipe()
  }

}
