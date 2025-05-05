import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Department } from '../../models/api/Department';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { faCommentDots, faHandPaper } from '@fortawesome/free-regular-svg-icons';
import { DialogueTexts } from 'src/app/constants/dialogueTexts';
import { Farger } from '../../utils/farger';
import { Role } from '../../models/api/Role';
import { HanskeSesjonService } from '../../services/data/hansker-sesjon.service';
import { GloveObservation } from '../../models/api/GloveObservation';
import { HanskeMedIndikasjonTypeService } from '../../services/data/hanske-med-indikasjon-type.service';
import { HanskeUtenIndikasjonTypeService } from '../../services/data/hanske-uten-indikasjon-type.service';
import { HandhygieneEtterHanskebrukTypeService } from '../../services/data/handhygiene-etter-hanskebruk-type.service';
import { GloveWithoutIndicationType } from '../../models/api/GloveWithoutIndicationType';
import { HandHygieneAfterGloveUseType } from '../../models/api/HandHygieneAfterGloveUseType';
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';
import { Uuid } from 'src/app/utils/uuid';

@Component({
  selector: 'app-rediger-hanske-observasjon',
  templateUrl: './rediger-hanske-observasjon.component.html'
})
export class RedigerHanskeObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  Farger = Farger;
  DialogueTexts = DialogueTexts;
  gloveWithIndicationTypes: GloveWithIndicationType[] = [];
  gloveWithoutIndicationTypes: GloveWithoutIndicationType[] = [];
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];

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
  @Input() observasjon: GloveObservation;
  @Input() department: Department;
  @Output() observasjonSlettetEvent = new EventEmitter();
  visInfoModal = false;
  observasjonMangelTekst: any;


  ngOnInit(): void {

    this.uuid = Uuid.generateUUID();

    this.hanskeMedIndikasjonTypeService.getHanskeMedIndikasjonTyper().subscribe((gloveWithIndicationTypes) => {
      this.gloveWithIndicationTypes = gloveWithIndicationTypes;
      if (this.observasjon.gloveWithIndicationTypes.length) {
        this.activeTab = "med";
        this.gloveWithIndicationTypes.forEach(x => {
          if (this.observasjon.gloveWithIndicationTypes.some(y => y.code === x.code))
            x.isSelected = true;
        });
      }
    });
    this.hanskeUtenIndikasjonTypeService.getHanskeUtenIndikasjonTyper().subscribe((gloveWithoutIndicationTypes) => {
      this.gloveWithoutIndicationTypes = gloveWithoutIndicationTypes;
      if (this.observasjon.gloveWithoutIndicationTypes.length) {
        this.activeTab = "uten";
        this.gloveWithoutIndicationTypes.forEach(x => {
          if (this.observasjon.gloveWithoutIndicationTypes.some(y => y.code === x.code))
            x.isSelected = true;
        });
      }
    });
    this.handhygieneEtterHanskebrukTypeService.getHandhygieneEtterHanskebrukTyper().subscribe((handHygieneAfterGloveUseTypes) => {
      this.handHygieneAfterGloveUseTypes = handHygieneAfterGloveUseTypes;
    });

    this.hanskeBrukt = this.observasjon.gloveUsed;
    this.valgtHygieneEtterHanskebruk = this.observasjon.handHygieneAfterGloveUseType?.code;
  }

  hanskeMedIndikasjonerChanged(code, event) {
    this.gloveWithIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  hanskeUtenIndikasjonerChanged(code, event) {
    this.gloveWithoutIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  lagreObservasjon() {


    this.observasjon.gloveWithIndicationTypes = this.gloveWithIndicationTypes.filter(x => x.isSelected);
    this.observasjon.gloveWithoutIndicationTypes = this.gloveWithoutIndicationTypes.filter(x => x.isSelected);
    this.observasjon.gloveUsed = this.hanskeBrukt;

    if(this.observasjon.gloveUsed){
      this.observasjon.handHygieneAfterGloveUseType = this.handHygieneAfterGloveUseTypes.find(x => x.code === this.valgtHygieneEtterHanskebruk);
    }
    else {
      this.observasjon.handHygieneAfterGloveUseType = null;
      this.valgtHygieneEtterHanskebruk = null;
    }


    this.sesjonService.endreObservasjon(this.observasjon);
    this.erRedigeringsmodus = false;
  }

  slettObservasjon() {
    this.sesjonService.slettObservasjon(this.observasjon);
    this.observasjonSlettetEvent.emit();
  }

  registrerKommentar(comment: string) {
    this.observasjon.comment = comment;
  }

  rolleValgt(role: Role) {
    this.observasjon.role = role;
  }

  lukkInfoModal($event: boolean) {
    this.visInfoModal = $event;
    if(this.visInfoModal === false){
      this.observasjonMangelTekst = null;
    }
  }
}
