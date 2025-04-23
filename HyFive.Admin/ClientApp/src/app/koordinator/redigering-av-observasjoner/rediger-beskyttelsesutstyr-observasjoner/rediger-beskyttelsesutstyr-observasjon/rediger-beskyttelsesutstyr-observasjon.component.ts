import { Component, Input, OnInit, EventEmitter, Output, TemplateRef, ViewChild } from '@angular/core';
import { faCheck, faCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { faSave } from '@fortawesome/free-solid-svg-icons';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ProtectiveEquipment } from 'src/app/models/api/ProtectiveEquipment';
import { ProtectiveEquipmentObservation } from "src/app/models/api/ProtectiveEquipmentObservation";
import { Department} from "src/app/models/api/Department";
import { ProtectiveEquipmentMapper } from 'src/app/utils/protective-equipment-mapper';
import {ToastrService} from "ngx-toastr";
import { BeskyttelsesutstyrModalComponent, BeskyttelsesutstyrModalComponentConfig } from '../beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component';
import { ObservationService } from 'src/app/services/data/observation.service';
import { Role } from "../../../../models/api/Role";
import { Farger } from 'src/app/utils/farger';
import {ProtectiveEquipmentSettingTypesService} from "../../../../services/data/protectiveEquipmentSettingTypes.service";
import {ProtectiveEquipmentType} from "../../../../models/api/ProtectiveEquipmentType";


@Component({
  selector: 'app-rediger-beskyttelsesutstyr-observation',
  templateUrl: './rediger-beskyttelsesutstyr-observasjon.component.html'
})
export class RedigerBeskyttelsesutstyrObservasjonComponent implements OnInit {

  Farger = Farger;
  ikonTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();
  beskyttelsesutstyr: ProtectiveEquipment[] = [];
  settinger: ProtectiveEquipmentType[];
  valgtSetting: ProtectiveEquipmentType;
  comment: string;
  valgtUtstyr = null;
  observation: ProtectiveEquipmentObservation;
  observasjonLaster: boolean = true;

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;

  constructor(
    private modalService: NgbModal,
    private observationService: ObservationService,
    private settingService: ProtectiveEquipmentSettingTypesService,
    private toastrService: ToastrService) { }

  @Input() isReadonly: boolean = false;
  @Input() observasjonId: string;

  @Input() avdeling: Department;
  @Input() institusjonid: number;
  @Input() sesjonId: string;
  @Output() observasjonSlettetEvent = new EventEmitter();
  @Output() observasjonOppdatertEvent = new EventEmitter<ProtectiveEquipmentObservation>();

  ngOnInit(): void {
    this.observationService.getProtectiveEquipmentObservation(this.observasjonId, this.sesjonId).subscribe(
      (o) => {
        this.observation = o;
        this.beskyttelsesutstyr = this.observation.protectiveEquipmentList;
        this.observation.sessionId = this.sesjonId;
        this.settingService.getProtectiveEquipmentTypes().subscribe((settinger) => {
          this.settinger = settinger;
          this.valgtSetting = this.observation.settingtype;
        })
      },
      (error) => this.toastrService.error("En feil skjedde under lasting av observation med id " + this.observasjonId, '', {disableTimeOut: true}),
      () => this.observasjonLaster = false
    );

  }

  registrerKommentar(comment: string) {
    this.observation.comment = comment;
    this.oppdater();
  }

  beskyttelsesutstyrIndikert(): ProtectiveEquipment[] {
    return this.beskyttelsesutstyr.filter(b => b.isIndicated);
  }

  beskyttelsesutstyrIkkeIndikert(): ProtectiveEquipment[] {
    return this.beskyttelsesutstyr.filter(b => b.isIndicated === false);
  }

  changed(event, valg: ProtectiveEquipment) {
    event.srcElement.blur();
    event.preventDefault();

    valg.wasUsed = true;
    valg.equipmentTypeq.misuseTypes.filter(fb => fb.isSelected == true).map(fb => fb.isSelected = false);

    valg.misuseTypes.forEach(f => {
      const index = valg.equipmentTypeq.misuseTypes.findIndex(fb => fb.id == f.id);
      valg.equipmentTypeq.misuseTypes[index].isSelected = true;
    });

    if (valg.wasUsed) {
      this.visModal(valg);
    }
    this.oppdater();
  }

  visModal(valgtUtstyr: ProtectiveEquipment) {

    const modalRef = this.modalService.open(BeskyttelsesutstyrModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: BeskyttelsesutstyrModalComponentConfig.windowClass
    });

    modalRef.componentInstance.valgtUtstyr = JSON.parse(JSON.stringify(valgtUtstyr)) as ProtectiveEquipment;

    modalRef.componentInstance.visningsmodus = false;

    if (valgtUtstyr.wasUsed) {
      if (valgtUtstyr.wasUsedProperly || valgtUtstyr.misuseTypes.length > 0 || valgtUtstyr.comment !== '') {
        modalRef.componentInstance.visKnappForSlettingAvUtstyr = true;
      }
      else {
        modalRef.componentInstance.valgtUtstyr.wasUsedProperly = null;
      }
    }

    modalRef.result.then((result: ProtectiveEquipment) => {
      valgtUtstyr.isIndicated = result.isIndicated;
      if (result.wasUsed === false) {
        this.nullstillUtstyr(valgtUtstyr);
      }
      else {
        valgtUtstyr.wasUsedProperly = result.wasUsedProperly;
        valgtUtstyr.comment = result.comment;
        valgtUtstyr.misuseTypes = result.equipmentTypeq.misuseTypes.filter(x => x.isSelected);
        valgtUtstyr.wasUsed = result.wasUsedProperly || valgtUtstyr.misuseTypes.length > 0 || valgtUtstyr.comment !== '';
      }
      this.oppdater();
    }, (error) => {
      valgtUtstyr.wasUsed = false;
      this.nullstillUtstyr(valgtUtstyr);
    });
  }

  setAlleUtstyrTilRiktigBrukt(event) {
    this.beskyttelsesutstyrIndikert().forEach(x => {
      x.wasUsed = true;
      x.wasUsedProperly = true;
    });
    this.oppdater();
  }

  nullstillUtstyr(valg: ProtectiveEquipment) {
    let valgIndex = this.beskyttelsesutstyr.findIndex(x => x.equipmentTypeq.id === valg.equipmentTypeq.id);
    this.beskyttelsesutstyr[valgIndex] = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.observation.settingtype.equipmentTypeqs, valg).find(x => x.equipmentTypeq.id === valg.equipmentTypeq.id);
  }

  velgRolle($event: Role) {
    this.observation.role = $event;
    this.oppdater();
  }

  velgSetting() {
    this.observation.settingtype = this.valgtSetting;
    this.oppdater();
  }

  oppdater(){
    this.observasjonOppdatertEvent.emit(this.observation);
  }
  sorterteSettinger() : ProtectiveEquipmentType[]
  {
    let kombinerteSettinger = this.settinger.filter(s => s.code != this.valgtSetting.code)
    kombinerteSettinger.push(this.valgtSetting);
    kombinerteSettinger.sort((a, b) => {
      if(a.name < b.name) { return -1; }
      if(a.name > b.name) { return 1; }
      return 0;});

    return kombinerteSettinger;
  }
}
