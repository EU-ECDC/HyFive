import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ObservasjonEventService } from '../../services/events/observasjon-event.service';
import { IndikasjonType } from '../../models/api/IndikasjonType';
import { IndikasjonService } from '../../services/data/indikasjon.service';
import { faCircle, faPlus, faCheck } from '@fortawesome/free-solid-svg-icons';
import { IndikasjonTypeValg } from '../../models/registrering/indikasjontypevalg.model';
import { IndikasjonTypeMapper } from '../../utils/indikasjontype-mapper';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndikasjonTypeKonstanter } from '../../models/api/IndikasjonTypeKonstanter';

@Component({
  selector: 'app-indikasjonsvalg',
  templateUrl: './indikasjonsvalg.component.html'
})
export class IndikasjonsValgComponent implements OnInit {

  faCircle = faCircle;
  faCheck = faCheck;
  faPlus = faPlus;
  ikonTypeMap: Map<IndikasjonTypeKonstanter, IconProp> = IndikasjonTypeMapper.getIkontypeMap();

  indikasjonTypeValg: IndikasjonTypeValg[] = [];

  @Input("erAktiv") erAktiv: boolean;
  @Input("parentId") parentId: string;
  @Input("tilgjengeligeIndikasjoner") tilgjengeligeIndikasjoner: IndikasjonType[];
  @Input("valgteIndikasjoner") valgteIndikasjoner: IndikasjonType[] = [];
  @Input("isReadonly") isReadonly: boolean;
  @Output() indikasjonsValgChangedEvent = new EventEmitter<IndikasjonType[]>();

  constructor(private observasjonEventService: ObservasjonEventService,
    private indikasjonService: IndikasjonService) { }

  ngOnInit(): void {
    this.indikasjonService.getIndikasjonstyper().subscribe((indikasjonstyper) => {
      this.tilgjengeligeIndikasjoner = indikasjonstyper;
      this.indikasjonTypeValg = IndikasjonTypeMapper.getIndikasjonstypeValg(this.tilgjengeligeIndikasjoner, this.valgteIndikasjoner);
    });

    this.observasjonEventService.observasjonNullstiltEvent.subscribe((parentId) => {
      if (parentId == this.parentId) {
        this.valgteIndikasjoner = [];
        this.indikasjonTypeValg = IndikasjonTypeMapper.getIndikasjonstypeValg(this.tilgjengeligeIndikasjoner, []);
      }
    })
  }

  changed(indikasjon: IndikasjonType): void {
    let valg = this.indikasjonTypeValg.filter(x => x.erValgt);
    this.indikasjonsValgChangedEvent.emit(this.tilgjengeligeIndikasjoner.filter(x => valg.some(y => y.kode === x.kode)));
  }
}
