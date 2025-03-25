import { Component, OnInit, OnDestroy } from '@angular/core';
import { RegionService } from '../../../services/data/region.service';
import { ToastrService } from 'ngx-toastr';
import { Region } from 'src/app/models/api/Region';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-region',
  templateUrl: './redigering-av-region.component.html'
})
export class RedigeringAvRegionComponent implements OnInit, OnDestroy {

  regioner: Region[] = [];
  regionSomEndres: Region = null;
  nyRegion: Region = this.tomRequest();

  constructor(
    private regionService: RegionService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastRegioner();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  lastRegioner() {
    this.regionService.hentRegioner().subscribe(
      (resultat) => this.regioner = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av regioner: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  tomRequest(): Region {
    return {
      id: 0,
      kode: null,
      navn: null
    };
  }

  opprettRegion(): void {
    this.regionService.opprettRegion(this.nyRegion).subscribe(
      (region) => this.toastrService.success(`Region opprettet.`),
      error => this.toastrService.error(`En feil skjedde under opprettelse av region ${this.nyRegion.navn}. Feil: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.nyRegion = this.tomRequest(); this.lastRegioner(); }
    );
  }

  valgtRegion(region: Region): void {
    if (this.regionSomEndres?.id == region.id) return;
    this.regionSomEndres = JSON.parse(JSON.stringify(region));
  }

  oppdaterRegion(region: Region): void {
    this.regionService.oppdaterRegion(region).subscribe(
      (oppdatertRegion) => {
        this.toastrService.success(`Region ${region.id} ble oppdatert`);
        this.lastRegioner();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av region: ' + error?.error, '', { disableTimeOut: true}),
      () => this.regionSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.regionSomEndres = null;
  }
}
