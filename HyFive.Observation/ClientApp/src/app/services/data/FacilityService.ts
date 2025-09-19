import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Facility } from '../../models/api/Facility';
import { Localstoragepaths } from '../../constants/localstoragepaths';

@Injectable({
  providedIn: 'root'
})
export class FacilityService {

  constructor(private readonly http: HttpClient) { }

  getFacility(id: number): Observable<Facility> {
    return this.getFacilities().pipe(map((p: Facility[]) => p.find(x => x.id === id)));
  }

  getSelectedFacility(): Observable<Facility> {
    let selectedFacilityId = this.getSelectedFacilityId();
    return this.getFacility(selectedFacilityId);
  }

  getFacilities(): Observable<Facility[]> {
    const url = `${environment.apiBaseUrl}/v1/facility/`;
    return this.http.get<Facility[]>(url).pipe(map(data => data.filter(x => x != null)));
  }

  getSelectedFacilityId(): number | null {
    const selectedFacilityIdString = localStorage.getItem(Localstoragepaths.SelectedFacility);
    return selectedFacilityIdString ? parseInt(selectedFacilityIdString) : null;
  }

  updateSelectedFacilityId(facilityId: number): number | null {
    localStorage.setItem(Localstoragepaths.SelectedFacility, JSON.stringify(facilityId));
    return this.getSelectedFacilityId();
  }
}
