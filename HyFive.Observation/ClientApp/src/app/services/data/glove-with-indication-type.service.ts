import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';

@Injectable({
  providedIn: 'root'
})
export class GloveWithIndicationTypeService {

  constructor(private http: HttpClient) {
  }

  getGloveWithIndicationTypes(): Observable<GloveWithIndicationType[]> {
    return this.http.get<GloveWithIndicationType[]>('api/v1/glove/gloveindicationtype').pipe()
  }

}
