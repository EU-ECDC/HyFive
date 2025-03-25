import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { RegionaltHelseforetak } from "src/app/models/api/RegionaltHelseforetak";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class RegionaltHelseforetakService {

    constructor(private httpClient: HttpClient) { }

    hentAlleRegionaltHelseforetak() : Observable<RegionaltHelseforetak[]> {
        const url = `${environment.apiBaseUrl}/v1/regionalthelseforetak`;
        var regionaltHelseforetakListe = this.httpClient.get<RegionaltHelseforetak[]>(url);
        return regionaltHelseforetakListe;
    }
}