import { Component, OnInit, Output, EventEmitter, Input, OnDestroy } from '@angular/core';
import { MisuseType } from '../../../../models/api/MisuseType';
import { ProtectiveEquipmentTypeq } from '../../../../models/api/ProtectiveEquipmentTypeq';
import { ProtectiveEquipmentTypeqsService } from '../../../../services/data/protectiveEquipmentTypeqs.service';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { OpprettFeilbrukTypeRequest } from '../../../../models/api/OpprettFeilbrukTypeRequest';
import { KeyEventService } from '../../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-misuse-types',
  templateUrl: './editing-of-misuse-types.component.html'
})
export class EditingMisuseTypesComponent implements OnInit, OnDestroy {

  nyFeilbruktype: OpprettFeilbrukTypeRequest = this.emptyRequest();
  feilbruktyper: MisuseType[] = [];
  feilbruktypeSomEndres: MisuseType = null;

  faChevronLeft = faChevronLeft;

  @Input() equipmentType: ProtectiveEquipmentTypeq;
  @Output() visRedigeringAvBeskyttelsesutstyrtyperEvent: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private protectiveEquipmentTypeqsService: ProtectiveEquipmentTypeqsService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.lastFeilbruktyper();
  }

  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastFeilbruktyper() {
    this.protectiveEquipmentTypeqsService.getMisuseTypes(this.equipmentType.id).subscribe(
      (feilbruktyper) => this.feilbruktyper = feilbruktyper,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Feilbruktyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): OpprettFeilbrukTypeRequest {
    return {
      name: null
    };
  }

  createMisuseType(): void {
    this.protectiveEquipmentTypeqsService.createMisuseType(this.equipmentType.id, this.nyFeilbruktype).subscribe(
      (opprettetFeilbruktype) => this.toastrService.success(`Feilbruktype opprettet.`),
      error => this.toastrService.error(`An error occurred while creating feilbruktype ${this.nyFeilbruktype.name}. Error: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.nyFeilbruktype = this.emptyRequest(); this.lastFeilbruktyper(); }
    );
  }

  valgtFeilbruktype(feilbruktype: MisuseType) {
    if (this.feilbruktypeSomEndres?.id == feilbruktype.id) return;
    this.feilbruktypeSomEndres = JSON.parse(JSON.stringify(feilbruktype));
  }

  updateMisuseType(feilbruktype: MisuseType): void {
    this.protectiveEquipmentTypeqsService.updateMisuseType(this.equipmentType.id, feilbruktype).subscribe(
      (result) => {
        this.toastrService.success('Feilbruktype was updated');
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

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.feilbruktypeSomEndres = null;
  }
}
