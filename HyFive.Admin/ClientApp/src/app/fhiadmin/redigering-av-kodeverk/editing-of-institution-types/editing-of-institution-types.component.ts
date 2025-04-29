import { Component, OnInit, OnDestroy } from '@angular/core';
import { InstitutionType } from '../../../models/api/InstitutionType';
import { ToastrService } from 'ngx-toastr';
import { InstitutionTypesService } from '../../../services/data/institusjonstyper.service';
import { CreateInstitutionTypeRequest } from 'src/app/models/api/CreateInstitutionTypeRequest';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-institution-types',
  templateUrl: './editing-of-institution-types.component.html'
})
export class EditingInstitutionTypesComponent implements OnInit, OnDestroy {

  institutionTypes: InstitutionType[] = [];
  newInstitutionType: CreateInstitutionTypeRequest = this.emptyRequest();
  institutiontypeAsChanged: InstitutionType = null;

  constructor(
    private institutionTypesService: InstitutionTypesService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadtInstitutionTypes();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadtInstitutionTypes() {
    this.institutionTypesService.getInstitutionTypes().subscribe(
      (result) => this.institutionTypes = result,
      (error) => this.toastrService.error('An error occurred while loading Institution type: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): CreateInstitutionTypeRequest {
    return {
      code: null,
      name: null
    }
  }

  createInstitutionType(): void {
    this.institutionTypesService.createInstitutionType(this.newInstitutionType).subscribe(
      (createdInstitutiontype) => this.toastrService.success(`Institutiontype created.`),
      error => this.toastrService.error(`An error occurred while creating the institution type ${this.newInstitutionType.name}. Error: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.newInstitutionType = this.emptyRequest(); this.loadtInstitutionTypes(); }
    );
  }

  selectedInstitutiontype(institutiontype: InstitutionType): void {
    if (this.institutiontypeAsChanged?.id == institutiontype.id) return;
    this.institutiontypeAsChanged = JSON.parse(JSON.stringify(institutiontype));
  }

  updateInstitutionType(institutiontype: InstitutionType): void {
    this.institutionTypesService.updateInstitutionType(institutiontype).subscribe(
      (updatedInstitutiontype) => {
        this.toastrService.success("Institutiontype updated");
        this.loadtInstitutionTypes();
      },
      error => this.toastrService.error('An error occurred while updating Institutiontype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.institutiontypeAsChanged = null
    );
  }

  deleteInstitutionType(institutiontypeId: number) {
    this.institutionTypesService.deleteInstitutionType(institutiontypeId).subscribe(
      (erSlettet) => {
        this.toastrService.success("Institutiontype was deleted");
        this.institutiontypeAsChanged = null;
        this.loadtInstitutionTypes();
      }
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.institutiontypeAsChanged = null;
  }
}
