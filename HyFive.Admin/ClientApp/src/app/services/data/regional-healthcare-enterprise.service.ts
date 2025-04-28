import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { RegionalHealthcareEnterprise } from "src/app/models/api/RegionalHealthcareEnterprise";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class RegionalHealthcareEnterpriseService {

    constructor(private httpClient: HttpClient) { }

    getAllRegionalHealthcareEnterprises() : Observable<RegionalHealthcareEnterprise[]> {
        const url = `${environment.apiBaseUrl}/v1/regionalhealthcareenterprise`;
        var regionalHealthcareOrganisationList = this.httpClient.get<RegionalHealthcareEnterprise[]>(url);
        return regionalHealthcareOrganisationList;
    }
}