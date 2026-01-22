import { Component, OnInit } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { Facility } from '../../models/api/Facility';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { QueryParameters } from '../../_common/constants/queryparameters';
import { FacilityReport } from '../../models/api/FacilityReport';
import { User } from 'src/app/models/api/User';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { TranslateService } from '@ngx-translate/core';
import { SortHelper } from 'src/app/utils/sort-helper';

@Component({
  selector: 'app-editing-of-facilities',
  templateUrl: './editing-of-facilities.component.html'
})
export class EditingOfFacilitiesComponent implements OnInit {

  facilityId: number = 0;
  facilities: FacilityReport[] = [];
  filteredFacilities: FacilityReport[] = [];
  totalFacilities: FacilityReport[] = [];
  keyword: string = '';
  keywordPerson: string = '';
  users: User[] = [];
  showCreateFacilityForm: boolean = false;

  constructor(private readonly facilityService: FacilityService,
    private readonly toastrService: ToastrService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly translate: TranslateService) { }


  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        this.facilityId = params[QueryParameters.id] || 0;
      }
    );

    this.getFacilities();
  }

  getFacilities() {
    this.facilityService.getFacilities().subscribe((result) => {
    this.totalFacilities = result;
      this.facilities = result;
      this.filteredFacilities = this.facilities;
      
      for (const i of this.facilities) {
        this.getUsers(i.id);
      };
    });
  }

  getUsers(facilityId: number) {
    this.facilityService.getObservers(facilityId).subscribe((result) => {
      this.users.push(...result);
    });
    this.facilityService.getCoordinators(facilityId).subscribe((result) => {
      this.users.push(...result);
    });
  }

  navigateToFacility(facilityId: number) {

    if (facilityId === 0) {
      this.router.navigate([], { relativeTo: this.route });
    }

    const queryParams: Params = { id: facilityId };
    this.router.navigate(
      [],
      {
        relativeTo: this.route,
        queryParams,
        queryParamsHandling: 'merge'
      });
  }

  updateFacility(facility: Facility) {
    this.facilities[this.facilities.map(i => i.id).indexOf(facility.id)] = facility;
  }

  deleteFacility(facilityId: number) {
    this.getFacilities();
    this.toastrService.success('Deleted facility with id: ' + facilityId, 'Facility deleted');
    this.navigateToFacility(0);
  }

  filterFacilities(): void {
    if (this.keyword.length >= 2)
      this.filteredFacilities = this.facilities.filter(i => i.name.toLowerCase().includes(this.keyword.toLowerCase()) ||
        i.city?.name.toLowerCase().includes(this.keyword.toLowerCase()));
    else if (this.keyword.length === 0)
      this.filteredFacilities = this.facilities;
  }

  filterPersonsAtFacilities(): void {
    if (this.keywordPerson.length >= 2) {
      this.filteredFacilities = this.facilities.filter(i => {
        const persons = SearchHelper.filterUsers(this.keywordPerson, this.users);
        return persons.some(p => p.facilityId === i.id);
      });
    } else if (this.keywordPerson.length === 0) {
      this.filteredFacilities = this.facilities;
    }
  }

  sort($event: IColumnSortedEvent) {
    const userSortConfig = {
      [this.translate.instant("Name")]: (x: Facility) => x.name,
      [this.translate.instant("City")]: (x: Facility) => x.city?.name,
    };

    this.filteredFacilities = SortHelper.sort(this.filteredFacilities, $event, userSortConfig);
  }

  toggleShowForm() {
    this.showCreateFacilityForm = !this.showCreateFacilityForm;
  }
}
