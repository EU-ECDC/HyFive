import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChange, SimpleChanges, OnDestroy} from '@angular/core';
import {OpprettAvdelingRequest} from '../../../models/api/OpprettAvdelingRequest';
import {AvdelingService} from '../../../services/data/avdeling.service';
import {Avdeling} from '../../../models/api/Avdeling';
import {ToastrService} from 'ngx-toastr';
import {Rollevalg} from '../../../models/kodeverk/rollevalg.model';
import {AvdelingType} from '../../../models/api/AvdelingType';
import { Rolle } from '../../../models/api/Rolle';
import { RolleService } from '../../../services/data/rolle.service';

@Component({
  selector: 'app-opprett-avdeling',
  templateUrl: './opprett-avdeling.component.html'
})
export class OpprettAvdelingComponent implements OnInit, OnDestroy {

  nyAvdeling: OpprettAvdelingRequest;
  rollevalg: Rollevalg[] = [];
  avdelingstyper: AvdelingType[];
  roller: Rolle[] = [];

  @Input() institusjonId: number;
  @Output() avdelingOpprettetEvent: EventEmitter<Avdeling> = new EventEmitter<Avdeling>();


  constructor(private rolleService: RolleService, private avdelingService: AvdelingService, private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.nullstillSkjema();
    this.lastAvdelingstyper();
    this.rolleService.hentRoller().subscribe(
      roller => {
      this.roller = roller;
      this.rollevalg = roller.map<Rollevalg>((r) => ({rolle: r, erValgt: true}) );
      },
      error => this.toastrService.error(`Noe gikk galt under lasting av roller for institusjon: ${error?.message ? error.message : error}`, '', { disableTimeOut: true})
    );
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  opprettAvdeling() {
    this.nyAvdeling.rolleIder = this.rollevalg.filter(r => r.erValgt).map(r => r.rolle.id);
    this.avdelingService.opprettAvdeling(this.nyAvdeling).subscribe((avdeling) => {
        this.toastrService.success('Avdeling opprettet', `Avdeling med ID: ${avdeling.id} opprettet`);
        this.rollevalg = this.roller.map<Rollevalg>((r) => ({rolle: r, erValgt: false}) );
        this.avdelingOpprettetEvent.emit(avdeling);
      },
      (error) => this.toastrService.error(`En feil skjedde under opprettelse av avdeling. Feilmelding fra server: ${error?.message ? error.message : error}`, 'Feil under opprettelse av avdeling', { disableTimeOut: true}),
      () => { this.nullstillSkjema();  }
    );
  }

  lastAvdelingstyper() {
    this.avdelingService.hentAvdelingstyper().subscribe(
      (avdelingstyper) => {
        this.avdelingstyper = avdelingstyper;
      },
      (err) => this.toastrService.error(`Kunne ikke laste inn roller: ${err?.message ? err.message : err}`, 'Teknisk feil', { disableTimeOut: true})
    );
  }

  nullstillSkjema() {
    this.nyAvdeling = {
      navn: null,
      institusjonId: this.institusjonId,
      avdelingTypeId: 0,
      rolleIder: []
    };
  }

  kanIkkeOppretteAvdeling(): boolean {
    return this.kanOppretteAvdeling() === false;
  }

  kanOppretteAvdeling(): boolean{
    return this.nyAvdeling.institusjonId > 0
      && this.nyAvdeling.avdelingTypeId > 0
      && this.rollevalg?.filter(r => r.erValgt)?.length > 0
      && this.nyAvdeling.navn?.length > 0;
  }

}
