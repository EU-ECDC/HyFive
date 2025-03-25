import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Avdeling } from '../../models/api/Avdeling';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { faCommentDots, faHandPaper } from '@fortawesome/free-regular-svg-icons';
import { Dialogtekster } from 'src/app/konstanter/dialogtekster';
import { Farger } from '../../utils/farger';
import { Rolle } from '../../models/api/Rolle';
import { HanskeSesjonService } from '../../services/data/hansker-sesjon.service';
import { HanskeObservasjon } from '../../models/api/HanskeObservasjon';
import { HanskeMedIndikasjonTypeService } from '../../services/data/hanske-med-indikasjon-type.service';
import { HanskeUtenIndikasjonTypeService } from '../../services/data/hanske-uten-indikasjon-type.service';
import { HandhygieneEtterHanskebrukTypeService } from '../../services/data/handhygiene-etter-hanskebruk-type.service';
import { HanskeUtenIndikasjonType } from '../../models/api/HanskeUtenIndikasjonType';
import { HandhygieneEtterHanskebrukType } from '../../models/api/HandhygieneEtterHanskebrukType';
import { HanskeMedIndikasjonType } from '../../models/api/HanskeMedIndikasjonType';
import { Uuid } from 'src/app/utils/uuid';

@Component({
  selector: 'app-rediger-hanske-observasjon',
  templateUrl: './rediger-hanske-observasjon.component.html'
})
export class RedigerHanskeObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  Farger = Farger;
  Dialogtekster = Dialogtekster;
  hanskeMedIndikasjonTyper: HanskeMedIndikasjonType[] = [];
  hanskeUtenIndikasjonTyper: HanskeUtenIndikasjonType[] = [];
  handhygieneEtterHanskebrukTyper: HandhygieneEtterHanskebrukType[] = [];

  activeTab = "med";
  hanskeBrukt = null;
  valgtHygieneEtterHanskebruk: string = null;

  uuid: string;

  faCircle = faCircle;
  faCheck = faCheck;
  faCommentLines = faCommentDots;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faHandPaper = faHandPaper;

  constructor(
    private sesjonService: HanskeSesjonService,
    private hanskeMedIndikasjonTypeService: HanskeMedIndikasjonTypeService,
    private hanskeUtenIndikasjonTypeService: HanskeUtenIndikasjonTypeService,
    private handhygieneEtterHanskebrukTypeService: HandhygieneEtterHanskebrukTypeService,
  ) { }

  @Input() isReadonly: boolean = false;
  @Input() observasjon: HanskeObservasjon;
  @Input() avdeling: Avdeling;
  @Output() observasjonSlettetEvent = new EventEmitter();
  visInfoModal = false;
  observasjonMangelTekst: any;


  ngOnInit(): void {

    this.uuid = Uuid.generateUUID();

    this.hanskeMedIndikasjonTypeService.getHanskeMedIndikasjonTyper().subscribe((hanskeMedIndikasjonTyper) => {
      this.hanskeMedIndikasjonTyper = hanskeMedIndikasjonTyper;
      if (this.observasjon.hanskeMedIndikasjonTyper.length) {
        this.activeTab = "med";
        this.hanskeMedIndikasjonTyper.forEach(x => {
          if (this.observasjon.hanskeMedIndikasjonTyper.some(y => y.kode === x.kode))
            x.erValgt = true;
        });
      }
    });
    this.hanskeUtenIndikasjonTypeService.getHanskeUtenIndikasjonTyper().subscribe((hanskeUtenIndikasjonTyper) => {
      this.hanskeUtenIndikasjonTyper = hanskeUtenIndikasjonTyper;
      if (this.observasjon.hanskeUtenIndikasjonTyper.length) {
        this.activeTab = "uten";
        this.hanskeUtenIndikasjonTyper.forEach(x => {
          if (this.observasjon.hanskeUtenIndikasjonTyper.some(y => y.kode === x.kode))
            x.erValgt = true;
        });
      }
    });
    this.handhygieneEtterHanskebrukTypeService.getHandhygieneEtterHanskebrukTyper().subscribe((handhygieneEtterHanskebrukTyper) => {
      this.handhygieneEtterHanskebrukTyper = handhygieneEtterHanskebrukTyper;
    });

    this.hanskeBrukt = this.observasjon.benyttetHanske;
    this.valgtHygieneEtterHanskebruk = this.observasjon.handhygieneEtterHanskebrukType?.kode;
  }

  hanskeMedIndikasjonerChanged(kode, event) {
    this.hanskeMedIndikasjonTyper.forEach(x => {
      if (x.kode === kode) x.erValgt = event.target.checked;
    });
  }

  hanskeUtenIndikasjonerChanged(kode, event) {
    this.hanskeUtenIndikasjonTyper.forEach(x => {
      if (x.kode === kode) x.erValgt = event.target.checked;
    });
  }

  lagreObservasjon() {


    this.observasjon.hanskeMedIndikasjonTyper = this.hanskeMedIndikasjonTyper.filter(x => x.erValgt);
    this.observasjon.hanskeUtenIndikasjonTyper = this.hanskeUtenIndikasjonTyper.filter(x => x.erValgt);
    this.observasjon.benyttetHanske = this.hanskeBrukt;

    if(this.observasjon.benyttetHanske){
      this.observasjon.handhygieneEtterHanskebrukType = this.handhygieneEtterHanskebrukTyper.find(x => x.kode === this.valgtHygieneEtterHanskebruk);
    }
    else {
      this.observasjon.handhygieneEtterHanskebrukType = null;
      this.valgtHygieneEtterHanskebruk = null;
    }


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

  lukkInfoModal($event: boolean) {
    this.visInfoModal = $event;
    if(this.visInfoModal === false){
      this.observasjonMangelTekst = null;
    }
  }
}
