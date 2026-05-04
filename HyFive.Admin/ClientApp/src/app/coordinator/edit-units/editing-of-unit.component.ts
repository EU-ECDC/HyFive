import { Component, OnInit } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { UnitService } from '../../services/data/unit.service';
import { QueryParameters } from "../../_common/constants/queryparameters";
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { TranslateService } from '@ngx-translate/core';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { UnitResponse } from 'src/app/models/api/UnitResponse';

@Component({
  selector: 'app-editing-of-unit',
  templateUrl: './editing-of-unit.component.html'
})
export class EditingUnitsComponent implements OnInit {

  units: UnitResponse[] = [];
  filteredUnits: UnitResponse[] = [];
  facilityName: string;
  facilityId: number;
  unitId = 0;
  unitAsEdited: UnitResponse;
  showCreateUnitForm: boolean = false;
  loading: boolean = false;
  keyword: string;

  constructor(private readonly facilityService: FacilityService,
              private readonly unitService: UnitService,
              private readonly router: Router,
              private readonly route: ActivatedRoute,
              private readonly translate: TranslateService) { }

  ngOnInit(): void {
    this.getUnits();
  }

  getUnits() {
    this.showCreateUnitForm = false;
    this.loading = true;
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    this.facilityService.getFacility(selectedFacilityId).subscribe((result: OrganisationUnit) => {
      this.facilityName = result.name;
      this.facilityId = result.id;
      this.unitService.getUnitsForFacility(this.facilityId).subscribe(units => {
        this.loading = false;
        this.units = units;
        this.filteredUnits = units;
        this.route.queryParams.subscribe(
          params => {
            const UnitIdFromQuery = params[QueryParameters.id] || 0;
            this.unitId = Number.parseInt(UnitIdFromQuery, 0);
            this.unitAsEdited = this.units.find(a => a.id === this.unitId);
          }
        );
      });
    });
  }

  getDepartmentName(unit: UnitResponse) {
    return unit.departments?.map(r => r.name).join(',');
  }

  navigateToUnit(id: number) {
    if (id === 0) {
      this.router.navigate([], { relativeTo: this.route });
    }

    const queryParams: Params = { id };
    this.router.navigate(
      [],
      {
        relativeTo: this.route,
        queryParams,
        queryParamsHandling: 'merge'
      });
  }

  toggleShowForm() {
    this.showCreateUnitForm = !this.showCreateUnitForm;
  }

    sort($event: IColumnSortedEvent) {
    let propertyOf: (x: UnitResponse) => any;
    switch ($event.columnName) {
      case this.translate.instant("Name"):
        propertyOf = (x: UnitResponse) => x.name;
        break;
      case this.translate.instant("Departments"):
        propertyOf = (x: UnitResponse) => x.departments;
        break;
      default:
        throw new Error(this.translate.instant("Invalid sort column"));
    }
    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;
    const sortFunc = (a: UnitResponse, b: UnitResponse) => {
      if(typeof propertyOf(this.units[0]) === 'string') {
        const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
        return result * sortOrder;
      } else {
        const result = (propertyOf(a).map(p => p.name).join() < propertyOf(b).map(p => p.name).join()) ? -1 : (propertyOf(a).map(p => p.name).join() > propertyOf(b).map(p => p.name).join()) ? 1 : 0;
        return result * sortOrder; 
      }
    };

    this.units.sort(sortFunc);
  }

    filterUnits() {
    if(this.keyword.length >= 2) {
      this.filteredUnits = this.units.filter(a => a.name.toLowerCase().includes(this.keyword.toLowerCase()));
    }
    else if(this.keyword.length === 0)
      this.filteredUnits = this.units;
  }
}
