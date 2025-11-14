import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PostGloveHandHygieneType } from '../../models/api/PostGloveHandHygieneType';

@Injectable({
  providedIn: 'root'
})
export class PostGloveHandHygieneTypeService {

  constructor(private readonly http: HttpClient) {
  }

  getPostGloveHandHygieneTypes(): Observable<PostGloveHandHygieneType[]> {
    return this.http.get<PostGloveHandHygieneType[]>('api/v1/glove/handhygieneaftergloveusetype').pipe()
  }

}
