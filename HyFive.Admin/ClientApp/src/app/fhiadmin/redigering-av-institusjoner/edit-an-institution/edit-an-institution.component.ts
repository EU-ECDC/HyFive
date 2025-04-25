import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { InstitutionService } from '../../../services/data/institution.service';
import { Institution } from '../../../models/api/Institution';
import { InstitutionType } from '../../../models/api/InstitutionType';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { HealthcareEnterprise } from 'src/app/models/api/HealthcareEnterprise';
import { HealthcareEnterpriseService } from 'src/app/services/data/healthcareEnterprise.service';
import { InstitusjonstypeKonstanter } from 'src/app/models/api/InstitusjonstypeKonstanter';
import { MunicipalityService } from 'src/app/services/data/municipality.service';
import { Municipality } from 'src/app/models/api/Municipality';

@Component({
  selector: 'app-edit-an-institution',
  templateUrl: './edit-an-institution.component.html'
})
export class RedigerEnInstitusjonComponent implements OnInit {

  constructor(private institutionService: InstitutionService,
              private toastrService: ToastrService,
              private municipalityService: MunicipalityService,
              private healthcareEnterpriseService: HealthcareEnterpriseService) { }

  institution: Institution = null;
  institutionTypes: InstitutionType[] = [];
  institusjontypeId = 0;
  listOfHealthcareEnterprises: HealthcareEnterprise[] = [];

  kommune: Municipality = null;
  kommuner: Municipality[];
  kommuneId = 0;
  UrlPaths = UrlPaths;
  helseforetakId = 0;
  visKommune = false;
  visHelseforetak = false;


  @Input() institutionId: number;
  @Output() institusjonSlettetEvent: EventEmitter<number> = new EventEmitter<number>();
  @Output() institusjonOppdatertEvent: EventEmitter<Institution> = new EventEmitter<Institution>();

  ngOnInit(): void {
    if (this.institutionId === 0) {
      this.institution = null;
      return;
    }
    this.institutionService.getInstitution(this.institutionId).subscribe((institution) => {
      this.institution = institution;
      this.institusjontypeId = institution.institutionType.id;
      this.kommuneId = institution.municipality?.id;
      this.helseforetakId = institution.healthcareEnterprise?.id;
      
      this.institutionService.getInstitutionTypes().subscribe((types) => {
        this.institutionTypes = types;
        this.visKommune = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitusjonstypeKonstanter.Sykehjem;
        this.visHelseforetak = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitusjonstypeKonstanter.Sykehus;
      });
    });

    this.municipalityService.getMunicipalities().subscribe(
      (kommuner) => {
        this.kommuner = kommuner;
    });

    this.healthcareEnterpriseService.getAllHealthcareEnterprises().subscribe(
      (alleHelseforetak) => {
        this.listOfHealthcareEnterprises = alleHelseforetak;
    });
  }

  deleteInstitution() {
      if (this.institutionId > 0) {
        this.institutionService.deleteInstitution(this.institutionId).subscribe(() => {
          this.institution = null;
          this.institusjonSlettetEvent.emit(this.institutionId);
        },
          (error =>
            this.toastrService.error(`En feil oppstod: ${error?.error} / ${error?.message}`, 'Feil under sletting av institution', { disableTimeOut: true})));
      }
  }

  institusjonTypeEndret() {
    this.institution.institutionType = this.institutionTypes.find(i => i.id === this.institusjontypeId);
    this.visKommune = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitusjonstypeKonstanter.Sykehjem;
     this.visHelseforetak = this.institutionTypes.length > 0 && this.institution.institutionType.code == InstitusjonstypeKonstanter.Sykehus;
  }

  kommuneEndret() {
    if (this.kommuneId) {
      this.institution.municipality = this.kommuner.find(r => r.id === this.kommuneId);
    }
  }

  lagreInstitusjon() {
    this.institutionService.updateInstitution(this.institution).subscribe(
      (institution) => {
        this.toastrService.success('Institusjonen ble oppdatert');
        this.institusjonOppdatertEvent.emit(institution);
      },
      (error) => this.toastrService.error(`En feil oppstod: ${error?.error} / ${error?.message}`, 'Feil under oppdatering', { disableTimeOut: true}));
  }

  helseforetakEndret() {
    if (this.helseforetakId) {
      this.institution.healthcareEnterprise = this.listOfHealthcareEnterprises.find(r => r.id === this.helseforetakId);
    }
  }

  kanIkkeLagreInstitusjon(): boolean {
    if (this.institution.institutionType.id > 0 && this.institution.name?.length > 0)
      return false;
    else
      return true;
  }
}

