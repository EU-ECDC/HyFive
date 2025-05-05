import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ProtectiveEquipmentCodingService } from '../../services/data/protectiveEquipment-coding-service';
import { ProtectiveEquipmentSettingType } from '../../models/api/ProtectiveEquipmentSettingType';
import { ProtectiveEquipmentSessionService } from '../../services/data/protectiveEquipment-session.service';
import { Department } from '../../models/api/Department';
import { Urls } from '../../constants/urls';
import { Router } from '@angular/router';
import { ProtectiveEquipmentSettingMapper } from '../../utils/ProtectiveEquipment-setting-mapper';
import { faCircle } from '@fortawesome/free-solid-svg-icons';
import { RoleSelected } from '../../models/registration/roleSelected.model';
import { ProtectiveEquipmentSessionView } from '../../models/registration/protectiveEquipment-sessionView.model';
import { ProtectiveEquipmentType } from '../../models/api/ProtectiveEquipmentType';
import { ProtectiveEquipmentTypeConstants } from '../../models/api/ProtectiveEquipmentTypeConstants';

@Component({
  selector: 'app-valg-for-beskyttelsesutstyr',
  templateUrl: './valg-for-beskyttelsesutstyr.component.html'
})
export class ValgForBeskyttelsesutstyrComponent implements OnInit {

  settinger: ProtectiveEquipmentSettingType[];
  valgtSetting: ProtectiveEquipmentSettingType;
  harPredefinertUtstyr = false;
  faCircle = faCircle;
  beskyttelsesutstyrsettingMapper = ProtectiveEquipmentSettingMapper;


  @Input("sessionView") sessionView: ProtectiveEquipmentSessionView = null;
  @Input("roles") roles: RoleSelected[];
  @Input("department") department: Department;
  @Output("settingOgUtstyrBleEndret") settingOgUtstyrBleEndret: EventEmitter<ProtectiveEquipmentSessionView> = new EventEmitter<ProtectiveEquipmentSessionView>();

  constructor(
    private protectiveEquipmentCodingService: ProtectiveEquipmentCodingService,
    private protectiveEquipmentSessionService: ProtectiveEquipmentSessionService,
    private router: Router) { }

  ngOnInit(): void {
    this.protectiveEquipmentCodingService.getProtectiveEquipmentSettings().subscribe(
      (settinger) => {
        this.settinger = settinger;
      }
    );
  }

  endreValgtSetting(setting: ProtectiveEquipmentSettingType) {
    setting.equipmentTypes = this.visUtstyrVedRekkefolge(setting.equipmentTypes);
    setting.equipmentTypes = setting.equipmentTypes.map(u => { u.isRequired = u.isDefault; return u });
    this.valgtSetting = setting;
    this.harPredefinertUtstyr = this.valgtSetting?.equipmentTypes?.filter(u => u.isRequired)?.length > 0
  }

  startObservasjon() {
    var valgteRoller = this.roles.filter(roleSelected => roleSelected.isSelected).map(roleSelected => roleSelected.role);
    var sessionId = this.protectiveEquipmentSessionService.createSessionView(valgteRoller, this.department, this.valgtSetting);
    this.router.navigate([Urls.RegisterProtectiveEquipmentUrl], { queryParams: { sessionId: sessionId } });
  }

  endreSettingOgUtstyr() {
    this.sessionView.setting = this.valgtSetting;
    this.sessionView.card = this.sessionView.card.map(k => { k.equipment = this.valgtSetting.equipmentTypes; return k });
    this.settingOgUtstyrBleEndret.emit(this.sessionView);
  }

  visUtstyrVedRekkefolge(beskyttelsesutstyrTyper: ProtectiveEquipmentType[]): ProtectiveEquipmentType[] {
    let beskyttelsesutstyrTyperVedRekkefolge: ProtectiveEquipmentType[] = [];

    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.Gloves)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.Gloves)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.PlasticApron)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.PlasticApron)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.CareGown)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.CareGown)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.InfectionGown)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.InfectionGown)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.FaceMask)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.FaceMask)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.RespiratoryProtection)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.RespiratoryProtection)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.EyeProtection)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.EyeProtection)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === ProtectiveEquipmentTypeConstants.Hood)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === ProtectiveEquipmentTypeConstants.Hood)[0]);
    }

    return beskyttelsesutstyrTyperVedRekkefolge;
  }
}

