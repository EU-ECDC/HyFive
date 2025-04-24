import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Role } from '../../../models/api/Role';
import { RoleService } from '../../../services/data/role.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-roles',
  templateUrl: './redigering-av-roles.component.html'

})
export class RedigeringAvRollerComponent implements OnInit, OnDestroy {

  roles: Role[];

  rolleSomEndres: Role = null;
  nyRolle: Role = this.opprettTomRolle();
  deaktiverOpprett: boolean = true;


  constructor(
    private roleService: RoleService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastRoller();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastRoller() {
    this.roleService.getRoles().subscribe(
      (roles) => this.roles = roles,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av roles: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  opprettTomRolle() {
    return {
      id: 0,
      name: '',
      description: '',
      institusjonIder: []
    } as Role;
  }

  createRole() {
    this.roleService.createRole(this.nyRolle).subscribe(
      (opprettetRolle) => this.toastrService.success('Role opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av rolle: ' + error?.message, '', { disableTimeOut: true}),
      () => { this.nyRolle = this.opprettTomRolle(); this.lastRoller(); }
    );
  }

  valgtRolle(rolle: Role): void {
    if (this.rolleSomEndres?.id == rolle.id) return;
    this.rolleSomEndres = JSON.parse(JSON.stringify(rolle));
  }

  updateRole(rolle: Role) {
    this.roleService.updateRole(rolle).subscribe(
      (oppdatertRolle) => {
        this.toastrService.success("Role oppdatert");
        this.lastRoller();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av rolle: ' + error?.error, '', { disableTimeOut: true}),
      () => this.rolleSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.rolleSomEndres = null;
  }
}
