import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { RegionalHealthcareOrganizationService } from 'src/app/services/data/regional-healthcare-enterprise.service';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { CreateHealthEnterpriseRequest } from '../../models/api/CreateHealthEnterpriseRequest';
import { HealthcareOrganizationService } from '../../services/data/healthcareOrganization.service';
import { HealthcareOrganization } from 'src/app/models/api/HealthcareOrganization';
import { RegionalHealthcareOrganization } from 'src/app/models/api/RegionalHealthcareOrganization';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-healthcareOrganization',
  templateUrl: './health-enterprise.component.html'
})
export class HealthEnterpriseComponent implements OnInit, OnDestroy
{
  listOfHealthcareEnterpises: HealthcareOrganization[] = null;
  newHealthcareEnterprise = this.createEmptyHealthcareEnterprise();
  healthcareEnterpriseAsChanged = null;
  RegionalHealthcareOrganizationList: RegionalHealthcareOrganization[] = null;
  loading: boolean;

  constructor(
    private HealthcareOrganizationService: HealthcareOrganizationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private regionalHealthcareOrganizationService: RegionalHealthcareOrganizationService
  ) { }
  
  ngOnInit(): void {
    this.loading = true;
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit(event);
    });

    const healthcareEnterpriseRequest = [
      this.HealthcareOrganizationService.getAllHealthcareEnterprises(),
      this.regionalHealthcareOrganizationService.getAllRegionalHealthcareOrganizations()
    ];

    forkJoin(healthcareEnterpriseRequest).subscribe((result) => {
      let i = 0;
      this.listOfHealthcareEnterpises = result[i++] as HealthcareOrganization[];
      this.RegionalHealthcareOrganizationList = result[i++] as RegionalHealthcareOrganization[];
      this.loading = false;
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  createEmptyHealthcareEnterprise() {
    return {
      name: '',
      RegionalHealthcareOrganizationId: 0
    } as CreateHealthEnterpriseRequest;
  }

  createHealthcareEnterprise() {
    this.HealthcareOrganizationService.createHealthcareEnterprise(this.newHealthcareEnterprise).subscribe(
      (isCreated) => {
        if(isCreated){
          this.toastrService.success(this.newHealthcareEnterprise.name + " was created");
          this.newHealthcareEnterprise = this.createEmptyHealthcareEnterprise();
          this.loadAllHealthcareEnterprises();
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

  loadAllHealthcareEnterprises() {
    this.HealthcareOrganizationService.getAllHealthcareEnterprises().subscribe(
      (allHealthcateEnterprises) => {
        this.listOfHealthcareEnterpises = allHealthcateEnterprises;
      }
    );
  }

  selectedHealthcareEnterpriseAsChanged(healthcareOrganization: HealthcareOrganization) {
    if (this.healthcareEnterpriseAsChanged?.id == healthcareOrganization.id) return;
    this.healthcareEnterpriseAsChanged = JSON.parse(JSON.stringify(healthcareOrganization));
  }

  updateHealthcareEnterprise(healthcareOrganization: HealthcareOrganization) {
    this.HealthcareOrganizationService.updateHealthcareEnterprise(healthcareOrganization).subscribe(
      (isUpdated) => {
        if(isUpdated) {
        this.toastrService.success(healthcareOrganization.name + " is updated");
        this.healthcareEnterpriseAsChanged = null;
        this.loadAllHealthcareEnterprises();
        }
        else{
          this.toastrService.error(healthcareOrganization.name + " exists from before", '', { disableTimeOut: true})
        }
      }
    );
  }

  canChange(healthcareOrganization: HealthcareOrganization) {
    return healthcareOrganization.name.length > 0;
  }

  cancelEdit($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.healthcareEnterpriseAsChanged = null;
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: HealthcareOrganization) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: HealthcareOrganization) => x.name.toLowerCase();
        break;
      case "Regional healthcareOrganization":
        propertyOf = (x: HealthcareOrganization) => x.regionalHealthcareOrganization?.name;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: HealthcareOrganization, b: HealthcareOrganization) => {

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

    this.listOfHealthcareEnterpises = this.listOfHealthcareEnterpises.sort(sortFunc);
  }
}











