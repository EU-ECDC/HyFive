import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "src/environments/environment";
import { Kommune } from "src/app/models/api/Kommune";

@Injectable({
  providedIn: 'root'
})

export class KommuneService {

  constructor(private httpClient: HttpClient) { }

  hentKommuner(): Observable<Kommune[]> {
    const url = `${environment.apiBaseUrl}/v1/kommune`;
    return this.httpClient.get<Kommune[]>(url);
  }
}
