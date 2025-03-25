import {Component, EventEmitter, OnInit, Output, OnDestroy} from '@angular/core';
import { InstitusjonType } from '../../../models/api/InstitusjonType';
import { OpprettInstitusjonRequest } from '../../../models/api/OpprettInstitusjonRequest';
import { InstitusjonService } from '../../../services/data/institusjon.service';
import { ToastrService } from 'ngx-toastr';
import { Institusjon } from '../../../models/api/Institusjon';
import { Helseforetak } from 'src/app/models/api/Helseforetak';
import { HelseforetakService } from 'src/app/services/data/helseforetak.service';
import { InstitusjonstypeKonstanter } from 'src/app/models/api/InstitusjonstypeKonstanter';
import { Kommune } from 'src/app/models/api/Kommune';
import { KommuneService } from 'src/app/services/data/kommune.service';

@Component({
  selector: 'app-opprett-institusjon',
  templateUrl: './opprett-institusjon.component.html'
})
export class OpprettInstitusjonComponent implements OnInit, OnDestroy {

  institusjonstyper: InstitusjonType[] = [];
  nyInstitusjon: OpprettInstitusjonRequest = null;
  kommuner: Kommune[] = [];
  listAvHelseforetak: Helseforetak[] = [];
  visHelseforetak: boolean = false;
  visKommune: boolean = false;

  @Output() institusjonOpprettetEvent: EventEmitter<Institusjon> = new EventEmitter<Institusjon>();

  constructor(private institusjonService: InstitusjonService, private toastrService: ToastrService,
              private kommuneService: KommuneService, 
              private helseforetakService: HelseforetakService) { }

  ngOnInit(): void {
    this.institusjonService.hentInstitusjontyper().subscribe((resultat) => {
      this.institusjonstyper = resultat;
      this.nyInstitusjon = this.opprettDefaultInstitusjon();
    });

    this.kommuneService.hentKommuner().subscribe(
      (kommuner) => {
        this.kommuner = kommuner;
      }
    );

    this.helseforetakService.hentAlleHelseforetak().subscribe(
      (alleHelseforetak) => {
        this.listAvHelseforetak = alleHelseforetak;
      }
    );
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  finnDefaultInstitusjonstype() {
    return this.institusjonstyper.find(p => p.kode === InstitusjonstypeKonstanter.Sykehus);
  }

  opprettInstitusjon() {
    this.institusjonService.opprettInstitusjon(this.nyInstitusjon).subscribe((resultat) => {
        this.toastrService.success('Institusjon opprettet', `Institusjon med ID: ${resultat.id} opprettet`);
        this.institusjonOpprettetEvent.emit(resultat);
      },
        (err) => this.toastrService.error(`En feil skjedde under opprettelse av institusjon. Feilmelding fra server: ${err}`, 'Feil under opprettelse av institusjon', { disableTimeOut: true }),
      () => {
        this.nyInstitusjon = this.opprettDefaultInstitusjon();

      }
    );
  }

 private opprettDefaultInstitusjon(): OpprettInstitusjonRequest {
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
    if(valgtInstitusjonstype.kode === InstitusjonstypeKonstanter.Sykehus)
    {
      this.visHelseforetak = true;
      this.visKommune = false;
    }
    else if(valgtInstitusjonstype.kode === InstitusjonstypeKonstanter.Sykehjem)
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
