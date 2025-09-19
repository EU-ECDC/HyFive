import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { RegionalHealthcareOrganizationService } from 'src/app/services/data/regional-healthcare-enterprise.service';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { CreateHealthcareOrganizationRequest } from '../../models/api/CreateHealthcareOrganizationRequest';
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
  listOfHealthcareOrganizations: HealthcareOrganization[] = null;
  newHealthcareOrganization = this.createEmptyHealthcareOrganization();
  healthcareOrganizationAsChanged = null;
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

    const healthcareOrganizationRequest = [
      this.HealthcareOrganizationService.getAllHealthcareOrganizations(),
      this.regionalHealthcareOrganizationService.getAllRegionalHealthcareOrganizations()
    ];

    forkJoin(healthcareOrganizationRequest).subscribe((result) => {
      let i = 0;
      this.listOfHealthcareOrganizations = [ { id: 0, name: 'Not selected', regionalHealthcareOrganizationId: 0, regionalHealthcareOrganization: null } , ...result[i++]] as HealthcareOrganization[];
      this.RegionalHealthcareOrganizationList = [ {id: 0, name: 'Not selected'}, ...result[i++]] as RegionalHealthcareOrganization[];
      this.loading = false;
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  createEmptyHealthcareOrganization() {
    return {
      name: '',
      regionalHealthcareOrganizationId: 0
    } as CreateHealthcareOrganizationRequest;
  }

  createHealthcareOrganization() {
    this.HealthcareOrganizationService.createHealthcareOrganization(this.newHealthcareOrganization).subscribe(
      (isCreated) => {
        if(isCreated){
          this.toastrService.success(this.newHealthcareOrganization.name + " was created");
          this.newHealthcareOrganization = this.createEmptyHealthcareOrganization();
          this.loadAllHealthcareOrganizations();
        }
        else{
          this.toastrService.error("City already exists", '', { disableTimeOut: true})
        }
      },
      () => {
        this.toastrService.error("Error creating City", '', { disableTimeOut: true});
      }
    );
  }

    canCreateHealthcareOrganization() {
    return this.newHealthcareOrganization.name.trim().length > 0
      && this.newHealthcareOrganization.regionalHealthcareOrganizationId > 0
      && this.checkCreate()
    }

    checkCreate() {
      if (this.listOfHealthcareOrganizations.length > 0)
      {
          //return this.listOfHealthcareOrganizations.find(x => x.name === this.newHealthcareOrganization.name && x.regionalHealthcareOrganizationId == this.newHealthcareOrganization.regionalHealthcareOrganizationId && this.newHealthcareOrganization.regionalHealthcareOrganizationId == x.regionalHealthcareOrganizationId) == undefined;
          return this.listOfHealthcareOrganizations.find(x => x.name === this.newHealthcareOrganization.name) == undefined;
        } else
      {
          return true;
      }
    }

  loadAllHealthcareOrganizations() {
    this.HealthcareOrganizationService.getAllHealthcareOrganizations().subscribe(
      (allHealthcateEnterprises) => {
        this.listOfHealthcareOrganizations = allHealthcateEnterprises;
      }
    );
  }

  selectedHealthcareOrganizationAsChanged(healthcareOrganization: HealthcareOrganization) {
    if (this.healthcareOrganizationAsChanged?.id == healthcareOrganization.id) return;
    this.healthcareOrganizationAsChanged = JSON.parse(JSON.stringify(healthcareOrganization));
  }

  updateHealthcareOrganization(healthcareOrganization: HealthcareOrganization) {
    this.HealthcareOrganizationService.updateHealthcareOrganization(healthcareOrganization).subscribe(
      (isUpdated) => {
        if(isUpdated) {
        this.toastrService.success(healthcareOrganization.name + " is updated");
        this.healthcareOrganizationAsChanged = null;
        this.loadAllHealthcareOrganizations();
        }
        else{
          this.toastrService.error(healthcareOrganization.name + " exists from before", '', { disableTimeOut: true})
        }
      }
    );
  }

  canUpdateHealthcareOrganization() {
    return this.healthcareOrganizationAsChanged.name.length > 0
      && this.healthcareOrganizationAsChanged.regionalHealthcareOrganizationId > 0
      && this.checkUpdate()
  }

  checkUpdate() {
    if (this.listOfHealthcareOrganizations.length > 0)
    {
        //return this.listOfHealthcareOrganizations.filter(x => x.id !== this.healthcareOrganizationAsChanged.id && this.healthcareOrganizationAsChanged.regionalHealthcareOrganizationId !== x.regionalHealthcareOrganizationId).find(x => x.name === this.healthcareOrganizationAsChanged.name && this.healthcareOrganizationAsChanged.regionalHealthcareOrganizationId !== x.regionalHealthcareOrganizationId) == undefined;
        return this.listOfHealthcareOrganizations.filter(x => x.id !== this.healthcareOrganizationAsChanged.id).find(x => x.name === this.healthcareOrganizationAsChanged.name) == undefined;
      } else
    {
        return true;
    }
  }

  canChange(healthcareOrganization: HealthcareOrganization) {
    return healthcareOrganization.name.length > 0;
  }

  cancelEdit($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.healthcareOrganizationAsChanged = null;
  }

  omitSpecialChar(event)
{   
   var k;  
   k = event.charCode;  //         k = event.keyCode;  (Both can be used)
   return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
}

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: HealthcareOrganization) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: HealthcareOrganization) => x.name.toLowerCase();
        break;
      case "City":
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

    this.listOfHealthcareOrganizations = this.listOfHealthcareOrganizations.sort(sortFunc);
  }
}











