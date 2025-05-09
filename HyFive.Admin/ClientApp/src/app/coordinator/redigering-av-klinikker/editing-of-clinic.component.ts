import { Component, OnInit } from '@angular/core';
import { Department} from '../../models/api/Department';
import { InstitutionService } from '../../services/data/institution.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Clinic } from '../../models/api/Clinic';
import { ClinicService } from '../../services/data/clinic.service';
import { QueryParameters } from "../../_common/konstanter/queryparameters";
import { Institution } from '../../models/api/Institution';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-editing-of-clinic',
  templateUrl: './editing-of-clinic.component.html'
})
export class EditingClinicsComponent implements OnInit {

  clinics: Clinic[] = [];
  institutionName: string;
  institutionId: number;
  clinicId = 0;
  clinicAsEdited: Clinic;

  loading: boolean = false;

  constructor(private institutionService: InstitutionService,
    private clinicService: ClinicService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.getClinics();
  }

  getClinics() {
    this.loading = true;
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getInstitution(selectedInstitutionId).subscribe((result: Institution) => {
      this.institutionName = result.name;
      this.institutionId = result.id;
      this.clinicService.getClinicsForInstitution(this.institutionId).subscribe(clinics => {
        this.loading = false;
        this.clinics = clinics;
        this.route.queryParams.subscribe(
          params => {
            const ClinicIdFromQuery = params[QueryParameters.id] || 0;
            this.clinicId = parseInt(ClinicIdFromQuery, 0);
            this.clinicAsEdited = this.clinics.find(a => a.id === this.clinicId);
          }
        );
      });
    });
  }

  getDepartmentName(clinic: Clinic) {
    return clinic.departments?.map(r => r.name).join(',');
  }

  navigateToClinic(id: number) {
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
    let propertyOf: (x: Clinic) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: Clinic) => x.name;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Clinic, b: Clinic) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.clinics = this.clinics.sort(sortFunc);
  }
}
