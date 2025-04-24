import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { Municipality } from "src/app/models/api/Municipality";

@Injectable({
  providedIn: 'root'
})

export class MunicipalityService {

  constructor(private httpClient: HttpClient) { }

  getMunicipalities(): Observable<Municipality[]> {
    const url = `${environment.apiBaseUrl}/v1/municipality`;
    return this.httpClient.get<Municipality[]>(url);
  }
}
