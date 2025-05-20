import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivityType } from '../../models/api/ActivityType';

@Injectable({
  providedIn: 'root'
})
export class ActivityService {

  constructor(private http: HttpClient) {
  }

  getActivityTypes(): Observable<ActivityType[]> {
    return this.http.get<ActivityType[]>('api/v1/fourindications/activitytypes').pipe()
  }

}
