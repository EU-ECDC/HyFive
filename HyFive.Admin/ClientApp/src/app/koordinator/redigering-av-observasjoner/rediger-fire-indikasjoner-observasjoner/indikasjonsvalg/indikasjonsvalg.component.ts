import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import {IndikasjonTypeValg} from "../../../../models/fireindikasjoner/indikasjontypevalg.model";
import {IndikasjonType} from "../../../../models/api/IndikasjonType";
import {IndikasjonstyperService} from "../../../../services/data/indikasjonstyper.service";
import {IndikasjonTypeMapper} from "../../../../utils/indikasjontype-mapper";


@Component({
  selector: 'app-indikasjonsvalg',
  templateUrl: './indikasjonsvalg.component.html'
})
export class IndikasjonsValgComponent implements OnInit {

  indikasjonTypeValg: IndikasjonTypeValg[] = [];

  @Input("parentId") parentId: string;
  @Input("tilgjengeligeIndikasjoner") tilgjengeligeIndikasjoner: IndikasjonType[];
  @Input("valgteIndikasjoner") valgteIndikasjoner: IndikasjonType[] = [];
  @Output() indikasjonsValgChangedEvent = new EventEmitter<IndikasjonType[]>();

  constructor(private indikasjonstyperService: IndikasjonstyperService) { }

  ngOnInit(): void {
    this.indikasjonstyperService.hentIndikasjonstyper().subscribe((indikasjonstyper) => {
      this.tilgjengeligeIndikasjoner = indikasjonstyper;
      this.indikasjonTypeValg = IndikasjonTypeMapper.getIndikasjonstypeValg(this.tilgjengeligeIndikasjoner, this.valgteIndikasjoner);
    });
  }

  changed(indikasjon: IndikasjonType): void {
    let valg = this.indikasjonTypeValg.filter(x => x.erValgt);
    this.indikasjonsValgChangedEvent.emit(this.tilgjengeligeIndikasjoner.filter(x => valg.some(y => y.kode === x.kode)));
  }
}
