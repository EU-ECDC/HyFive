import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ActivityType } from '../../models/api/ActivityType';

@Injectable({
  providedIn: 'root'
})
export class ActivityTypeService {

  constructor(private readonly http: HttpClient) { }

  getActivityTypes(): Observable<ActivityType[]> {
    const url = `${environment.apiBaseUrl}/v1/activitytype/`;
    return this.http.get<ActivityType[]>(url);
  }

  updateActivityType(activitytype: ActivityType): Observable<ActivityType> {
    const url = `${environment.apiBaseUrl}/v1/activitytype/update`;
    return this.http.put<ActivityType>(url, activitytype);
  }
}
