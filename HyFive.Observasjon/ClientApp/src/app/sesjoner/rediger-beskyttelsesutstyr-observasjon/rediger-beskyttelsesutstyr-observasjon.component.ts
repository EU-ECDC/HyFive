import { Component, Input, OnInit, EventEmitter, Output, TemplateRef, ViewChild, OnDestroy } from '@angular/core';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { Dialogtekster } from 'src/app/konstanter/dialogtekster';
import { BeskyttelsesutstyrMapper } from 'src/app/utils/beskyttelsesutstyrmapper';
import { Farger } from '../../utils/farger';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { BeskyttelsesutstyrObservasjon } from '../../models/api/BeskyttelsesutstyrObservasjon';
import { Avdeling } from '../../models/api/Avdeling';
import { Beskyttelsesutstyr } from '../../models/api/Beskyttelsesutstyr';
import { BeskyttelsesutstyrSesjonService } from '../../services/data/beskyttelsesutstyr-sesjon.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BeskyttelsesutstyrModalComponent, BeskyttelsesutstyrModalComponentConfig } from '../../registrering/beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component';
import { SesjonType } from 'src/app/models/api/SesjonType';
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-rediger-beskyttelsesutstyr-observasjon',
  templateUrl: './rediger-beskyttelsesutstyr-observasjon.component.html'
})
export class RedigerBeskyttelsesutstyrObservasjonComponent implements OnInit, OnDestroy {

  erRedigeringsmodus: boolean = false;
  dialogtekster = Dialogtekster;
  Farger = Farger;
  ikonTypeMap: Map<string, IconProp> = BeskyttelsesutstyrMapper.getIkontypeMap();
  beskyttelsesutstyr: Beskyttelsesutstyr[] = [];
  kommentar: string;
  valgtUtstyr = null;
  beskyttelsesutstyrsesjontype: SesjonType = SesjonType.Beskyttelsesutstyr;

  kanIkkeLagreMelding = Dialogtekster.KanIkkeLagreBeskyttelsesutstyrObservasjon;

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;

  constructor(
    private sesjonService: BeskyttelsesutstyrSesjonService,
    private modalService: NgbModal,
    private toastrService: ToastrService) { }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observasjon") observasjon: BeskyttelsesutstyrObservasjon;
  @Input("avdeling") avdeling: Avdeling;
  @Input("institusjonid") institusjonid: number;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent = new EventEmitter();


  ngOnInit(): void {
    this.beskyttelsesutstyr = this.observasjon.beskyttelsesutstyrliste;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  registrerKommentar(kommentar: string) {
    this.observasjon.kommentar = kommentar;
  }

  lagreObservasjon() {
    if(!this.kanLagre()){
      this.toastrService.error(this.kanIkkeLagreMelding, '', {disableTimeOut: true})
      return;
    }
    if (this.erRedigeringsmodus) {
      this.sesjonService.endreObservasjon(this.observasjon);
    }
    this.erRedigeringsmodus = false;
  }

  slettObservasjon() {
    this.sesjonService.slettObservasjon(this.observasjon);
    this.observasjonSlettetEvent.emit();
  }

  beskyttelsesutstyrIndikert(): Beskyttelsesutstyr[] {
    return this.beskyttelsesutstyr.filter(b => b.erIndikert);
  }

  beskyttelsesutstyrIkkeIndikert(): Beskyttelsesutstyr[] {
    return this.beskyttelsesutstyr.filter(b => b.erIndikert === false);
  }

  changed(event, valg: Beskyttelsesutstyr) {
    event.srcElement.blur();
    event.preventDefault();

    valg.bleBenyttet = true;
    valg.utstyrstype.feilbruktyper.filter(fb => fb.erValgt == true).map(fb => fb.erValgt = false);

    valg.feilbruktyper.forEach(f => {
      const index = valg.utstyrstype.feilbruktyper.findIndex(fb => fb.id == f.id);
      valg.utstyrstype.feilbruktyper[index].erValgt = true;
    });

    if (valg.bleBenyttet) {
      this.visModal(valg);
    }
  }

  visModal(valgtUtstyr: Beskyttelsesutstyr) {

    const modalRef = this.modalService.open(BeskyttelsesutstyrModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: BeskyttelsesutstyrModalComponentConfig.windowClass
    });

    modalRef.componentInstance.valgtUtstyr = JSON.parse(JSON.stringify(valgtUtstyr)) as Beskyttelsesutstyr;

    modalRef.componentInstance.visningsmodus = !this.erRedigeringsmodus;

    if (this.erRedigeringsmodus && valgtUtstyr.bleBenyttet) {
      if (valgtUtstyr.bleBenyttetRiktig || valgtUtstyr.feilbruktyper.length > 0 || valgtUtstyr.kommentar !== '') {
        modalRef.componentInstance.visKnappForSlettingAvUtstyr = true;
      }
      else {
        modalRef.componentInstance.valgtUtstyr.bleBenyttetRiktig = null;
      }
    }

    modalRef.result.then((result: Beskyttelsesutstyr) => {
      if (!this.erRedigeringsmodus) {
        return;
      }
      valgtUtstyr.erIndikert = result.erIndikert;
      valgtUtstyr.utstyrstype.erIndikert = result.erIndikert;
      if (result.bleBenyttet === false) {
        this.nullstillUtstyr(valgtUtstyr);
      }
      else {
        valgtUtstyr.bleBenyttetRiktig = result.bleBenyttetRiktig;
        valgtUtstyr.kommentar = result.kommentar;
        valgtUtstyr.feilbruktyper = result.utstyrstype.feilbruktyper.filter(x => x.erValgt);
        valgtUtstyr.bleBenyttet = result.bleBenyttetRiktig || valgtUtstyr.feilbruktyper.length > 0 || valgtUtstyr.kommentar !== '';
      }
    }, (reason) => {
      if (this.erRedigeringsmodus) {
        valgtUtstyr.bleBenyttet = false;
        this.nullstillUtstyr(valgtUtstyr);
      }
    });
  }

  setAlleUtstyrTilRiktigBrukt(event) {
    this.beskyttelsesutstyrIndikert().forEach(x => {
      x.bleBenyttet = true;
      x.bleBenyttetRiktig = true;
    });
  }

  nullstillUtstyr(valg: Beskyttelsesutstyr) {
    let valgIndex = this.beskyttelsesutstyr.findIndex(x => x.utstyrstype.id === valg.utstyrstype.id);
    this.beskyttelsesutstyr[valgIndex] = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.observasjon.settingtype.utstyrstyper).find(x => x.utstyrstype.id === valg.utstyrstype.id);
  }

  visVisningsmodusModal(event, valg: Beskyttelsesutstyr) {
    if (!this.erRedigeringsmodus && valg.bleBenyttet) {
      event.stopPropagation();
      event.preventDefault();
      this.visModal(valg);
      return;
    }
  }

  kanLagre() {
    return this.sesjonService.antallKvalifisertUtstyr(this.observasjon.beskyttelsesutstyrliste) > 0;
  }
}
