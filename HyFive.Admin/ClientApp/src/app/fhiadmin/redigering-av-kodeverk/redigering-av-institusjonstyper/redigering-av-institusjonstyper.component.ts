import { Component, OnInit, OnDestroy } from '@angular/core';
import { InstitutionType } from '../../../models/api/InstitutionType';
import { ToastrService } from 'ngx-toastr';
import { InstitusjonstyperService } from '../../../services/data/institusjonstyper.service';
import { OpprettInstitusjonstypeRequest } from 'src/app/models/api/OpprettInstitusjonstypeRequest';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-institusjonstyper',
  templateUrl: './redigering-av-institusjonstyper.component.html'
})
export class RedigeringAvInstitusjonstyperComponent implements OnInit, OnDestroy {

  institutionTypes: InstitutionType[] = [];
  nyInstitusjonstype: OpprettInstitusjonstypeRequest = this.tomRequest();
  institusjonstypeSomEndres: InstitutionType = null;

  constructor(
    private institusjonstyperService: InstitusjonstyperService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.lastInstitusjonstyper();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastInstitusjonstyper() {
    this.institusjonstyperService.hentInstitusjonstyper().subscribe(
      (result) => this.institutionTypes = result,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Institutiontype: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  tomRequest(): OpprettInstitusjonstypeRequest {
    return {
      code: null,
      name: null
    }
  }

  opprettInstitusjonstype(): void {
    this.institusjonstyperService.opprettInstitusjonstype(this.nyInstitusjonstype).subscribe(
      (opprettetInstitusjonstype) => this.toastrService.success(`Institutiontype opprettet.`),
      error => this.toastrService.error(`En feil skjedde under opprettelse av institutiontype ${this.nyInstitusjonstype.name}. Feil: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.nyInstitusjonstype = this.tomRequest(); this.lastInstitusjonstyper(); }
    );
  }

  selectedInstitutiontype(institutiontype: InstitutionType): void {
    if (this.institusjonstypeSomEndres?.id == institutiontype.id) return;
    this.institusjonstypeSomEndres = JSON.parse(JSON.stringify(institutiontype));
  }

  oppdaterInstitusjonstype(institutiontype: InstitutionType): void {
    this.institusjonstyperService.oppdaterInstitusjonstype(institutiontype).subscribe(
      (oppdatertInstitusjonstype) => {
        this.toastrService.success("Institutiontype oppdatert");
        this.lastInstitusjonstyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Institutiontype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.institusjonstypeSomEndres = null
    );
  }

  slettInstitusjonstype(institusjonstypeId: number) {
    this.institusjonstyperService.slettInstitusjonstype(institusjonstypeId).subscribe(
      (erSlettet) => {
        this.toastrService.success("Institutiontype ble slettet");
        this.institusjonstypeSomEndres = null;
        this.lastInstitusjonstyper();
      }
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.institusjonstypeSomEndres = null;
  }
}
