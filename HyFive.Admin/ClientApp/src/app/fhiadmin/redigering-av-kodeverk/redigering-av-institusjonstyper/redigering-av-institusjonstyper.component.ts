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

  institusjonstyper: InstitutionType[] = [];
  nyInstitusjonstype: OpprettInstitusjonstypeRequest = this.tomRequest();
  institusjonstypeSomEndres: InstitutionType = null;

  constructor(
    private institusjonstyperService: InstitusjonstyperService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastInstitusjonstyper();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastInstitusjonstyper() {
    this.institusjonstyperService.hentInstitusjonstyper().subscribe(
      (resultat) => this.institusjonstyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Institusjonstype: ' + error?.message, '', { disableTimeOut: true}),
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
      (opprettetInstitusjonstype) => this.toastrService.success(`Institusjonstype opprettet.`),
      error => this.toastrService.error(`En feil skjedde under opprettelse av institusjonstype ${this.nyInstitusjonstype.name}. Feil: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.nyInstitusjonstype = this.tomRequest(); this.lastInstitusjonstyper(); }
    );
  }

  valgtInstitusjonstype(institusjonstype: InstitutionType): void {
    if (this.institusjonstypeSomEndres?.id == institusjonstype.id) return;
    this.institusjonstypeSomEndres = JSON.parse(JSON.stringify(institusjonstype));
  }

  oppdaterInstitusjonstype(institusjonstype: InstitutionType): void {
    this.institusjonstyperService.oppdaterInstitusjonstype(institusjonstype).subscribe(
      (oppdatertInstitusjonstype) => {
        this.toastrService.success("Institusjonstype oppdatert");
        this.lastInstitusjonstyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Institusjonstype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.institusjonstypeSomEndres = null
    );
  }

  slettInstitusjonstype(institusjonstypeId: number) {
    this.institusjonstyperService.slettInstitusjonstype(institusjonstypeId).subscribe(
      (erSlettet) => {
        this.toastrService.success("Institusjonstype ble slettet");
        this.institusjonstypeSomEndres = null;
        this.lastInstitusjonstyper();
      }
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.institusjonstypeSomEndres = null;
  }
}
