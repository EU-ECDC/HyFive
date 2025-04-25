import {Component, EventEmitter, OnInit, Output, OnDestroy} from '@angular/core';
import { InstitutionType } from '../../../models/api/InstitutionType';
import { CreateInstitutionRequest } from '../../../models/api/CreateInstitutionRequest';
import { InstitutionService } from '../../../services/data/institution.service';
import { ToastrService } from 'ngx-toastr';
import { Institution } from '../../../models/api/Institution';
import { HealthcareEnterprise } from 'src/app/models/api/HealthcareEnterprise';
import { HealthcareEnterpriseService } from 'src/app/services/data/healthcareEnterprise.service';
import { InstitutionTypeConstants } from 'src/app/models/api/InstitutionTypeConstants';
import { Municipality } from 'src/app/models/api/Municipality';
import { MunicipalityService } from 'src/app/services/data/municipality.service';

@Component({
  selector: 'app-create-institution',
  templateUrl: './create-institution.component.html'
})
export class CreateInstitutionComponent implements OnInit, OnDestroy {

  institutionTypes: InstitutionType[] = [];
  newInstitution: CreateInstitutionRequest = null;
  municipalities: Municipality[] = [];
  listOfHealthcareEnterprises: HealthcareEnterprise[] = [];
  showHealthcareEnterprise: boolean = false;
  showMunicipality: boolean = false;

  @Output() institutionCreatedEvent: EventEmitter<Institution> = new EventEmitter<Institution>();

  constructor(private institutionService: InstitutionService,
              private toastrService: ToastrService,
              private municipalityService: MunicipalityService, 
              private healthcareEnterpriseService: HealthcareEnterpriseService) { }

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

    this.healthcareEnterpriseService.getAllHealthcareEnterprises().subscribe(
      (allHealthcareEnterprise) => {
        this.listOfHealthcareEnterprises = allHealthcareEnterprise;
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
      coordinatorLastname: null,
      coordinatorFirstname: null,
      coordinatorHPRnumber: null,
      coordinatorEmail: null,
      coordinatorPseudonym: null,
      herId: null,
      abbreviation: null,
      regionId: 0,
      municipalityId: 0,
      healthEnterpriseId: 0
    };    
  }

  canNotCreateInstitution(): boolean {
    return this.canCreateInstitution() === false;
  }

  canCreateInstitution(): boolean{
    return this.newInstitution?.institutionName?.length > 0
      && this.newInstitution?.coordinatorHPRnumber?.length > 0
      && this.newInstitution?.coordinatorFirstname?.length > 0
      && this.newInstitution?.coordinatorLastname?.length > 0;
  }

  showHealthcareOrRegion(institutionTypeId: number) {
    var selectedInstitutiontype = this.institutionTypes.find(i => i.id === institutionTypeId);
    if(selectedInstitutiontype.code === InstitutionTypeConstants.Hospital)
    {
      this.showHealthcareEnterprise = true;
      this.showMunicipality = false;
    }
    else if(selectedInstitutiontype.code === InstitutionTypeConstants.NursingHome)
    {
      this.showMunicipality = true;
      this.showHealthcareEnterprise = false;
    }
    else
    {
      this.showHealthcareEnterprise = false;
      this.showMunicipality = false;
    }
  }
}
