import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { SessionType } from '../../models/api/SessionType';

@Injectable({
  providedIn: 'root'
})

export class PredefinedCommentsService {

  constructor(private readonly httpClient: HttpClient){  }

  getPredefinedComments(facilityid: number, sessiontype: SessionType): Observable<string[]>{
    const url = `${environment.apiBaseUrl}/v1/facility/predefinedcomments`;
    let params = new HttpParams();
    if(facilityid)    {
      params = params.append("facilityid", facilityid.toString());
    }
    if(sessiontype){
      params = params.append("sessiontype", sessiontype.toString());
    }
    return this.httpClient.get<string[]>(url, {params});
  }
}
