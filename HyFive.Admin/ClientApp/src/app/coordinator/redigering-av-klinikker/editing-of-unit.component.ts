import { Component, OnInit } from '@angular/core';
import { Department} from '../../models/api/Department';
import { FacilityService } from '../../services/data/facility.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Unit } from '../../models/api/Unit';
import { UnitService } from '../../services/data/unit.service';
import { QueryParameters } from "../../_common/constants/queryparameters";
import { Facility } from '../../models/api/Facility';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-editing-of-unit',
  templateUrl: './editing-of-unit.component.html'
})
export class EditingUnitsComponent implements OnInit {

  units: Unit[] = [];
  facilityName: string;
  facilityId: number;
  unitId = 0;
  unitAsEdited: Unit;

  loading: boolean = false;

  constructor(private facilityService: FacilityService,
    private unitService: UnitService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.getUnits();
  }

  getUnits() {
    this.loading = true;
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    this.facilityService.getFacility(selectedFacilityId).subscribe((result: Facility) => {
      this.facilityName = result.name;
      this.facilityId = result.id;
      this.unitService.getUnitsForFacility(this.facilityId).subscribe(units => {
        this.loading = false;
        this.units = units;
        this.route.queryParams.subscribe(
          params => {
            const UnitIdFromQuery = params[QueryParameters.id] || 0;
            this.unitId = parseInt(UnitIdFromQuery, 0);
            this.unitAsEdited = this.units.find(a => a.id === this.unitId);
          }
        );
      });
    });
  }

  getDepartmentName(unit: Unit) {
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

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: Unit) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: Unit) => x.name;
        break;
      case "Departments":
        propertyOf = (x: Unit) => x.departments;
        break;
      default:
        throw new Error("Invalid sort column");
    }
    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;
    const sortFunc = (a: Unit, b: Unit) => {
      if(typeof propertyOf(this.units[0]) === 'string') {
        const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
        return result * sortOrder;
      } else {
        const result = (propertyOf(a).map(p => p.name).join() < propertyOf(b).map(p => p.name).join()) ? -1 : (propertyOf(a).map(p => p.name).join() > propertyOf(b).map(p => p.name).join()) ? 1 : 0;
        return result * sortOrder; 
      }
    };

    this.units = this.units.sort(sortFunc);
  }
}
