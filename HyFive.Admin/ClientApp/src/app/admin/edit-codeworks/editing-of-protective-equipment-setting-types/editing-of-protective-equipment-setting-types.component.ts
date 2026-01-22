import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProtectiveEquipmentSettingType } from 'src/app/models/api/ProtectiveEquipmentSettingType';
import { ProtectiveEquipmentSettingTypesService } from '../../../services/data/protectiveEquipmentSettingTypes.service';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-protective-equipment-setting-types',
  templateUrl: './editing-of-protective-equipment-setting-types.component.html'
})
export class EditingOfProtectiveEquipmentSettingTypesComponent implements OnInit, OnDestroy {

  protectiveequipmentSettingTypes: ProtectiveEquipmentSettingType[];
  settingTypeAsChanged: ProtectiveEquipmentSettingType = null;

  constructor(
    private readonly protectiveEquipmentSettingTypesService: ProtectiveEquipmentSettingTypesService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService
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
    this.protectiveEquipmentSettingTypesService.getProtectiveEquipmentSettingTypes().subscribe(
      (settingtyper) => this.protectiveequipmentSettingTypes = settingtyper,
      (error) => this.toastrService.error('An error occurred while loading Protective Equipment setting Types: ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  selectedSettingType(settingType: ProtectiveEquipmentSettingType): void {
    if (this.settingTypeAsChanged?.id == settingType.id) return;
    this.settingTypeAsChanged = structuredClone(settingType);
  }

  updateSettingType(settingType: ProtectiveEquipmentSettingType): void {
    this.protectiveEquipmentSettingTypesService.updateProtectiveEquipmentSettingType(settingType).subscribe(
      (updatedProtectiveEquipmentSettingType) => {
        this.toastrService.success("ProtectiveEquipmentType was updated");
        this.loadSettingtypes();
      },
      error => this.toastrService.error('An error occurred while updating ProtectiveEquipmentType: ' + error?.error.message, '', { disableTimeOut: true}),
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
