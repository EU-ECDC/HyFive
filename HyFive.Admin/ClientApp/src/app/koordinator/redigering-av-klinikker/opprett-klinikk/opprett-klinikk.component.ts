import { Component, EventEmitter, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { InstitusjonService } from '../../../services/data/institusjon.service';
import { ToastrService } from 'ngx-toastr';
import { KlinikkService } from '../../../services/data/klinikk.service';
import { Klinikk } from '../../../models/api/Klinikk';
import { Avdelingsvalg } from '../../../models/kodeverk/avdelingsvalg.model';
import { AvdelingService } from '../../../services/data/avdeling.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-opprett-klinikk',
  templateUrl: './opprett-klinikk.component.html'
})
export class OpprettKlinikkComponent implements OnInit, OnDestroy {

  nyKlinikk: Klinikk;
  avdelingsvalg: Avdelingsvalg[] = [];

  klinikkerListe: Klinikk[] = [];

  fawarningicon = faExclamationTriangle;

  @Input() institusjonId: number;
  @Output() klinikkOpprettetEvent: EventEmitter<Klinikk> = new EventEmitter<Klinikk>();


  constructor(
    private institusjonService: InstitusjonService,
    private klinikkService: KlinikkService,
    private avdelingService: AvdelingService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.nullstillSkjema();
    this.lastAvdelinger();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  opprettKlinikk() {
    this.nyKlinikk.avdelinger = this.avdelingsvalg
      .filter(r => r.erValgt)
      .map((r) => ({ id: r.avdeling.id, avdelingTypeId: 0, roller: null, institusjonId: this.institusjonId, navn: null, avdelingType: null }));

    this.klinikkService.opprettKlinikk(this.nyKlinikk).subscribe((klinikk) => {
      this.toastrService.success('Klinikk opprettet', `Klinikk med ID: ${klinikk.id} opprettet`);
      this.klinikkOpprettetEvent.emit(klinikk);

      this.lastAvdelinger();
    },
      (error) => this.toastrService.error(`En feil skjedde under opprettelse av klinikk. Feilmelding fra server: ${error?.message ? error.message : error}`, 'Feil under opprettelse av klinikk', { disableTimeOut: true}),
      () => { this.nullstillSkjema(); }
    );
  }

  lastAvdelinger() {

    this.klinikkService.hentKlinikkerForInstitusjon(this.institusjonId).subscribe((result: Klinikk[]) => {
      this.klinikkerListe = result;

      this.institusjonService.hentAvdelinger(this.institusjonId).subscribe(
        (avdelinger) => {
          this.avdelingsvalg = avdelinger.map(a =>
          ({
            avdeling: a, erValgt: false,
            erAlleredePaKlinikk: this.klinikkerListe.some(k => k.avdelinger.some(av => av.id === a.id))
          }));
        },
        (err) => this.toastrService.error(`Kunne ikke laste inn klinikker: ${err?.message ? err.message : err}`, 'Teknisk feil', { disableTimeOut: true})
      );
    });
  }

  nullstillSkjema() {
    this.nyKlinikk = {
      id: 0,
      navn: null,
      institusjonId: this.institusjonId,
      avdelinger: []
    };
    for (const avdeling of this.avdelingsvalg) {
      avdeling.erValgt = false;
    }
  }

  kanIkkeOppretteKlinikk(): boolean {
    return this.kanOppretteKlinikk() === false;
  }

  kanOppretteKlinikk(): boolean {
    return this.nyKlinikk.institusjonId > 0
      && this.avdelingsvalg?.filter(r => r.erValgt)?.length > 0
      && this.nyKlinikk.navn?.length > 0;
  }

}
