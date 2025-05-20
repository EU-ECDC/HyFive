import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { tap } from "rxjs/operators";
import { SessionType } from '../../models/api/SessionType';

@Injectable({
  providedIn: 'root'
})

export class PredefinedCommentsService {

  constructor(private httpClient: HttpClient){  }

  getPredefinedComments(institutionid: number, sessiontype: SessionType): Observable<string[]>{
    const url = `${environment.apiBaseUrl}/v1/institution/predefinedcomments`;
    let params = new HttpParams();
    if(institutionid)    {
      params = params.append("institutionid", institutionid.toString());
    }
    if(sessiontype){
      params = params.append("sessiontype", sessiontype.toString());
    }
    return this.httpClient.get<string[]>(url, {params});
  }
}
