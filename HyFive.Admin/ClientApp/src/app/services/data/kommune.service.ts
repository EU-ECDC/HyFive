import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { Municipality } from "src/app/models/api/Municipality";

@Injectable({
  providedIn: 'root'
})

export class KommuneService {

  constructor(private httpClient: HttpClient) { }

  hentKommuner(): Observable<Municipality[]> {
    const url = `${environment.apiBaseUrl}/v1/kommune`;
    return this.httpClient.get<Municipality[]>(url);
  }
}
