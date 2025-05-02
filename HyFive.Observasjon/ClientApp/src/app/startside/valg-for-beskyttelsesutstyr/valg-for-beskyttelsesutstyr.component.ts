import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BeskyttelsesutstyrKodeverkService } from '../../services/data/beskyttelsesutstyr-kodeverk.service';
import { ProtectiveEquipmentSettingType } from '../../models/api/ProtectiveEquipmentSettingType';
import { BeskyttelsesutstyrSesjonService } from '../../services/data/beskyttelsesutstyr-sesjon.service';
import { Department } from '../../models/api/Department';
import { Urls } from '../../konstanter/urls';
import { Router } from '@angular/router';
import { BeskyttelsesutstyrsettingMapper } from '../../utils/beskyttelsesutstyrsetting-mapper';
import { faCircle } from '@fortawesome/free-solid-svg-icons';
import { Rollevalg } from '../../models/registrering/rollevalg.model';
import { BeskyttelsesutstyrSesjonsvisning } from '../../models/registrering/beskyttelsesutstyr-sesjonsvisning.model';
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
  beskyttelsesutstyrsettingMapper = BeskyttelsesutstyrsettingMapper;


  @Input("sesjonsvisning") sesjonsvisning: BeskyttelsesutstyrSesjonsvisning = null;
  @Input("roles") roles: Rollevalg[];
  @Input("department") department: Department;
  @Output("settingOgUtstyrBleEndret") settingOgUtstyrBleEndret: EventEmitter<BeskyttelsesutstyrSesjonsvisning> = new EventEmitter<BeskyttelsesutstyrSesjonsvisning>();

  constructor(
    private beskyttelsesutstyrKodeverkService: BeskyttelsesutstyrKodeverkService,
    private beskyttelsesutstyrSesjonService: BeskyttelsesutstyrSesjonService,
    private router: Router) { }

  ngOnInit(): void {
    this.beskyttelsesutstyrKodeverkService.hentBeskyttelsesutstyrSettinger().subscribe(
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
    var valgteRoller = this.roles.filter(rollevalg => rollevalg.erValgt).map(rollevalg => rollevalg.role);
    var sessionId = this.beskyttelsesutstyrSesjonService.lagSesjonsvisning(valgteRoller, this.department, this.valgtSetting);
    this.router.navigate([Urls.RegistrereBeskyttelsesutstyrUrl], { queryParams: { sessionId: sessionId } });
  }

  endreSettingOgUtstyr() {
    this.sesjonsvisning.setting = this.valgtSetting;
    this.sesjonsvisning.kort = this.sesjonsvisning.kort.map(k => { k.utstyr = this.valgtSetting.equipmentTypes; return k });
    this.settingOgUtstyrBleEndret.emit(this.sesjonsvisning);
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

