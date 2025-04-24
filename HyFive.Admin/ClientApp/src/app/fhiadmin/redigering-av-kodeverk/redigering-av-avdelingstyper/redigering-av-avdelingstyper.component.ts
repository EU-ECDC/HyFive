import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { DepartmentType } from '../../../models/api/DepartmentType';
import { DepartmentService } from '../../../services/data/department.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-avdelingstyper',
  templateUrl: './redigering-av-avdelingstyper.component.html'
})
export class RedigeringAvAvdelingstyperComponent implements OnInit, OnDestroy {

  departmentTypes: DepartmentType[] = [];
  newDepartmentType: DepartmentType = this.tomRequest();
  departmentTypeAsChanged: DepartmentType = null;

  constructor(
    private departmentService: DepartmentService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.lastAvdelingTyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastAvdelingTyper() {
    this.departmentService.getDepartmentTypes().subscribe(
      (resultat) => this.departmentTypes = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av departmentTypes: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  tomRequest(): DepartmentType {
    return {
      id: 0,
      code: null,
      name: null
    };
  }

  opprettAvdelingType() {
    this.departmentService.createDepartmentType(this.newDepartmentType).subscribe(
      (avdelingstype) => this.toastrService.success(`Avdelingtype med name ${avdelingstype.name} was created`),
      error => this.toastrService.error(`En feil skjedde under opprettelse av departmentType ${this.newDepartmentType.name}. Feil: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.newDepartmentType = this.tomRequest(); this.lastAvdelingTyper(); }
    );
  }

  valgtAvdelingType(avdelingstype: DepartmentType): void {
    if (this.departmentTypeAsChanged?.id == avdelingstype.id) return;
    this.departmentTypeAsChanged = JSON.parse(JSON.stringify(avdelingstype));
  }

  oppdaterAvdelingType(avdelingstype: DepartmentType): void {
    this.departmentService.updateDepartmentType(avdelingstype).subscribe(
      (result) => {
        this.toastrService.success('Avdelingtype ble oppdatert');
        this.lastAvdelingTyper();
      },
      (error) => {
        this.toastrService.error('En feil skjedde under oppdatering av avdelingtype: ' + error?.error, '', { disableTimeOut: true});
      },
      () => this.departmentTypeAsChanged = null
    );
  }
  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.departmentTypeAsChanged = null;
  }
}
