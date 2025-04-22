import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservationOverviewReport} from "../../../models/api/ObservationOverviewReport";
import {Department} from "../../../models/api/Department";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {KeyEventService} from "../../../services/events/key-event.service";
import {HanskeMedIndikasjonTypeService} from "../../../services/data/hanskemedindikasjontype.service";
import {HanskeUtenIndikasjonTypeService} from "../../../services/data/hanskeutenindikasjontype.service";
import {HandhygieneEtterHanskebrukTypeService} from "../../../services/data/handhygieneetterhanskebruktype.service";
import {GloveWithIndicationType} from "../../../models/api/GloveWithIndicationType";
import {GloveWithoutIndicationType} from "../../../models/api/GloveWithoutIndicationType";
import {Role} from "../../../models/api/Role";
import {GloveObservation} from "../../../models/api/GloveObservation";
import {HandHygieneAfterGloveUseType} from "../../../models/api/HandHygieneAfterGloveUseType";

@Component({
  selector: 'app-edit-glove-observations',
  templateUrl: './edit-glove-observations.component.html'
})
export class EditGloveObservationsComponent implements OnInit{

  @Input() observations: ObservationOverviewReport[]
  @Input() sessionId: string;
  @Input() department: Department;
  @Input() canEdit = false;

  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  gloveObservationAsChanged: GloveObservation;
  selectedHygieneAfterGloveuseCode: any;

  gloveWithIndicationsSelected: boolean;

  gloveWithIndicationTypes: GloveWithIndicationType[];
  gloveWithoutIndicationTypes: GloveWithoutIndicationType[];
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[];

  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private hanskeMedIndikasjonService: HanskeMedIndikasjonTypeService,
    private hanskeUtenIndikasjonService: HanskeUtenIndikasjonTypeService,
    private handhygieneEtterHanskebrukService: HandhygieneEtterHanskebrukTypeService

  ) { }

  ngOnInit(): void {
    this.handhygieneEtterHanskebrukService.hentHandhygieneEtterHanskebrukTyper().subscribe((typer) => {
      this.handHygieneAfterGloveUseTypes = typer;
    })

    this.hanskeMedIndikasjonService.hentHanskeMedIndikasjonTyper().subscribe((typer)=> {
      this.gloveWithIndicationTypes = typer;
    })

    this.hanskeUtenIndikasjonService.hentHanskeUtenIndikasjonTyper().subscribe((typer)=> {
      this.gloveWithoutIndicationTypes = typer;
    })

    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.gloveObservationAsChanged)
        this.gloveObservationAsChanged = null;
    });
  }

  velgObservasjon(observation: ObservationOverviewReport) {
    if(!this.canEdit){
      return;
    }

    this.gloveObservationAsChanged = {
      usedGlove: observation.gloveObservation.usedGlove,
      id: observation.id,
      sessionId:  this.sessionId,
      comment: observation.comment,
      role: observation.role,
      registrationTime: observation.registrationTime,
      handhygieneAfterGloveuseType: observation.gloveObservation.handhygieneAfterGloveuseType,
    }
    this.gloveWithIndicationTypes.map((h) =>{
      h.isSelected = observation.gloveObservation.gloveWithIndicationTypes.map(x => x.code).indexOf(h.code) !== -1;
      return h;
    });

    this.gloveWithoutIndicationTypes.map((h) =>{
      h.isSelected = observation.gloveObservation.gloveWithoutIndicationTypes.map(x => x.code).indexOf(h.code) !== -1;
      return h;
    });

    this.selectedHygieneAfterGloveuseCode = this.gloveObservationAsChanged.handhygieneAfterGloveuseType?.code;
    this.gloveWithIndicationsSelected = this.gloveWithIndicationTypes.filter(h => h.isSelected).length > 0;
    if(this.gloveWithIndicationsSelected){
      this.gloveObservationAsChanged.gloveWithoutIndicationTypes = [];
    }
    else{
      // Impliserer at gloveWithoutIndicationTypes er valgt. Da skal hanske benyttes
      this.gloveObservationAsChanged.gloveWithIndicationTypes = [];
      this.gloveObservationAsChanged.usedGlove = true;
    }
  }

  endretKommentar(comment: string) {
    this.gloveObservationAsChanged.comment = comment;
  }

  updateGloveObservation() {
    if(this.gloveObservationAsChanged.usedGlove){
      this.gloveObservationAsChanged.handhygieneAfterGloveuseType = this.handHygieneAfterGloveUseTypes.find(x => x.code === this.selectedHygieneAfterGloveuseCode);
    }
    else {
      this.gloveObservationAsChanged.handhygieneAfterGloveuseType = null;
      this.selectedHygieneAfterGloveuseCode = null;
    }

    if(this.gloveWithIndicationsSelected){
      this.gloveObservationAsChanged.gloveWithIndicationTypes = this.gloveWithIndicationTypes.filter(h => h.isSelected);
      this.gloveObservationAsChanged.gloveWithoutIndicationTypes = [];
    }
    else {
      this.gloveObservationAsChanged.gloveWithoutIndicationTypes = this.gloveWithoutIndicationTypes.filter(h => h.isSelected);
      this.gloveObservationAsChanged.gloveWithIndicationTypes = [];
    }

    this.observationService.updateGloveObservation(this.gloveObservationAsChanged).subscribe(
      (erOppdatert) => {
        this.gloveObservationAsChanged = null;
        this.toastrService.success('Observasjonen ble oppdatert');
        this.observationUpdatedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observation: ', { disableTimeOut: true});
      }
    );
  }

  deleteGloveObservation() {
    this.observationService.deleteGloveObservation(this.gloveObservationAsChanged.id, this.sessionId).subscribe(
      () => {
        this.gloveObservationAsChanged = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved sletting av observation', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event) {
    event.stopPropagation();
    this.gloveObservationAsChanged = null;
    this.selectedHygieneAfterGloveuseCode = null;
  }

  velgRolle(role: Role) {
    this.gloveObservationAsChanged.role = role;
  }

  settBenyttetHanskeHvisAktuelt(gloveWithIndicationsSelected: boolean) {
    if(gloveWithIndicationsSelected == false){
      this.gloveObservationAsChanged.usedGlove = true;
    }
  }
}
