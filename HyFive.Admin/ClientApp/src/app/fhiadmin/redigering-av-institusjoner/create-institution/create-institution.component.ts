import {Component, EventEmitter, OnInit, Output, OnDestroy, Input} from '@angular/core';
import { InstitutionType } from '../../../models/api/InstitutionType';
import { CreateInstitutionRequest } from '../../../models/api/CreateInstitutionRequest';
import { InstitutionService } from '../../../services/data/institution.service';
import { ToastrService } from 'ngx-toastr';
import { Institution } from '../../../models/api/Institution';
import { HealthcareOrganization } from 'src/app/models/api/HealthcareOrganization';
import { HealthcareOrganizationService } from 'src/app/services/data/healthcareOrganization.service';
import { InstitutionTypeConstants } from 'src/app/models/api/InstitutionTypeConstants';
import { Municipality } from 'src/app/models/api/Municipality';
import { MunicipalityService } from 'src/app/services/data/municipality.service';
import { User } from 'src/app/models/api/User';

@Component({
  selector: 'app-create-institution',
  templateUrl: './create-institution.component.html'
})
export class CreateInstitutionComponent implements OnInit, OnDestroy {

  institutionTypes: InstitutionType[] = [];
  newInstitution: CreateInstitutionRequest = null;
  municipalities: Municipality[] = [];
  listOfHealthcareOrganizations: HealthcareOrganization[] = [];
  showHealthcareOrganization: boolean = false;
  showMunicipality: boolean = false;

  @Input() institutions: Institution[] = [];
  @Input() coordinators: User[] = [];
  @Output() institutionCreatedEvent: EventEmitter<Institution> = new EventEmitter<Institution>();

  constructor(private institutionService: InstitutionService,
              private toastrService: ToastrService,
              private municipalityService: MunicipalityService, 
              private healthcareOrganizationService: HealthcareOrganizationService) { }

  ngOnInit(): void {
    this.institutionService.getInstitutionTypes().subscribe((result) => {
      this.institutionTypes = result;
      this.newInstitution = this.createDefaultInstitution();
    });

    this.municipalityService.getMunicipalities().subscribe(
      (municipalities) => {
        this.municipalities = municipalities;
      }
    );

    this.healthcareOrganizationService.getAllHealthcareOrganizations().subscribe(
      (allHealthcareOrganization) => {
        this.listOfHealthcareOrganizations = allHealthcareOrganization;
      }
    );
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  findDefaultInstitutionType() {
    return this.institutionTypes.find(p => p.code === InstitutionTypeConstants.Hospital);
  }

  createInstitution() {
    this.institutionService.createInstitution(this.newInstitution).subscribe((result) => {
        this.toastrService.success('Institution created', `Institution with ID: ${result.id} created`);
        this.institutionCreatedEvent.emit(result);
      },
        (err) => this.toastrService.error(`An error occurred while creating the institution. Error message from server: ${err}`, 'Error creating institution', { disableTimeOut: true }),
      () => {
        this.newInstitution = this.createDefaultInstitution();

      }
    );
  }

 private createDefaultInstitution(): CreateInstitutionRequest {
    let defaultInstitutionType = this.findDefaultInstitutionType();
    this.showHealthcareOrRegion(defaultInstitutionType.id);
    return {
      institutionName: null,
      institutionTypeId: defaultInstitutionType.id,
      coordinatorLastName: null,
      coordinatorFirstName: null,
      coordinatorHPRNumber: null,
      coordinatorEmail: null,
      coordinatorPseudonym: null,
      herId: null,
      abbreviation: null,
      regionId: 0,
      municipalityId: 0,
      healthcareOrganizationId: 0
    };    
  }

  canNotCreateInstitution(): boolean {
    return this.canCreateInstitution() === false;
  }

  canCreateInstitution(): boolean{
    return this.newInstitution?.institutionName?.length > 0
      // && this.newInstitution?.coordinatorHPRNumber?.length > 0
      && this.newInstitution?.coordinatorFirstName?.length > 0
      && this.newInstitution?.coordinatorLastName?.length > 0
      && this.coordinators.find(fc => fc.email == this.newInstitution?.coordinatorEmail) == undefined
      && this.newInstitution?.coordinatorEmail?.length > 0
      && this.institutions.find(i => i.name === this.newInstitution.institutionName) == undefined;
  }

  showHealthcareOrRegion(institutionTypeId: number) {
    var selectedInstitutiontype = this.institutionTypes.find(i => i.id === institutionTypeId);
    if(selectedInstitutiontype.code === InstitutionTypeConstants.Hospital)
    {
      this.showHealthcareOrganization = true;
      this.showMunicipality = false;
    }
    else if(selectedInstitutiontype.code === InstitutionTypeConstants.NursingHome)
    {
      this.showMunicipality = true;
      this.showHealthcareOrganization = false;
    }
    else
    {
      this.showHealthcareOrganization = false;
      this.showMunicipality = false;
    }
  }

  omitSpecialChar(event) {   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }
}
