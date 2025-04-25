import {Component, EventEmitter, OnInit, Output, OnDestroy} from '@angular/core';
import { InstitutionType } from '../../../models/api/InstitutionType';
import { CreateInstitutionRequest } from '../../../models/api/CreateInstitutionRequest';
import { InstitutionService } from '../../../services/data/institution.service';
import { ToastrService } from 'ngx-toastr';
import { Institution } from '../../../models/api/Institution';
import { HealthcareEnterprise } from 'src/app/models/api/HealthcareEnterprise';
import { HealthcareEnterpriseService } from 'src/app/services/data/healthcareEnterprise.service';
import { InstitusjonstypeKonstanter } from 'src/app/models/api/InstitusjonstypeKonstanter';
import { Municipality } from 'src/app/models/api/Municipality';
import { MunicipalityService } from 'src/app/services/data/municipality.service';

@Component({
  selector: 'app-opprett-institusjon',
  templateUrl: './opprett-institusjon.component.html'
})
export class OpprettInstitusjonComponent implements OnInit, OnDestroy {

  institusjonstyper: InstitutionType[] = [];
  nyInstitusjon: CreateInstitutionRequest = null;
  kommuner: Municipality[] = [];
  listOfHealthcareEnterprises: HealthcareEnterprise[] = [];
  visHelseforetak: boolean = false;
  visKommune: boolean = false;

  @Output() institusjonOpprettetEvent: EventEmitter<Institution> = new EventEmitter<Institution>();

  constructor(private institutionService: InstitutionService, private toastrService: ToastrService,
              private municipalityService: MunicipalityService, 
              private healthcareEnterpriseService: HealthcareEnterpriseService) { }

  ngOnInit(): void {
    this.institutionService.getInstitutionTypes().subscribe((resultat) => {
      this.institusjonstyper = resultat;
      this.nyInstitusjon = this.opprettDefaultInstitusjon();
    });

    this.municipalityService.getMunicipalities().subscribe(
      (kommuner) => {
        this.kommuner = kommuner;
      }
    );

    this.healthcareEnterpriseService.getAllHealthcareEnterprises().subscribe(
      (alleHelseforetak) => {
        this.listOfHealthcareEnterprises = alleHelseforetak;
      }
    );
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  finnDefaultInstitusjonstype() {
    return this.institusjonstyper.find(p => p.code === InstitusjonstypeKonstanter.Sykehus);
  }

  createInstitution() {
    this.institutionService.createInstitution(this.nyInstitusjon).subscribe((resultat) => {
        this.toastrService.success('Institution opprettet', `Institution med ID: ${resultat.id} opprettet`);
        this.institusjonOpprettetEvent.emit(resultat);
      },
        (err) => this.toastrService.error(`En feil skjedde under opprettelse av institusjon. Feilmelding fra server: ${err}`, 'Feil under opprettelse av institusjon', { disableTimeOut: true }),
      () => {
        this.nyInstitusjon = this.opprettDefaultInstitusjon();

      }
    );
  }

 private opprettDefaultInstitusjon(): CreateInstitutionRequest {
    let defaultInstitusjonType = this.finnDefaultInstitusjonstype();
    this.visHelseforetakEllerRegion(defaultInstitusjonType.id);
    return {
      institusjonsnavn: null,
      institusjonTypeId: defaultInstitusjonType.id,
      koordinatorEtternavn: null,
      koordinatorFornavn: null,
      koordinatorHPRnummer: null,
      koordinatorEpost: null,
      koordinatorPseudonym: null,
      herId: null,
      forkortelse: null,
      regionId: 0,
      kommuneId: 0,
      helseforetakId: 0
    };    
  }

  kanIkkeOppretteInstitusjon(): boolean {
    return this.kanOppretteInstitusjon() === false;
  }

  kanOppretteInstitusjon(): boolean{
    return this.nyInstitusjon?.institusjonsnavn?.length > 0
      && this.nyInstitusjon?.koordinatorHPRnummer?.length > 0
      && this.nyInstitusjon?.koordinatorFornavn?.length > 0
      && this.nyInstitusjon?.koordinatorEtternavn?.length > 0;
  }

  visHelseforetakEllerRegion(institusjonTypeId: number)
  {
    var valgtInstitusjonstype = this.institusjonstyper.find(i => i.id === institusjonTypeId);
    if(valgtInstitusjonstype.code === InstitusjonstypeKonstanter.Sykehus)
    {
      this.visHelseforetak = true;
      this.visKommune = false;
    }
    else if(valgtInstitusjonstype.code === InstitusjonstypeKonstanter.Sykehjem)
    {
      this.visKommune = true;
      this.visHelseforetak = false;
    }
    else
    {
      this.visHelseforetak = false;
      this.visKommune = false;
    }
  }
}
