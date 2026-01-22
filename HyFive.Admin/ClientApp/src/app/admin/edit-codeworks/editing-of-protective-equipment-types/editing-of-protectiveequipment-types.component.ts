import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProtectiveEquipmentTypeqsService } from 'src/app/services/data/protectiveEquipmentTypeqs.service';
import { ProtectiveEquipmentTypeq } from '../../../models/api/ProtectiveEquipmentTypeq';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-protectiveequipment-types',
  templateUrl: './editing-of-protectiveequipment-types.component.html'
})
export class EditingProtectiveEquipmentTypeComponent implements OnInit, OnDestroy {

  equipmentTypes: ProtectiveEquipmentTypeq[] = [];
  equipmentTypeAsChanged: ProtectiveEquipmentTypeq = null;
  showEditOfProtectiveEquipmentTypes: boolean;

  constructor(
    private readonly protectiveEquipmentTypeqsService: ProtectiveEquipmentTypeqsService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.showEditOfProtectiveEquipmentTypes)
        this.cancelEdit();
    });

    this.showEditOfProtectiveEquipmentTypes = true;
    this.loadProtectiveEquipmentTypes();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadProtectiveEquipmentTypes() {
    this.protectiveEquipmentTypeqsService.getProtectiveEquipmentTypes().subscribe(
      (result) => this.equipmentTypes = result,
      (error) => this.toastrService.error('An error occurred while loading Protective Equipment Types: ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  selectedEquipmentType(equipmentType: ProtectiveEquipmentTypeq): void {
    if (this.equipmentTypeAsChanged?.id == equipmentType.id) return;
    this.equipmentTypeAsChanged = structuredClone(equipmentType);
  }

  updateProtectiveEquipmentType(equipmentType: ProtectiveEquipmentTypeq): void {
    this.protectiveEquipmentTypeqsService.updateProtectiveEquipmentTypes(equipmentType).subscribe(
      (updatedEquipmenttype) => {
        this.toastrService.success("Protective equipment type updated");
        this.loadProtectiveEquipmentTypes();
      },
      error => this.toastrService.error('An error occurred while updating Protective equipment type: ' + error?.error.message, '', { disableTimeOut: true}),
      () => this.equipmentTypeAsChanged = null
    );
  }

  navigateToMisuseTypes(equipmentType: ProtectiveEquipmentTypeq): void {
    this.showEditOfProtectiveEquipmentTypes = false;
    this.equipmentTypeAsChanged = equipmentType;
  }

  NavigateToProtectiveEquipmentTypes(showEditOfProtectiveEquipmentTypes: boolean): void {
    this.showEditOfProtectiveEquipmentTypes = showEditOfProtectiveEquipmentTypes;
    this.equipmentTypeAsChanged = null;
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.equipmentTypeAsChanged = null;
  }
}
