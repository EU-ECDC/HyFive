import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProtectiveEquipmentTypeqsService } from 'src/app/services/data/protectiveEquipmentTypeqs.service';
import { ProtectiveEquipmentTypeq } from '../../../models/api/ProtectiveEquipmentTypeq';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-beskyttelsesutstyrtyper',
  templateUrl: './redigering-av-beskyttelsesutstyrtyper.component.html'
})
export class RedigeringAvBeskyttelsesutstyrtyperComponent implements OnInit, OnDestroy {

  utstyrtyper: ProtectiveEquipmentTypeq[] = [];
  utstyrtypeSomEndres: ProtectiveEquipmentTypeq = null;
  visRedigerAvBeskyttelsesutstyrtyper: boolean;

  constructor(
    private protectiveEquipmentTypeqsService: ProtectiveEquipmentTypeqsService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.visRedigerAvBeskyttelsesutstyrtyper)
        this.cancelEdit();
    });

    this.visRedigerAvBeskyttelsesutstyrtyper = true;
    this.lastBeskyttelsesutstyrtyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastBeskyttelsesutstyrtyper() {
    this.protectiveEquipmentTypeqsService.getProtectiveEquipmentTypes().subscribe(
      (resultat) => this.utstyrtyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Beskyttelsesutstyrtyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtUtstyrtype(utstyrtype: ProtectiveEquipmentTypeq): void {
    if (this.utstyrtypeSomEndres?.id == utstyrtype.id) return;
    this.utstyrtypeSomEndres = JSON.parse(JSON.stringify(utstyrtype));
  }

  oppdaterUtstyrtype(utstyrtype: ProtectiveEquipmentTypeq): void {
    this.protectiveEquipmentTypeqsService.updateProtectiveEquipmentTypes(utstyrtype).subscribe(
      (oppdatertUtstyrtype) => {
        this.toastrService.success("Beskyttelsesutstyrtype oppdatert");
        this.lastBeskyttelsesutstyrtyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Beskyttelsesutstyrtype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.utstyrtypeSomEndres = null
    );
  }

  navigerTilFeilbruktyper(utstyrtype: ProtectiveEquipmentTypeq): void {
    this.visRedigerAvBeskyttelsesutstyrtyper = false;
    this.utstyrtypeSomEndres = utstyrtype;
  }

  navigerTilBeskyttelsesutstyrtyper(visRedigerAvBeskyttelsesutstyrtyper: boolean): void {
    this.visRedigerAvBeskyttelsesutstyrtyper = visRedigerAvBeskyttelsesutstyrtyper;
    this.utstyrtypeSomEndres = null;
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.utstyrtypeSomEndres = null;
  }
}
