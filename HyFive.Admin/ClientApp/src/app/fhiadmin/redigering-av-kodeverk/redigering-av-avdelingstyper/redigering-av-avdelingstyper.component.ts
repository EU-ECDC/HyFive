import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AvdelingType } from '../../../models/api/AvdelingType';
import { AvdelingService } from '../../../services/data/avdeling.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-avdelingstyper',
  templateUrl: './redigering-av-avdelingstyper.component.html'
})
export class RedigeringAvAvdelingstyperComponent implements OnInit, OnDestroy {

  avdelingtyper: AvdelingType[] = [];
  nyAvdelingType: AvdelingType = this.tomRequest();
  avdelingtypeSomEndres: AvdelingType = null;

  constructor(
    private avdelingService: AvdelingService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastAvdelingTyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastAvdelingTyper() {
    this.avdelingService.hentAvdelingstyper().subscribe(
      (resultat) => this.avdelingtyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av avdelingtyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  tomRequest(): AvdelingType {
    return {
      id: 0,
      kode: null,
      navn: null
    };
  }

  opprettAvdelingType() {
    this.avdelingService.opprettAvdelingType(this.nyAvdelingType).subscribe(
      (avdelingstype) => this.toastrService.success(`Avdelingtype med navn ${avdelingstype.navn} ble opprettet`),
      error => this.toastrService.error(`En feil skjedde under opprettelse av avdelingtype ${this.nyAvdelingType.navn}. Feil: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.nyAvdelingType = this.tomRequest(); this.lastAvdelingTyper(); }
    );
  }

  valgtAvdelingType(avdelingstype: AvdelingType): void {
    if (this.avdelingtypeSomEndres?.id == avdelingstype.id) return;
    this.avdelingtypeSomEndres = JSON.parse(JSON.stringify(avdelingstype));
  }

  oppdaterAvdelingType(avdelingstype: AvdelingType): void {
    this.avdelingService.oppdaterAvdelingType(avdelingstype).subscribe(
      (result) => {
        this.toastrService.success('Avdelingtype ble oppdatert');
        this.lastAvdelingTyper();
      },
      (error) => {
        this.toastrService.error('En feil skjedde under oppdatering av avdelingtype: ' + error?.error, '', { disableTimeOut: true});
      },
      () => this.avdelingtypeSomEndres = null
    );
  }
  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.avdelingtypeSomEndres = null;
  }
}
