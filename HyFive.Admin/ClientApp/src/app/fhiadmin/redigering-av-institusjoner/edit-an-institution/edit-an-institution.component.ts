import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { InstitutionService } from '../../../services/data/institution.service';
import { Institution } from '../../../models/api/Institution';
import { InstitutionType } from '../../../models/api/InstitutionType';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_common/konstanter/url-paths';
import { HealthcareOrganization } from 'src/app/models/api/HealthcareOrganization';
import { HealthcareOrganizationService } from 'src/app/services/data/healthcareOrganization.service';
import { InstitutionTypeConstants } from 'src/app/models/api/InstitutionTypeConstants';
import { MunicipalityService } from 'src/app/services/data/municipality.service';
import { Municipality } from 'src/app/models/api/Municipality';

@Component({
  selector: 'app-edit-an-institution',
  templateUrl: './edit-an-institution.component.html'
})
export class EditInstitutionComponent implements OnInit {

  constructor(private institutionService: InstitutionService,
              private toastrService: ToastrService,
              private municipalityService: MunicipalityService,
              private healthcareOrganizationService: HealthcareOrganizationService) { }

  institution: Institution = null;
  institutionTypes: InstitutionType[] = [];
  institutiontypeId = 0;
  listOfHealthcareOrganizations: HealthcareOrganization[] = [];

  municipality: Municipality = null;
  municipalities: Municipality[];
  municipalityId = 0;
  UrlPaths = UrlPaths;
  healthcareOrganizationId = 0;
  showMunicipality = false;
  showHealthcareOrganization = false;


  @Input() institutionId: number;
  @Input() institutions: Institution[] = [];
  @Output() institutionDeletedEvent: EventEmitter<number> = new EventEmitter<number>();
  @Output() institutionUpdatedEvent: EventEmitter<Institution> = new EventEmitter<Institution>();

  ngOnInit(): void {
    if (this.institutionId === 0) {
      this.institution = null;
      return;
    }
    this.institutionService.getInstitution(this.institutionId).subscribe((institution) => {
      this.institution = institution;
      this.institutiontypeId = institution.institutionType.id;
      this.municipalityId = institution.municipality?.id;
      this.healthcareOrganizationId = institution.healthcareOrganization?.id;
      
      this.institutionService.getInstitutionTypes().subscribe((types) => {
        this.institutionTypes = types;
        this.showMunicipality = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitutionTypeConstants.NursingHome;
        this.showHealthcareOrganization = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitutionTypeConstants.Hospital;
      });
    });

    this.municipalityService.getMunicipalities().subscribe(
      (municipalities) => {
        this.municipalities = [ {id: 0, number: null, name: null}, ...municipalities];
    });

    this.healthcareOrganizationService.getAllHealthcareOrganizations().subscribe(
      (allHealthcareorganization) => {
        this.listOfHealthcareOrganizations = [ { id: 0, name: null, regionalHealthcareOrganization: null, regionalHealthcareOrganizationId: null }, ...allHealthcareorganization];
    });
  }

  deleteInstitution() {
      if (this.institutionId > 0) {
        this.institutionService.deleteInstitution(this.institutionId).subscribe(() => {
          this.institution = null;
          this.institutionDeletedEvent.emit(this.institutionId);
        },
          (error =>
            this.toastrService.error(`An error occurred: ${error?.error} / ${error?.message}`, 'Error while deleting institution', { disableTimeOut: true})));
      }
  }

  institutionTypeChanged() {
    this.institution.healthcareOrganization = null;
    this.institution.municipality = null;
    this.healthcareOrganizationId = 0;
    this.municipalityId = 0;
    this.institution.institutionType = this.institutionTypes.find(i => i.id === this.institutiontypeId);
    this.showMunicipality = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitutionTypeConstants.NursingHome;
     this.showHealthcareOrganization = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitutionTypeConstants.Hospital;
  }

  municipalityChanged() {
      this.healthcareOrganizationId = 0;
      this.institution.healthcareOrganization = null;
      if (this.municipalityId == 0) {
        this.institution.municipality = null;
      } else {
        this.institution.municipality = this.municipalities.find(r => r.id === this.municipalityId);
      }
  }

  saveInstitution() {
    this.institutionService.updateInstitution(this.institution).subscribe(
      (institution) => {
        this.toastrService.success('The institution was updated');
        this.institutionUpdatedEvent.emit(institution);
      },
      (error) => this.toastrService.error(`An error occurred: ${error?.error} / ${error?.message}`, 'Error during update', { disableTimeOut: true}));
  }

  healthEnterpriseChanged() {
      this.municipalityId = 0;
      this.institution.municipality = null;
      if (this.healthcareOrganizationId == 0) {
        this.institution.healthcareOrganization = null;
      } else {
        this.institution.healthcareOrganization = this.listOfHealthcareOrganizations.find(r => r.id === this.healthcareOrganizationId);
      }
  }

  canNotSaveInstitution(): boolean {
    if (this.institution.institutionType.id > 0 
      && this.institution.name?.length > 0
      && this.institutions.filter(i => i.id !== this.institution.id).find(i => i.name === this.institution.name) == undefined
    )
      return false;
    else
      return true;
  }

  omitSpecialChar(event){   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }
}

