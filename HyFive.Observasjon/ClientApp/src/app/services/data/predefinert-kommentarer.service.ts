import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { tap } from "rxjs/operators";
import { SesjonType } from '../../models/api/SesjonType';

@Injectable({
  providedIn: 'root'
})

export class PredefinertKommentarerService {

  constructor(private httpClient: HttpClient){  }

  hentPredefinertKommentarer(institusjonid: number, sesjontype: SesjonType): Observable<string[]>{
    const url = `${environment.apiBaseUrl}/v1/institusjon/predefinertekommentarer`;
    let params = new HttpParams();
    if(institusjonid)    {
      params = params.append("institusjonid", institusjonid.toString());
    }
    if(sesjontype){
      params = params.append("sesjontype", sesjontype.toString());
    }
    return this.httpClient.get<string[]>(url, {params});
  }
}
