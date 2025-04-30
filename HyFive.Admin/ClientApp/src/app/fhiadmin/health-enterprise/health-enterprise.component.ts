import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { RegionalHealthcareEnterpriseService } from 'src/app/services/data/regional-healthcare-enterprise.service';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { CreateHealthEnterpriseRequest } from '../../models/api/CreateHealthEnterpriseRequest';
import { HealthcareEnterpriseService } from '../../services/data/healthcareOrganization.service';
import { HealthcareEnterprise } from 'src/app/models/api/HealthcareEnterprise';
import { RegionalHealthcareEnterprise } from 'src/app/models/api/RegionalHealthcareEnterprise';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-healthcareOrganization',
  templateUrl: './health-enterprise.component.html'
})
export class HealthEnterpriseComponent implements OnInit, OnDestroy
{
  listOfHealthcareEnterpises: HealthcareEnterprise[] = null;
  newHealthcareEnterprise = this.createEmptyHealthcareEnterprise();
  healthcareEnterpriseAsChanged = null;
  regionalHealthcareEnterpriseList: RegionalHealthcareEnterprise[] = null;
  loading: boolean;

  constructor(
    private healthcareEnterpriseService: HealthcareEnterpriseService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private regionalHealthcareEnterpriseService: RegionalHealthcareEnterpriseService
  ) { }
  
  ngOnInit(): void {
    this.loading = true;
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit(event);
    });

    const healthcareEnterpriseRequest = [
      this.healthcareEnterpriseService.getAllHealthcareEnterprises(),
      this.regionalHealthcareEnterpriseService.getAllRegionalHealthcareEnterprises()
    ];

    forkJoin(healthcareEnterpriseRequest).subscribe((result) => {
      let i = 0;
      this.listOfHealthcareEnterpises = result[i++] as HealthcareEnterprise[];
      this.regionalHealthcareEnterpriseList = result[i++] as RegionalHealthcareEnterprise[];
      this.loading = false;
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  createEmptyHealthcareEnterprise() {
    return {
      name: '',
      regionalHealthcareEnterpriseId: 0
    } as CreateHealthEnterpriseRequest;
  }

  createHealthcareEnterprise() {
    this.healthcareEnterpriseService.createHealthcareEnterprise(this.newHealthcareEnterprise).subscribe(
      (isCreated) => {
        if(isCreated){
          this.toastrService.success(this.newHealthcareEnterprise.name + " was created");
          this.newHealthcareEnterprise = this.createEmptyHealthcareEnterprise();
          this.loadAllHealthcareEnterprises();
        }
        else{
          this.toastrService.error("HealthcareEnterprise already exists", '', { disableTimeOut: true})
        }
      },
      () => {
        this.toastrService.error("Error creating HealthcareEnterprise", '', { disableTimeOut: true});
      }
    );
  }

  loadAllHealthcareEnterprises() {
    this.healthcareEnterpriseService.getAllHealthcareEnterprises().subscribe(
      (allHealthcateEnterprises) => {
        this.listOfHealthcareEnterpises = allHealthcateEnterprises;
      }
    );
  }

  selectedHealthcareEnterpriseAsChanged(healthcareOrganization: HealthcareEnterprise) {
    if (this.healthcareEnterpriseAsChanged?.id == healthcareOrganization.id) return;
    this.healthcareEnterpriseAsChanged = JSON.parse(JSON.stringify(healthcareOrganization));
  }

  updateHealthcareEnterprise(healthcareOrganization: HealthcareEnterprise) {
    this.healthcareEnterpriseService.updateHealthcareEnterprise(healthcareOrganization).subscribe(
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

  canChange(healthcareOrganization: HealthcareEnterprise) {
    return healthcareOrganization.name.length > 0;
  }

  cancelEdit($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.healthcareEnterpriseAsChanged = null;
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: HealthcareEnterprise) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: HealthcareEnterprise) => x.name.toLowerCase();
        break;
      case "Regional healthcareOrganization":
        propertyOf = (x: HealthcareEnterprise) => x.regionalHealthcareEnterprise?.name;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: HealthcareEnterprise, b: HealthcareEnterprise) => {

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











