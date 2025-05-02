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
import { BeskyttelsesutstyrTypeKonstanter } from '../../models/api/BeskyttelsesutstyrTypeKonstanter';

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
    var sesjonId = this.beskyttelsesutstyrSesjonService.lagSesjonsvisning(valgteRoller, this.department, this.valgtSetting);
    this.router.navigate([Urls.RegistrereBeskyttelsesutstyrUrl], { queryParams: { sesjonId: sesjonId } });
  }

  endreSettingOgUtstyr() {
    this.sesjonsvisning.setting = this.valgtSetting;
    this.sesjonsvisning.kort = this.sesjonsvisning.kort.map(k => { k.utstyr = this.valgtSetting.equipmentTypes; return k });
    this.settingOgUtstyrBleEndret.emit(this.sesjonsvisning);
  }

  visUtstyrVedRekkefolge(beskyttelsesutstyrTyper: ProtectiveEquipmentType[]): ProtectiveEquipmentType[] {
    let beskyttelsesutstyrTyperVedRekkefolge: ProtectiveEquipmentType[] = [];

    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Hansker)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Hansker)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Plastforkle)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Plastforkle)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Stellefrakk)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Stellefrakk)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Smittefrakk)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Smittefrakk)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Munnbind)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Munnbind)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Andedrettsvern)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Andedrettsvern)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.code === BeskyttelsesutstyrTypeKonstanter.Hette)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.code === BeskyttelsesutstyrTypeKonstanter.Hette)[0]);
    }

    return beskyttelsesutstyrTyperVedRekkefolge;
  }
}

