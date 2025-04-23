import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { BraceletType } from '../../models/api/BraceletType';


@Injectable({
  providedIn: 'root'
})
export class HandJewelryTypeService {

  constructor(private readonly http: HttpClient) { }

  getHandJewelryTypes(): Observable<BraceletType[]> {
    const url = `${environment.apiBaseUrl}/v1/handJewelryType/`;
    return this.http.get<BraceletType[]>(url);
  }

  updateHandJewelryType(handJewelryType: BraceletType): Observable<BraceletType> {
    const url = `${environment.apiBaseUrl}/v1/handJewelryType/update`;
    return this.http.put<BraceletType>(url, handJewelryType);
  }
}
