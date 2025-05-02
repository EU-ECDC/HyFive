import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { FireIndikasjonerSesjonsvisning } from '../../models/registrering/fire-indikasjoner-sesjonsvisning.model';
import { Activity } from '../../models/api/Activity';
import { Uuid } from '../../utils/uuid';
import { AktivitetUnderRegistrering, ObservasjonEventService } from '../../services/events/observasjon-event.service';
import { Animations } from '../../shared/animasjoner/animasjoner';
import { Kort } from '../../models/registrering/kort.model';
import { faSave, faTrashAlt, faTimesCircle } from '@fortawesome/free-regular-svg-icons';
import { Farger } from '../../utils/farger';
import { BaseKortSwipe } from '../../shared/kort-swipe/kort-swipe';
import { faHandHoldingWater, faDivide, faEraser, faHandsWash } from '@fortawesome/free-solid-svg-icons';
import { Role } from '../../models/api/Role';
import { IndikasjonType } from '../../models/api/IndikasjonType';
import { AktivitetTypeKonstanter } from 'src/app/models/api/AktivitetTypeKonstanter';
import { AktivitetService } from '../../services/data/aktivitet.service';
import { AktivitetType } from '../../models/api/AktivitetType';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Aktiviteter } from '../../konstanter/aktiviteter';


@Component({
  selector: 'app-fire-indikasjoner-observasjonskort',
  templateUrl: './fire-indikasjoner-observasjonskort.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class FireIndikasjonerObservasjonskortComponent extends BaseKortSwipe implements OnInit {

  AktivitetTypeKonstanter = AktivitetTypeKonstanter;
  kommentar: string;
  activity: Activity;
  aktivitetTyper: AktivitetType[];
  valgteIndikasjoner: IndikasjonType[] = new Array();
  aktivitetUnderRegistrering: AktivitetUnderRegistrering = null;
  observasjonMangelTekst: string;
  visInfoModal: boolean = false;
  dialogtekster = Dialogtekster;
  sprit: string = Aktiviteter.Sprit;
  vask: string = Aktiviteter.Vask;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faHandHoldingWater = faHandHoldingWater;
  faHandsWash = faHandsWash;
  faDivide = faDivide;
  faTimes = faTimes;
  faTimesCircle = faTimesCircle;
  farger = Farger;

  @Input("kort") kort: Kort;
  @Input("rollevalg") rollevalg: Role[]
  @Input("sesjonsvisning") sesjonsvisning: FireIndikasjonerSesjonsvisning

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Kort>();

  constructor(
    private observasjonEventService: ObservasjonEventService,
    private aktivitetService: AktivitetService,
    protected modalService: NgbModal
  ) {
    super(modalService);
  }

  ngOnInit(): void {
    this.aktivitetService.getAktivitetTyper().subscribe((aktivitetTyper) => {
      this.aktivitetTyper = aktivitetTyper;
    });
    this.observasjonEventService.registreringAvAktivitetHarBegynt.subscribe(activity => {
      if (activity.parentId == this.kort.id) {
        this.aktivitetUnderRegistrering = activity;
      }
    })
  }

  slettKort() {
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  velgRolle(role: Role) {
    this.kort.role = role;
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort[kortIndex] = this.kort;
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  nullstillKort() {
    this.kommentar = "";
    this.valgteIndikasjoner = [];
    this.activity = null;
    this.aktivitetUnderRegistrering = null;
    this.observasjonEventService.observasjonNullstiltEvent.emit(this.kort.id);
  }

  kanIkkeLagre(): boolean {
    if (this.valgteIndikasjoner?.length === 0) {
      this.observasjonMangelTekst = "Indikasjon(er) mangler";
      this.visInfoModal = true;
    }
    else if (!this.activity) {
      this.observasjonMangelTekst = "Activity mangler";
      this.visInfoModal = true;
    }
    return !(this.valgteIndikasjoner?.length && this.activity);
  }

  registrerObservasjon() {
    let observasjon: FireIndikasjonerObservasjon = {
      id: Uuid.generateUUID(),
      sesjonId: this.sesjonsvisning.sesjonId,
      kommentar: this.kommentar,
      role: this.kort.role,
      activity: this.activity,
      indikasjonstyper: this.valgteIndikasjoner,
      registrerttidspunkt: new Date(Date.now())
    }

    this.observasjonRegistrert.emit(observasjon);

    this.nullstillKort();

    this.kort.erAktivt = false;
  }

  registrerAktivitet(activity: Activity) {
    this.activity = activity;
  }

  fjernTidsregistrering(){
    this.activity.sekunderBrukt = 0;
    this.activity.tidtakingBleUtfort = false;
  }
  indikasjonsValgChanged(valgteIndikasjoner: IndikasjonType[]) {
    this.valgteIndikasjoner = valgteIndikasjoner;
  }

  registrerKommentar(kommentar: string) {
    this.kommentar = kommentar;
  }

  skalAktivitetDeaktiveres(aktivitetTypeKode: string) {
    return this.activity != null
      || (this.aktivitetUnderRegistrering != null && this.aktivitetUnderRegistrering.aktivitetType.kode != aktivitetTypeKode)
  }

  erRegistrert(aktivitetTypeKode: string) {
    return this.activity?.aktivitetType.kode == aktivitetTypeKode;
  }

  kortErValgt() {
    this.kort.erAktivt = true;
    this.kortErValgtEvent.emit(this.kort);
  }

  getAktivitetType(kode: string) {
    return this.aktivitetTyper?.find(x => x.kode === kode);
  }

  lukkInfoModal(erVisInfoModal): void {
    this.visInfoModal = erVisInfoModal;
  }
}
