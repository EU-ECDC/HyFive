import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ObservasjonEventService } from '../../services/events/observasjon-event.service';
import { IndicationType } from '../../models/api/IndicationType';
import { IndikasjonService } from '../../services/data/indikasjon.service';
import { faCircle, faPlus, faCheck } from '@fortawesome/free-solid-svg-icons';
import { IndicationTypeSelection } from '../../models/registration/indicationType-selection.model';
import { IndikasjonTypeMapper } from '../../utils/indikasjontype-mapper';
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
  ikonTypeMap: Map<IndicationTypeConstants, IconProp> = IndikasjonTypeMapper.getIkontypeMap();

  indicationTypeSelection: IndicationTypeSelection[] = [];

  @Input("erAktiv") erAktiv: boolean;
  @Input("parentId") parentId: string;
  @Input("tilgjengeligeIndikasjoner") tilgjengeligeIndikasjoner: IndicationType[];
  @Input("valgteIndikasjoner") valgteIndikasjoner: IndicationType[] = [];
  @Input("isReadonly") isReadonly: boolean;
  @Output() indikasjonsValgChangedEvent = new EventEmitter<IndicationType[]>();

  constructor(private observasjonEventService: ObservasjonEventService,
    private indikasjonService: IndikasjonService) { }

  ngOnInit(): void {
    this.indikasjonService.getIndikasjonstyper().subscribe((indicationTypes) => {
      this.tilgjengeligeIndikasjoner = indicationTypes;
      this.indicationTypeSelection = IndikasjonTypeMapper.getIndikasjonstypeValg(this.tilgjengeligeIndikasjoner, this.valgteIndikasjoner);
    });

    this.observasjonEventService.observasjonNullstiltEvent.subscribe((parentId) => {
      if (parentId == this.parentId) {
        this.valgteIndikasjoner = [];
        this.indicationTypeSelection = IndikasjonTypeMapper.getIndikasjonstypeValg(this.tilgjengeligeIndikasjoner, []);
      }
    })
  }

  changed(indication: IndicationType): void {
    let valg = this.indicationTypeSelection.filter(x => x.isSelected);
    this.indikasjonsValgChangedEvent.emit(this.tilgjengeligeIndikasjoner.filter(x => valg.some(y => y.code === x.code)));
  }
}
