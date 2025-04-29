import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProtectiveEquipmentType } from 'src/app/models/api/ProtectiveEquipmentType';
import { ProtectiveEquipmentSettingTypesService } from '../../../services/data/protectiveEquipmentSettingTypes.service';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-protective-equipment-setting-types',
  templateUrl: './editing-of-protective-equipment-setting-types.component.html'
})
export class EditingOfProtectiveEquipmentSettingTypesComponent implements OnInit, OnDestroy {

  protectiveequipmentSettingTypes: ProtectiveEquipmentType[];
  settingTypeAsChanged: ProtectiveEquipmentType = null;

  constructor(
    private protectiveEquipmentSettingTypesService: ProtectiveEquipmentSettingTypesService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadSettingtypes();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadSettingtypes() {
    this.protectiveEquipmentSettingTypesService.getProtectiveEquipmentTypes().subscribe(
      (settingtyper) => this.protectiveequipmentSettingTypes = settingtyper,
      (error) => this.toastrService.error('An error occurred while loading Protective Equipment setting Types: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  selectedSettingType(settingType: ProtectiveEquipmentType): void {
    if (this.settingTypeAsChanged?.id == settingType.id) return;
    this.settingTypeAsChanged = JSON.parse(JSON.stringify(settingType));
  }

  updateSettingType(settingType: ProtectiveEquipmentType): void {
    this.protectiveEquipmentSettingTypesService.updateProtectiveEquipmentSettingType(settingType).subscribe(
      (updatedProtectiveEquipmentSettingType) => {
        this.toastrService.success("ProtectiveEquipmentType was updated");
        this.loadSettingtypes();
      },
      error => this.toastrService.error('An error occurred while updating ProtectiveEquipmentType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.settingTypeAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.settingTypeAsChanged = null;
  }
}
