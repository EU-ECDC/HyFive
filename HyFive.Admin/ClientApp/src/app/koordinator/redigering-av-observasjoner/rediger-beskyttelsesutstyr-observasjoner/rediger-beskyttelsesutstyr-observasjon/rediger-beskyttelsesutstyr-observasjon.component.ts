import { Component, Input, OnInit, EventEmitter, Output, TemplateRef, ViewChild } from '@angular/core';
import { faCheck, faCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { faSave } from '@fortawesome/free-solid-svg-icons';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Beskyttelsesutstyr } from 'src/app/models/api/Beskyttelsesutstyr';
import { BeskyttelsesutstyrObservasjon } from "src/app/models/api/BeskyttelsesutstyrObservasjon";
import { Avdeling } from "src/app/models/api/Avdeling";
import { BeskyttelsesutstyrMapper } from 'src/app/utils/beskyttelsesutstyrmapper';
import {ToastrService} from "ngx-toastr";
import { BeskyttelsesutstyrModalComponent, BeskyttelsesutstyrModalComponentConfig } from '../beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component';
import { ObservasjonService } from 'src/app/services/data/observasjon.service';
import { Rolle } from "../../../../models/api/Rolle";
import { Farger } from 'src/app/utils/farger';
import {BeskyttelsesutstyrsettingtyperService} from "../../../../services/data/beskyttelsesutstyrsettingtyper.service";
import {BeskyttelsesutstyrsettingType} from "../../../../models/api/BeskyttelsesutstyrsettingType";


@Component({
  selector: 'app-rediger-beskyttelsesutstyr-observasjon',
  templateUrl: './rediger-beskyttelsesutstyr-observasjon.component.html'
})
export class RedigerBeskyttelsesutstyrObservasjonComponent implements OnInit {

  Farger = Farger;
  ikonTypeMap: Map<string, IconProp> = BeskyttelsesutstyrMapper.getIkontypeMap();
  beskyttelsesutstyr: Beskyttelsesutstyr[] = [];
  settinger: BeskyttelsesutstyrsettingType[];
  valgtSetting: BeskyttelsesutstyrsettingType;
  kommentar: string;
  valgtUtstyr = null;
  observasjon: BeskyttelsesutstyrObservasjon;
  observasjonLaster: boolean = true;

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;

  constructor(
    private modalService: NgbModal,
    private observasjonService: ObservasjonService,
    private settingService: BeskyttelsesutstyrsettingtyperService,
    private toastrService: ToastrService) { }

  @Input() isReadonly: boolean = false;
  @Input() observasjonId: string;

  @Input() avdeling: Avdeling;
  @Input() institusjonid: number;
  @Input() sesjonId: string;
  @Output() observasjonSlettetEvent = new EventEmitter();
  @Output() observasjonOppdatertEvent = new EventEmitter<BeskyttelsesutstyrObservasjon>();

  ngOnInit(): void {
    this.observasjonService.hentBeskyttelsesutstyrObservasjon(this.observasjonId, this.sesjonId).subscribe(
      (o) => {
        this.observasjon = o;
        this.beskyttelsesutstyr = this.observasjon.beskyttelsesutstyrliste;
        this.observasjon.sesjonId = this.sesjonId;
        this.settingService.hentBeskyttelsesutstyrsettingtyper().subscribe((settinger) => {
          this.settinger = settinger;
          this.valgtSetting = this.observasjon.settingtype;
        })
      },
      (error) => this.toastrService.error("En feil skjedde under lasting av observasjon med id " + this.observasjonId, '', {disableTimeOut: true}),
      () => this.observasjonLaster = false
    );

  }

  registrerKommentar(kommentar: string) {
    this.observasjon.kommentar = kommentar;
    this.oppdater();
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
    this.oppdater();
  }

  visModal(valgtUtstyr: Beskyttelsesutstyr) {

    const modalRef = this.modalService.open(BeskyttelsesutstyrModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: BeskyttelsesutstyrModalComponentConfig.windowClass
    });

    modalRef.componentInstance.valgtUtstyr = JSON.parse(JSON.stringify(valgtUtstyr)) as Beskyttelsesutstyr;

    modalRef.componentInstance.visningsmodus = false;

    if (valgtUtstyr.bleBenyttet) {
      if (valgtUtstyr.bleBenyttetRiktig || valgtUtstyr.feilbruktyper.length > 0 || valgtUtstyr.kommentar !== '') {
        modalRef.componentInstance.visKnappForSlettingAvUtstyr = true;
      }
      else {
        modalRef.componentInstance.valgtUtstyr.bleBenyttetRiktig = null;
      }
    }

    modalRef.result.then((result: Beskyttelsesutstyr) => {
      valgtUtstyr.erIndikert = result.erIndikert;
      if (result.bleBenyttet === false) {
        this.nullstillUtstyr(valgtUtstyr);
      }
      else {
        valgtUtstyr.bleBenyttetRiktig = result.bleBenyttetRiktig;
        valgtUtstyr.kommentar = result.kommentar;
        valgtUtstyr.feilbruktyper = result.utstyrstype.feilbruktyper.filter(x => x.erValgt);
        valgtUtstyr.bleBenyttet = result.bleBenyttetRiktig || valgtUtstyr.feilbruktyper.length > 0 || valgtUtstyr.kommentar !== '';
      }
      this.oppdater();
    }, (error) => {
      valgtUtstyr.bleBenyttet = false;
      this.nullstillUtstyr(valgtUtstyr);
    });
  }

  setAlleUtstyrTilRiktigBrukt(event) {
    this.beskyttelsesutstyrIndikert().forEach(x => {
      x.bleBenyttet = true;
      x.bleBenyttetRiktig = true;
    });
    this.oppdater();
  }

  nullstillUtstyr(valg: Beskyttelsesutstyr) {
    let valgIndex = this.beskyttelsesutstyr.findIndex(x => x.utstyrstype.id === valg.utstyrstype.id);
    this.beskyttelsesutstyr[valgIndex] = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.observasjon.settingtype.utstyrstyper, valg).find(x => x.utstyrstype.id === valg.utstyrstype.id);
  }

  velgRolle($event: Rolle) {
    this.observasjon.rolle = $event;
    this.oppdater();
  }

  velgSetting() {
    this.observasjon.settingtype = this.valgtSetting;
    this.oppdater();
  }

  oppdater(){
    this.observasjonOppdatertEvent.emit(this.observasjon);
  }
  sorterteSettinger() : BeskyttelsesutstyrsettingType[]
  {
    let kombinerteSettinger = this.settinger.filter(s => s.kode != this.valgtSetting.kode)
    kombinerteSettinger.push(this.valgtSetting);
    kombinerteSettinger.sort((a, b) => {
      if(a.navn < b.navn) { return -1; }
      if(a.navn > b.navn) { return 1; }
      return 0;});

    return kombinerteSettinger;
  }
}
