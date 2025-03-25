import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Avdeling } from '../../models/api/Avdeling';
import { HandsmykkeObservasjon } from '../../models/api/HandsmykkeObservasjon';
import { HandsmykkeType } from '../../models/api/HandsmykkeType';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { faCommentDots } from '@fortawesome/free-regular-svg-icons';
import { HandsmykkeSesjonService } from '../../services/data/handsmykke-sesjon.service';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { Handsmykkevalg } from '../../models/registrering/handsmykkevalg.model';
import { HandsmykkeMapper } from '../../utils/handsmykke-mapper';
import { Dialogtekster } from 'src/app/konstanter/dialogtekster';
import { Farger } from '../../utils/farger';
import { Rolle } from '../../models/api/Rolle';
import { HandsmykkeTypeKonstanter } from '../../models/api/HandsmykkeTypeKonstanter';
import { HandsmykkeTypeService } from '../../services/data/handsmykketype.service';

@Component({
  selector: 'app-rediger-handsmykker-observasjon',
  templateUrl: './rediger-handsmykker-observasjon.component.html'
})
export class RedigerHandsmykkerObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  handsmykkevalg = [] as Handsmykkevalg[];
  handsmykkeTyper: HandsmykkeType[] = [];
  ikonTypeMap: Map<HandsmykkeTypeKonstanter, IconProp> = HandsmykkeMapper.getIkontypeMap();
  Farger = Farger;
  Dialogtekster = Dialogtekster;

  faCircle = faCircle;
  faCheck = faCheck;
  faCommentLines = faCommentDots;
  faSave = faSave;
  faTrashAlt = faTrashAlt;

  constructor(
    private sesjonService: HandsmykkeSesjonService,
    private handsmykkeTypeService: HandsmykkeTypeService
  ) { }

  @Input() isReadonly: boolean = false;
  @Input() observasjon: HandsmykkeObservasjon;
  @Input() avdeling: Avdeling;
  @Output() observasjonSlettetEvent = new EventEmitter();


  ngOnInit(): void {
    this.handsmykkeTypeService.getHandsmykkeTyper().subscribe((handsmykkeTyper) => {
      this.handsmykkeTyper = handsmykkeTyper;
      this.handsmykkevalg = HandsmykkeMapper.getHandsmykkevalg(this.handsmykkeTyper, this.observasjon.handsmykker.map(x => x?.kode));
      this.handsmykkevalg.forEach(x => this.changed(x));
    });
  }

  antallValgteHandsmykker() {
    return this.handsmykkevalg.reduce((acc, curr) => { if (curr.erValgt) return acc + 1; return acc; }, 0);
  }

  changed(valg: Handsmykkevalg) {
    if (valg.erValgt && valg.type == HandsmykkeTypeKonstanter.AltOk)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { if (x.type !== HandsmykkeTypeKonstanter.AltOk) { x.disabled = true; x.erValgt = false; } return x; }) // disable all
    else if (valg.erValgt && valg.type != HandsmykkeTypeKonstanter.AltOk)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { if (x.type === HandsmykkeTypeKonstanter.AltOk) { x.disabled = true; x.erValgt = false; } return x; }) // disable altok
    else if (this.antallValgteHandsmykker() < 1)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { x.disabled = false; return x; }) // enable all
  }

  lagreObservasjon() {
    this.observasjon.handsmykker = this.handsmykkevalg.reduce((acc, item) => {
      if (item.erValgt) acc.push(this.handsmykkeTyper.find(x => x.kode === item.type));
      return acc;
    }, [] as HandsmykkeType[]) as HandsmykkeType[];
    this.sesjonService.endreObservasjon(this.observasjon);
    this.erRedigeringsmodus = false;
  }

  slettObservasjon() {
    this.sesjonService.slettObservasjon(this.observasjon);
    this.observasjonSlettetEvent.emit();
  }

  registrerKommentar(kommentar: string) {
    this.observasjon.kommentar = kommentar;
  }

  rolleValgt(rolle: Rolle) {
    this.observasjon.rolle = rolle;
  }
}
