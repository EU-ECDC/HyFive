import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../../models/api/User';
import { OpprettFhiAdminRequest } from '../../../models/api/OpprettFhiAdminRequest';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-rediger-fhiadmin',
  templateUrl: './rediger-fhiadmin.component.html'
})
export class RedigerFhiAdminComponent implements OnInit, OnDestroy {

  brukere: User[];
  fhiAdminSomEndres: User = null;
  nyFhiAdmin: OpprettFhiAdminRequest = null;

  constructor(
    private userService: UserService,
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
    this.userService.hentFhiAdmin().subscribe(
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
    this.userService.opprettFhiAdmin(this.nyFhiAdmin).subscribe(
      () => this.toastrService.success('FhiAdmin opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av FhiAdmin: ' + error?.error, '', { disableTimeOut: true}),
      () => { this.nyFhiAdmin = null; this.lastFhiAdmin(); }
    );
  }

  setFhiAdminSomEndres(fhiAdmin: User) {
    this.avbrytRedigering();
    if (this.fhiAdminSomEndres?.id == fhiAdmin.id) return;
    this.fhiAdminSomEndres = JSON.parse(JSON.stringify(fhiAdmin));
  }

  oppdaterFhiAdmin(fhiAdmin: User) {
    this.userService.oppdaterFhiAdmin(fhiAdmin).subscribe(
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
      && this.userService.isValidPseudonym(this.nyFhiAdmin?.identPseudonym);
  }

  kanEndres(fhiAdmin: User) {
    return fhiAdmin.fornavn.length > 0
      && fhiAdmin.etternavn.length > 0
      && this.userService.isValidPseudonym(fhiAdmin?.identPseudonym);
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
