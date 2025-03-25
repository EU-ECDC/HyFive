import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { BrukerService } from '../../../services/data/bruker.service';
import { ToastrService } from 'ngx-toastr';
import { Bruker } from '../../../models/api/Bruker';
import { OpprettFhiAdminRequest } from '../../../models/api/OpprettFhiAdminRequest';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-rediger-fhiadmin',
  templateUrl: './rediger-fhiadmin.component.html'
})
export class RedigerFhiAdminComponent implements OnInit, OnDestroy {

  brukere: Bruker[];
  fhiAdminSomEndres: Bruker = null;
  nyFhiAdmin: OpprettFhiAdminRequest = null;

  constructor(
    private brukerService: BrukerService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });
    this.lastFhiAdmin();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  lastFhiAdmin() {
    this.brukerService.hentFhiAdmin().subscribe(
      (fhiAdmins) => this.brukere = fhiAdmins,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av FhiAdmin: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  opprettTomFhiAdmin() {
    this.avbrytRedigering();
    this.nyFhiAdmin = {
      etternavn: '',
      fornavn: '',
      identPseudonym: null,
    } as OpprettFhiAdminRequest;
  }

  opprettFhiAdmin() {
    this.brukerService.opprettFhiAdmin(this.nyFhiAdmin).subscribe(
      () => this.toastrService.success('FhiAdmin opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av FhiAdmin: ' + error?.error, '', { disableTimeOut: true}),
      () => { this.nyFhiAdmin = null; this.lastFhiAdmin(); }
    );
  }

  setFhiAdminSomEndres(fhiAdmin: Bruker) {
    this.avbrytRedigering();
    if (this.fhiAdminSomEndres?.id == fhiAdmin.id) return;
    this.fhiAdminSomEndres = JSON.parse(JSON.stringify(fhiAdmin));
  }

  oppdaterFhiAdmin(fhiAdmin: Bruker) {
    this.brukerService.oppdaterFhiAdmin(fhiAdmin).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('FhiAdmin oppdatert');
        this.lastFhiAdmin();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av FhiAdmin: ' + error?.error, '', { disableTimeOut: true}),
      () => this.fhiAdminSomEndres = null
    );
  }

  kanOpprettes() {
    return this.nyFhiAdmin.fornavn?.length > 0
      && this.nyFhiAdmin.etternavn?.length > 0
      && this.brukerService.erGyldigPseudonym(this.nyFhiAdmin?.identPseudonym);
  }

  kanEndres(fhiAdmin: Bruker) {
    return fhiAdmin.fornavn.length > 0
      && fhiAdmin.etternavn.length > 0
      && this.brukerService.erGyldigPseudonym(fhiAdmin?.identPseudonym);
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.nyFhiAdmin = null;
    this.fhiAdminSomEndres = null;
  }
}
