import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GloveWithoutIndicationType } from '../../models/api/GloveWithoutIndicationType';

@Injectable({
  providedIn: 'root'
})
export class GloveWithoutIndicationTypeService {

  constructor(private readonly http: HttpClient) {
  }

  getGloveWithoutIndicationTypes(): Observable<GloveWithoutIndicationType[]> {
    return this.http.get<GloveWithoutIndicationType[]>('api/v1/glove/glovewithoutindicationtype').pipe()
  }

}
