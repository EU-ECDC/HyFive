import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { RegionalHealthcareOrganizationService } from 'src/app/services/data/regional-healthcare-rganization.service';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { CreateHealthOrganisationRequest } from '../../models/api/CreateHealthOrganisationRequest';
import { HealthcareOrganizationService } from '../../services/data/healthcareOrganization.service';
import { HealthcareOrganisation } from 'src/app/models/api/HealthcareOrganisation';
import { RegionalHealthcareOrganization } from 'src/app/models/api/RegionalHealthcareOrganization';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-healthcareOrganization',
  templateUrl: './health-institution.component.html'
})
export class HealthInstitutionComponent implements OnInit, OnDestroy
{
  listOfHealthcareOrganizations: HealthcareOrganisation[] = null;
  newHealthcareCompany = this.createEmptyHealthcareOrganization();
  healthcareCompanyAsChanged = null;
  regionalHealthcareOrganisationList: RegionalHealthcareOrganization[] = null;
  loading: boolean;

  constructor(
    private healthcareOrganizationService: HealthcareOrganizationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private regionalHealthcareOrganizationService: RegionalHealthcareOrganizationService
  ) { }
  
  ngOnInit(): void {
    this.loading = true;
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit(event);
    });

    const healthcareOrganizationRequest = [
      this.healthcareOrganizationService.getAllHealthcareOrganizations(),
      this.regionalHealthcareOrganizationService.getAllRegionalHealthcareOrganizations()
    ];

    forkJoin(healthcareOrganizationRequest).subscribe((result) => {
      let i = 0;
      this.listOfHealthcareOrganizations = result[i++] as HealthcareOrganisation[];
      this.regionalHealthcareOrganisationList = result[i++] as RegionalHealthcareOrganization[];
      this.loading = false;
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  createEmptyHealthcareOrganization() {
    return {
      name: '',
      regionalHealthcareOrganisationId: 0
    } as CreateHealthOrganisationRequest;
  }

  createHealthcareOrganization() {
    this.healthcareOrganizationService.createHealthcareOrganization(this.newHealthcareCompany).subscribe(
      (isCreated) => {
        if(isCreated){
          this.toastrService.success(this.newHealthcareCompany.name + " was created");
          this.newHealthcareCompany = this.createEmptyHealthcareOrganization();
          this.loadAllHealthcareOrganizations();
        }
        else{
          this.toastrService.error("HealthcareOrganization already exists", '', { disableTimeOut: true})
        }
      },
      () => {
        this.toastrService.error("Error creating HealthcareOrganization", '', { disableTimeOut: true});
      }
    );
  }

  loadAllHealthcareOrganizations() {
    this.healthcareOrganizationService.getAllHealthcareOrganizations().subscribe(
      (allHealthcateOrganizations) => {
        this.listOfHealthcareOrganizations = allHealthcateOrganizations;
      }
    );
  }

  selectedHealthcareOrganizationAsChanged(healthcareOrganization: HealthcareOrganisation) {
    if (this.healthcareCompanyAsChanged?.id == healthcareOrganization.id) return;
    this.healthcareCompanyAsChanged = JSON.parse(JSON.stringify(healthcareOrganization));
  }

  updateHealthcareOrganization(healthcareOrganization: HealthcareOrganisation) {
    this.healthcareOrganizationService.updateHealthcareOrganization(healthcareOrganization).subscribe(
      (isUpdated) => {
        if(isUpdated) {
        this.toastrService.success(healthcareOrganization.name + " is updated");
        this.healthcareCompanyAsChanged = null;
        this.loadAllHealthcareOrganizations();
        }
        else{
          this.toastrService.error(healthcareOrganization.name + " exists from before", '', { disableTimeOut: true})
        }
      }
    );
  }

  kanEndres(healthcareOrganization: HealthcareOrganisation) {
    return healthcareOrganization.name.length > 0;
  }

  cancelEdit($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.healthcareCompanyAsChanged = null;
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: HealthcareOrganisation) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: HealthcareOrganisation) => x.name.toLowerCase();
        break;
      case "Regional healthcareOrganization":
        propertyOf = (x: HealthcareOrganisation) => x.regionaltHelseforetak?.name;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: HealthcareOrganisation, b: HealthcareOrganisation) => {

      let propertyOfA = propertyOf(a);
      let propertyOfB = propertyOf(b);

      if(propertyOfA == undefined) {
        return 1;
      }
      if(propertyOfB == undefined) {
        return -1;
      }

      const result = (propertyOfA < propertyOfB) ? -1 : (propertyOfA > propertyOfB) ? 1 : 0;
      return result * sortOrder;
    };

    this.listOfHealthcareOrganizations = this.listOfHealthcareOrganizations.sort(sortFunc);
  }
}











