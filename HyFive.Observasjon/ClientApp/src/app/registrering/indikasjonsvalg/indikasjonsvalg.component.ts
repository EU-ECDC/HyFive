import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ObservationEventService } from '../../services/events/observation-event.service';
import { IndicationType } from '../../models/api/IndicationType';
import { IndicationService } from '../../services/data/indication.service';
import { faCircle, faPlus, faCheck } from '@fortawesome/free-solid-svg-icons';
import { IndicationTypeSelection } from '../../models/registration/indicationType-selection.model';
import { IndicationTypeMapper } from '../../utils/indicationtype-mapper';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndicationTypeConstants } from '../../models/api/IndicationTypeConstants';

@Component({
  selector: 'app-indikasjonsvalg',
  templateUrl: './indikasjonsvalg.component.html'
})
export class IndikasjonsValgComponent implements OnInit {

  faCircle = faCircle;
  faCheck = faCheck;
  faPlus = faPlus;
  iconTypeMap: Map<IndicationTypeConstants, IconProp> = IndicationTypeMapper.getIconTypeMap();

  indicationTypeSelection: IndicationTypeSelection[] = [];

  @Input("erAktiv") erAktiv: boolean;
  @Input("parentId") parentId: string;
  @Input("tilgjengeligeIndikasjoner") tilgjengeligeIndikasjoner: IndicationType[];
  @Input("valgteIndikasjoner") valgteIndikasjoner: IndicationType[] = [];
  @Input("isReadonly") isReadonly: boolean;
  @Output() indikasjonsValgChangedEvent = new EventEmitter<IndicationType[]>();

  constructor(private observationEventService: ObservationEventService,
    private indicationService: IndicationService) { }

  ngOnInit(): void {
    this.indicationService.getIndicationTypes().subscribe((indicationTypes) => {
      this.tilgjengeligeIndikasjoner = indicationTypes;
      this.indicationTypeSelection = IndicationTypeMapper.getIndicationTypeOption(this.tilgjengeligeIndikasjoner, this.valgteIndikasjoner);
    });

    this.observationEventService.observationResetEvent.subscribe((parentId) => {
      if (parentId == this.parentId) {
        this.valgteIndikasjoner = [];
        this.indicationTypeSelection = IndicationTypeMapper.getIndicationTypeOption(this.tilgjengeligeIndikasjoner, []);
      }
    })
  }

  changed(indication: IndicationType): void {
    let valg = this.indicationTypeSelection.filter(x => x.isSelected);
    this.indikasjonsValgChangedEvent.emit(this.tilgjengeligeIndikasjoner.filter(x => valg.some(y => y.code === x.code)));
  }
}
