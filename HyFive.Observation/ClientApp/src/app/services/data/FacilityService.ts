import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Localstoragepaths } from '../../constants/localstoragepaths';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';

@Injectable({
  providedIn: 'root'
})
export class FacilityService {

  constructor(private readonly http: HttpClient) { }

  getFacility(id: number): Observable<OrganisationUnit> {
    return this.getFacilities().pipe(map((p: OrganisationUnit[]) => p.find(x => x.id === id)));
  }

  getSelectedFacility(): Observable<OrganisationUnit> {
    let selectedFacilityId = this.getSelectedFacilityId();
    return this.getFacility(selectedFacilityId);
  }

  getFacilities(): Observable<OrganisationUnit[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/`;
    return this.http.get<OrganisationUnit[]>(url).pipe(
                                                map(data => data.filter(x => x != null)),
                                                map(data => {
                                                              let distinctSorted = [...new Map(data.map(item => [item.id, item])).values()]
                                                                          ?.sort((a,b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" }));
                                                              distinctSorted.forEach(facility => facility.children?.sort((a,b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" })));
                                                              return distinctSorted;
                                                            }
                                                    )
                                              );
  }

  getSelectedFacilityId(): number | null {
    const selectedFacilityIdString = localStorage.getItem(Localstoragepaths.SelectedFacility);
    return selectedFacilityIdString ? Number.parseInt(selectedFacilityIdString) : null;
  }

  updateSelectedFacilityId(facilityId: number): number | null {
    localStorage.setItem(Localstoragepaths.SelectedFacility, JSON.stringify(facilityId));
    return this.getSelectedFacilityId();
  }
}
