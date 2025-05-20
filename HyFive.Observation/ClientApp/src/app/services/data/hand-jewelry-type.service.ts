import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HandJewelryType } from '../../models/api/HandJewelryType';

@Injectable({
  providedIn: 'root'
})
export class HandJewelryTypeService {

  constructor(private http: HttpClient) {
  }

  getHandJewelryTypes(): Observable<HandJewelryType[]> {
    return this.http.get<HandJewelryType[]>('api/v1/handjewelry/getHandJewelryTypes').pipe()
  }

}
