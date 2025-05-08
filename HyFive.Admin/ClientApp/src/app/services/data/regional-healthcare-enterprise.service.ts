import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { RegionalHealthcareOrganization } from "src/app/models/api/RegionalHealthcareOrganization";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class RegionalHealthcareOrganizationService {

    constructor(private httpClient: HttpClient) { }

    getAllRegionalHealthcareOrganizations() : Observable<RegionalHealthcareOrganization[]> {
        const url = `${environment.apiBaseUrl}/v1/regionalhealthcareorganizations`;
        var regionalHealthcareOrganisationList = this.httpClient.get<RegionalHealthcareOrganization[]>(url);
        return regionalHealthcareOrganisationList;
    }
}