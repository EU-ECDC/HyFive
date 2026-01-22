import { Component, OnInit, Output, EventEmitter, Input, OnDestroy } from '@angular/core';
import { MisuseType } from '../../../../models/api/MisuseType';
import { ProtectiveEquipmentTypeq } from '../../../../models/api/ProtectiveEquipmentTypeq';
import { ProtectiveEquipmentTypeqsService } from '../../../../services/data/protectiveEquipmentTypeqs.service';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { CreateMisueTypeRequest } from '../../../../models/api/CreateMisueTypeRequest';
import { KeyEventService } from '../../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-misuse-types',
  templateUrl: './editing-of-misuse-types.component.html'
})
export class EditingMisuseTypesComponent implements OnInit, OnDestroy {

  newMisuseType: CreateMisueTypeRequest = this.emptyRequest();
  misuseType: MisuseType[] = [];
  misusetypeAsChanged: MisuseType = null;

  faChevronLeft = faChevronLeft;

  @Input() equipmentType: ProtectiveEquipmentTypeq;
  @Output() showEditingOfProtectiveEquipmentTypesEvent: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private readonly protectiveEquipmentTypeqsService: ProtectiveEquipmentTypeqsService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadMisuseTypes();
  }

  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadMisuseTypes() {
    this.protectiveEquipmentTypeqsService.getMisuseTypes(this.equipmentType.id).subscribe(
      (misuseType) => this.misuseType = misuseType,
      (error) => this.toastrService.error('An error occurred while loading Misuse Types: ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): CreateMisueTypeRequest {
    return {
      name: null
    };
  }

  createMisuseType(): void {
    this.protectiveEquipmentTypeqsService.createMisuseType(this.equipmentType.id, this.newMisuseType).subscribe(
      (createdMisuseType) => this.toastrService.success(`Misuse Type create.`),
      error => this.toastrService.error(`An error occurred while creating Misuse Type ${this.newMisuseType.name}. Error: "${error.error.message}"`, '', { disableTimeOut: true}),
      () => { this.newMisuseType = this.emptyRequest(); this.loadMisuseTypes(); }
    );
  }

  selectedMisuseType(misusetype: MisuseType) {
    if (this.misusetypeAsChanged?.id == misusetype.id) return;
    this.misusetypeAsChanged = structuredClone(misusetype);
  }

  updateMisuseType(misusetype: MisuseType): void {
    this.protectiveEquipmentTypeqsService.updateMisuseType(this.equipmentType.id, misusetype).subscribe(
      (result) => {
        this.toastrService.success('misuse type was updated');
        this.loadMisuseTypes();
      },
      (error) => {
        this.toastrService.error('An error occurred while updating misusetype: ' + error?.error.message, '', { disableTimeOut: true});
      },
      () => this.misusetypeAsChanged = null
    );
  }

  NavigateToProtectiveEquipmentTypes(): void {
    this.showEditingOfProtectiveEquipmentTypesEvent.emit(true);
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.misusetypeAsChanged = null;
  }
}
