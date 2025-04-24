import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../../models/api/User';
import { CreateFhiAdminRequest } from '../../../models/api/CreateFhiAdminRequest';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-rediger-fhiadmin',
  templateUrl: './rediger-fhiadmin.component.html'
})
export class RedigerFhiAdminComponent implements OnInit, OnDestroy {

  brukere: User[];
  fhiAdminSomEndres: User = null;
  nyFhiAdmin: CreateFhiAdminRequest = null;

  constructor(
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });
    this.lastFhiAdmin();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  lastFhiAdmin() {
    this.userService.getFhiAdmin().subscribe(
      (fhiAdmins) => this.brukere = fhiAdmins,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av FhiAdmin: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  opprettTomFhiAdmin() {
    this.cancelEdit();
    this.nyFhiAdmin = {
      lastName: '',
      firstName: '',
      identityPseudonym: null,
    } as CreateFhiAdminRequest;
  }

  createFhiAdmin() {
    this.userService.createFhiAdmin(this.nyFhiAdmin).subscribe(
      () => this.toastrService.success('FhiAdmin opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av FhiAdmin: ' + error?.error, '', { disableTimeOut: true}),
      () => { this.nyFhiAdmin = null; this.lastFhiAdmin(); }
    );
  }

  setFhiAdminSomEndres(fhiAdmin: User) {
    this.cancelEdit();
    if (this.fhiAdminSomEndres?.id == fhiAdmin.id) return;
    this.fhiAdminSomEndres = JSON.parse(JSON.stringify(fhiAdmin));
  }

  updateFhiAdmin(fhiAdmin: User) {
    this.userService.updateFhiAdmin(fhiAdmin).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('FhiAdmin oppdatert');
        this.lastFhiAdmin();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av FhiAdmin: ' + error?.error, '', { disableTimeOut: true}),
      () => this.fhiAdminSomEndres = null
    );
  }

  kanOpprettes() {
    return this.nyFhiAdmin.firstName?.length > 0
      && this.nyFhiAdmin.lastName?.length > 0
      && this.userService.isValidPseudonym(this.nyFhiAdmin?.identityPseudonym);
  }

  kanEndres(fhiAdmin: User) {
    return fhiAdmin.firstName.length > 0
      && fhiAdmin.lastName.length > 0
      && this.userService.isValidPseudonym(fhiAdmin?.identityPseudonym);
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.nyFhiAdmin = null;
    this.fhiAdminSomEndres = null;
  }
}
