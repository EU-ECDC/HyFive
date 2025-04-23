import { Component, OnInit, Output, EventEmitter, Input, OnDestroy } from '@angular/core';
import { MisuseType } from '../../../../models/api/MisuseType';
import { ProtectiveEquipmentTypeq } from '../../../../models/api/ProtectiveEquipmentTypeq';
import { ProtectiveEquipmentTypeqsService } from '../../../../services/data/protectiveEquipmentTypeqs.service';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { OpprettFeilbrukTypeRequest } from '../../../../models/api/OpprettFeilbrukTypeRequest';
import { KeyEventService } from '../../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-feilbruktyper',
  templateUrl: './redigering-av-feilbruktyper.component.html'
})
export class RedigeringAvFeilbruktyperComponent implements OnInit, OnDestroy {

  nyFeilbruktype: OpprettFeilbrukTypeRequest = this.tomRequest();
  feilbruktyper: MisuseType[] = [];
  feilbruktypeSomEndres: MisuseType = null;

  faChevronLeft = faChevronLeft;

  @Input() utstyrtype: ProtectiveEquipmentTypeq;
  @Output() visRedigeringAvBeskyttelsesutstyrtyperEvent: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private protectiveEquipmentTypeqsService: ProtectiveEquipmentTypeqsService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastFeilbruktyper();
  }

  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastFeilbruktyper() {
    this.protectiveEquipmentTypeqsService.getMisuseTypes(this.utstyrtype.id).subscribe(
      (feilbruktyper) => this.feilbruktyper = feilbruktyper,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Feilbruktyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  tomRequest(): OpprettFeilbrukTypeRequest {
    return {
      name: null
    };
  }

  createMisuseType(): void {
    this.protectiveEquipmentTypeqsService.createMisuseType(this.utstyrtype.id, this.nyFeilbruktype).subscribe(
      (opprettetFeilbruktype) => this.toastrService.success(`Feilbruktype opprettet.`),
      error => this.toastrService.error(`En feil skjedde under opprettelse av feilbruktype ${this.nyFeilbruktype.name}. Feil: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.nyFeilbruktype = this.tomRequest(); this.lastFeilbruktyper(); }
    );
  }

  valgtFeilbruktype(feilbruktype: MisuseType) {
    if (this.feilbruktypeSomEndres?.id == feilbruktype.id) return;
    this.feilbruktypeSomEndres = JSON.parse(JSON.stringify(feilbruktype));
  }

  updateMisuseType(feilbruktype: MisuseType): void {
    this.protectiveEquipmentTypeqsService.updateMisuseType(this.utstyrtype.id, feilbruktype).subscribe(
      (result) => {
        this.toastrService.success('Feilbruktype ble oppdatert');
        this.lastFeilbruktyper();
      },
      (error) => {
        this.toastrService.error('En feil skjedde under oppdatering av feilbruktype: ' + error?.error, '', { disableTimeOut: true});
      },
      () => this.feilbruktypeSomEndres = null
    );
  }

  navigerTilBeskyttelsesutsyrtyper(): void {
    this.visRedigeringAvBeskyttelsesutstyrtyperEvent.emit(true);
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.feilbruktypeSomEndres = null;
  }
}
