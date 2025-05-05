import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HandJewelryType } from '../../models/api/HandJewelryType';

@Injectable({
  providedIn: 'root'
})
export class HandsmykkeTypeService {

  constructor(private http: HttpClient) {
  }

  getHandsmykkeTyper(): Observable<HandJewelryType[]> {
    return this.http.get<HandJewelryType[]>('api/v1/handsmykke/handJewelryTypes').pipe()
  }

}
