import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../../models/api/User';
import { CreateAdminRequest } from '../../../models/api/CreateAdminRequest';
import { KeyEventService } from '../../../services/events/key-event.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';
import { TranslateService } from '@ngx-translate/core';
import { SortHelper } from 'src/app/utils/sort-helper';

@Component({
  selector: 'app-edit-admin',
  templateUrl: './edit-admin.component.html',
  styleUrls: ['./edit-admin.component.scss']
})
export class EditAdminComponent implements OnInit, OnDestroy {

  users: User[];
  fhiAdminAsChanged: User = null;
  newFhiAdmin: CreateAdminRequest = null;
  filteredAdmins: User[] = [];
  mailValidatorHelper;

  constructor(
    private readonly userService: UserService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly translate: TranslateService
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
    this.userService.getAdmin().subscribe(
      (fhiAdmins) => {
        this.users = fhiAdmins;
        this.filteredAdmins = this.users;
      },
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading Admin:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  createToAdmin() {
    this.cancelEdit();
    this.newFhiAdmin = {
      lastName: '',
      firstName: '',
      email: '',
      identityPseudonym: null,
    } as CreateAdminRequest;
  }

  createAdmin() {
    this.userService.createAdmin(this.newFhiAdmin).subscribe(
      () => this.toastrService.success(this.translate.instant('Admin created')),
      error => this.toastrService.error(this.translate.instant('An error occurred while creating Admin:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
      () => { this.newFhiAdmin = null; this.loadAdmin(); }
    );
  }

  setFhiAdminAsChanged(fhiAdmin: User) {
    this.cancelEdit();
    if (this.fhiAdminAsChanged?.id == fhiAdmin.id) return;
    this.fhiAdminAsChanged = structuredClone(fhiAdmin);
  }

  updateAdmin(fhiAdmin: User) {
    this.userService.updateAdmin(fhiAdmin).subscribe(
      (updatedUser) => {
        this.toastrService.success(this.translate.instant('Admin updated'));
        this.loadAdmin();
      },
      error => this.toastrService.error(this.translate.instant('An error occurred while updating Admin:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
      () => this.fhiAdminAsChanged = null
    );
  }

  canCreate() {
    return this.newFhiAdmin.firstName?.length > 0
      && this.newFhiAdmin.lastName?.length > 0
      && !this.users.some(fc => fc.email == this.newFhiAdmin?.email)
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
      const userSortConfig = {
        [this.translate.instant("First name")]: (x: User) => x.firstName,
        [this.translate.instant("Last name")]: (x: User) => x.lastName,
      };
      this.filteredAdmins = SortHelper.sort(this.filteredAdmins, $event, userSortConfig);
    }
}
