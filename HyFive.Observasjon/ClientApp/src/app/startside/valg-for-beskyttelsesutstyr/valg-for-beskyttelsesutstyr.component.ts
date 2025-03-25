import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BeskyttelsesutstyrKodeverkService } from '../../services/data/beskyttelsesutstyr-kodeverk.service';
import { BeskyttelsesutstyrsettingType } from '../../models/api/BeskyttelsesutstyrsettingType';
import { BeskyttelsesutstyrSesjonService } from '../../services/data/beskyttelsesutstyr-sesjon.service';
import { Avdeling } from '../../models/api/Avdeling';
import { Urls } from '../../konstanter/urls';
import { Router } from '@angular/router';
import { BeskyttelsesutstyrsettingMapper } from '../../utils/beskyttelsesutstyrsetting-mapper';
import { faCircle } from '@fortawesome/free-solid-svg-icons';
import { Rollevalg } from '../../models/registrering/rollevalg.model';
import { BeskyttelsesutstyrSesjonsvisning } from '../../models/registrering/beskyttelsesutstyr-sesjonsvisning.model';
import { BeskyttelsesutstyrType } from '../../models/api/BeskyttelsesutstyrType';
import { BeskyttelsesutstyrTypeKonstanter } from '../../models/api/BeskyttelsesutstyrTypeKonstanter';

@Component({
  selector: 'app-valg-for-beskyttelsesutstyr',
  templateUrl: './valg-for-beskyttelsesutstyr.component.html'
})
export class ValgForBeskyttelsesutstyrComponent implements OnInit {

  settinger: BeskyttelsesutstyrsettingType[];
  valgtSetting: BeskyttelsesutstyrsettingType;
  harPredefinertUtstyr = false;
  faCircle = faCircle;
  beskyttelsesutstyrsettingMapper = BeskyttelsesutstyrsettingMapper;


  @Input("sesjonsvisning") sesjonsvisning: BeskyttelsesutstyrSesjonsvisning = null;
  @Input("roller") roller: Rollevalg[];
  @Input("avdeling") avdeling: Avdeling;
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

  endreValgtSetting(setting: BeskyttelsesutstyrsettingType) {
    setting.utstyrstyper = this.visUtstyrVedRekkefolge(setting.utstyrstyper);
    setting.utstyrstyper = setting.utstyrstyper.map(u => { u.erIndikert = u.erDefaultIndikert; return u });
    this.valgtSetting = setting;
    this.harPredefinertUtstyr = this.valgtSetting?.utstyrstyper?.filter(u => u.erIndikert)?.length > 0
  }

  startObservasjon() {
    var valgteRoller = this.roller.filter(rollevalg => rollevalg.erValgt).map(rollevalg => rollevalg.rolle);
    var sesjonId = this.beskyttelsesutstyrSesjonService.lagSesjonsvisning(valgteRoller, this.avdeling, this.valgtSetting);
    this.router.navigate([Urls.RegistrereBeskyttelsesutstyrUrl], { queryParams: { sesjonId: sesjonId } });
  }

  endreSettingOgUtstyr() {
    this.sesjonsvisning.setting = this.valgtSetting;
    this.sesjonsvisning.kort = this.sesjonsvisning.kort.map(k => { k.utstyr = this.valgtSetting.utstyrstyper; return k });
    this.settingOgUtstyrBleEndret.emit(this.sesjonsvisning);
  }

  visUtstyrVedRekkefolge(beskyttelsesutstyrTyper: BeskyttelsesutstyrType[]): BeskyttelsesutstyrType[] {
    let beskyttelsesutstyrTyperVedRekkefolge: BeskyttelsesutstyrType[] = [];

    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Hansker)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Hansker)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Plastforkle)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Plastforkle)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Stellefrakk)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Stellefrakk)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Smittefrakk)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Smittefrakk)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Munnbind)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Munnbind)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Andedrettsvern)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Andedrettsvern)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse)[0]);
    }
    if (beskyttelsesutstyrTyper.find(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Hette)) {
      beskyttelsesutstyrTyperVedRekkefolge.push(beskyttelsesutstyrTyper.filter(b => b.kode === BeskyttelsesutstyrTypeKonstanter.Hette)[0]);
    }

    return beskyttelsesutstyrTyperVedRekkefolge;
  }
}

