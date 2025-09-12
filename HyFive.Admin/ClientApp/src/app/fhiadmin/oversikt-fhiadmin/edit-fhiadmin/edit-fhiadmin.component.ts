import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../../models/api/User';
import { CreateFhiAdminRequest } from '../../../models/api/CreateFhiAdminRequest';
import { KeyEventService } from '../../../services/events/key-event.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';

@Component({
  selector: 'app-edit-fhiadmin',
  templateUrl: './edit-fhiadmin.component.html'
})
export class EditFhiAdminComponent implements OnInit, OnDestroy {

  users: User[];
  fhiAdminAsChanged: User = null;
  newFhiAdmin: CreateFhiAdminRequest = null;
  filteredAdmins: User[] = [];
  mailValidatorHelper;

  constructor(
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) {
    this.mailValidatorHelper = MailValidatorHelper;
   }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });
    this.loadAdmin();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  loadAdmin() {
    this.userService.getFhiAdmin().subscribe(
      (fhiAdmins) => {
        this.users = fhiAdmins;
        this.filteredAdmins = this.users;
      },
      (error) => this.toastrService.error('An error occurred while loading Admin: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  createToAdmin() {
    this.cancelEdit();
    this.newFhiAdmin = {
      lastName: '',
      firstName: '',
      email: '',
      identityPseudonym: null,
    } as CreateFhiAdminRequest;
  }

  createAdmin() {
    this.userService.createAdmin(this.newFhiAdmin).subscribe(
      () => this.toastrService.success('Admin create'),
      error => this.toastrService.error('An error occurred while creating Admin: ' + error?.error, '', { disableTimeOut: true}),
      () => { this.newFhiAdmin = null; this.loadAdmin(); }
    );
  }

  setFhiAdminAsChanged(fhiAdmin: User) {
    this.cancelEdit();
    if (this.fhiAdminAsChanged?.id == fhiAdmin.id) return;
    this.fhiAdminAsChanged = JSON.parse(JSON.stringify(fhiAdmin));
  }

  updateFhiAdmin(fhiAdmin: User) {
    this.userService.updateFhiAdmin(fhiAdmin).subscribe(
      (updatedUser) => {
        this.toastrService.success('Admin updated');
        this.loadAdmin();
      },
      error => this.toastrService.error('An error occurred while updating Admin: ' + error?.error, '', { disableTimeOut: true}),
      () => this.fhiAdminAsChanged = null
    );
  }

  canCreate() {
    return this.newFhiAdmin.firstName?.length > 0
      && this.newFhiAdmin.lastName?.length > 0
      && this.users.find(fc => fc.email == this.newFhiAdmin?.email) == undefined
      && this.newFhiAdmin.email?.length > 0
      && this.mailValidatorHelper.validateMail(this.newFhiAdmin?.email)
  }

  canChange(fhiAdmin: User) {
    return fhiAdmin.firstName.length > 0
      && fhiAdmin.lastName.length > 0
      // && fhiAdmin.email?.length > 0
      //       && this.users
      //                   .filter(fc => fc.id !== fhiAdmin.id)
      //                   .find(fc => fc.email == fhiAdmin?.email) == undefined
      // && this.mailValidatorHelper.validateMail(fhiAdmin?.email)
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.newFhiAdmin = null;
    this.fhiAdminAsChanged = null;
  }

    sort($event: IColumnSortedEvent) {
      let propertyOf: (x: User) => any;
      switch ($event.columnName) {
        case "Firstname":
          propertyOf = (x: User) => x.firstName;
          break;
          case "Lastname":
            propertyOf = (x: User) => x.lastName;
          break;
        default:
          throw new Error("Invalid sort column");
      }
  
      const sortOrder = $event.sortDirection === "asc" ? 1 : -1;
  
      const sortFunc = (a: User, b: User) => {
        const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
        return result * sortOrder;
      };
  
      this.filteredAdmins.sort(sortFunc);
    }
}
