import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { RegionalHealthcareOrganization } from "src/app/models/api/RegionalHealthcareOrganization";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class RegionalHealthcareEnterpriseService {

    constructor(private httpClient: HttpClient) { }

    getAllRegionalHealthcareEnterprises() : Observable<RegionalHealthcareOrganization[]> {
        const url = `${environment.apiBaseUrl}/v1/regionalhealthcareenterprise`;
        var regionalHealthcareOrganisationList = this.httpClient.get<RegionalHealthcareOrganization[]>(url);
        return regionalHealthcareOrganisationList;
    }
}