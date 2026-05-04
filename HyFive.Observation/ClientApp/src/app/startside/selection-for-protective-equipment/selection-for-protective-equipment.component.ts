import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ProtectiveEquipmentCodingService } from '../../services/data/protectiveEquipment-coding-service';
import { ProtectiveEquipmentSettingType } from '../../models/api/ProtectiveEquipmentSettingType';
import { ProtectiveEquipmentSessionService } from '../../services/data/protectiveEquipment-session.service';
import { Urls } from '../../constants/urls';
import { Router } from '@angular/router';
import { ProtectiveEquipmentSettingMapper } from '../../utils/ProtectiveEquipment-setting-mapper';
import { faCircle } from '@fortawesome/free-solid-svg-icons';
import { RoleSelected } from '../../models/registration/roleSelected.model';
import { ProtectiveEquipmentSessionView } from '../../models/registration/protectiveEquipment-sessionView.model';
import { ProtectiveEquipmentType } from '../../models/api/ProtectiveEquipmentType';
import { ProtectiveEquipmentTypeConstants } from '../../models/api/ProtectiveEquipmentTypeConstants';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';

@Component({
  selector: 'app-selection-for-protective-equipment',
  templateUrl: './selection-for-protective-equipment.component.html'
})
export class SelectionForProtectiveEquipmentComponent implements OnInit {

  settings: ProtectiveEquipmentSettingType[];
  selectedSetting: ProtectiveEquipmentSettingType;
  hasPredefinedEquipment = false;
  faCircle = faCircle;
  protectiveEquipmentSettingMapper = ProtectiveEquipmentSettingMapper;


  @Input() sessionView: ProtectiveEquipmentSessionView = null;
  @Input() roles: RoleSelected[];
  @Input() department: OrganisationUnit;
  @Output() settingEquipmentWasChanged: EventEmitter<ProtectiveEquipmentSessionView> = new EventEmitter<ProtectiveEquipmentSessionView>();

  constructor(
    private readonly protectiveEquipmentCodingService: ProtectiveEquipmentCodingService,
    private readonly protectiveEquipmentSessionService: ProtectiveEquipmentSessionService,
    private readonly router: Router) { }

  ngOnInit(): void {
    this.protectiveEquipmentCodingService.getProtectiveEquipmentSettings().subscribe(
      (settings) => {
        this.settings = settings;
      }
    );
  }

  changeSelectedSetting(setting: ProtectiveEquipmentSettingType) {
    setting.equipmentTypes = this.showEquipmentByOrder(setting.equipmentTypes);
    setting.equipmentTypes = setting.equipmentTypes.map(u => { u.isRequired = u.isDefault; return u });
    this.selectedSetting = setting;
    this.hasPredefinedEquipment = this.selectedSetting?.equipmentTypes?.filter(u => u.isRequired)?.length > 0
  }

  startObservation() {
    let selectedRoles = this.roles.filter(roleSelected => roleSelected.isSelected).map(roleSelected => roleSelected.role);
    let sessionId = this.protectiveEquipmentSessionService.createSessionView(selectedRoles, this.department, this.selectedSetting);
    this.router.navigate([Urls.RegisterProtectiveEquipmentUrl], { queryParams: { sessionId: sessionId } });
  }

  changeSettingsAndEquipment() {
    this.sessionView.setting = this.selectedSetting;
    this.sessionView.card = this.sessionView.card.map(k => { k.equipment = this.selectedSetting.equipmentTypes; return k });
    this.settingEquipmentWasChanged.emit(this.sessionView);
  }

  showEquipmentByOrder(protectiveEquipmentTypes: ProtectiveEquipmentType[]): ProtectiveEquipmentType[] {
    let protectiveEquipmentTypesByOrder: ProtectiveEquipmentType[] = [];

    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.Gloves)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.Gloves)[0]);
    }
    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.PlasticApron)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.PlasticApron)[0]);
    }
    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.CareGown)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.CareGown)[0]);
    }
    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.InfectionGown)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.InfectionGown)[0]);
    }
    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.FaceMask)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.FaceMask)[0]);
    }
    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.RespiratoryProtection)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.RespiratoryProtection)[0]);
    }
    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.EyeProtection)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.EyeProtection)[0]);
    }
    if (protectiveEquipmentTypes.find(b => b.code === ProtectiveEquipmentTypeConstants.Hood)) {
      protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(b => b.code === ProtectiveEquipmentTypeConstants.Hood)[0]);
    }

    return protectiveEquipmentTypesByOrder;
  }
}

