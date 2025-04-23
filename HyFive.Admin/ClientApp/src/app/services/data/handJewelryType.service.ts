import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { HandJewelryType } from '../../models/api/HandJewelryType';


@Injectable({
  providedIn: 'root'
})
export class HandJewelryTypeService {

  constructor(private readonly http: HttpClient) { }

  getHandJewelryTypes(): Observable<HandJewelryType[]> {
    const url = `${environment.apiBaseUrl}/v1/handJewelryType/`;
    return this.http.get<HandJewelryType[]>(url);
  }

  updateHandJewelryType(handJewelryType: HandJewelryType): Observable<HandJewelryType> {
    const url = `${environment.apiBaseUrl}/v1/handJewelryType/update`;
    return this.http.put<HandJewelryType>(url, handJewelryType);
  }
}
