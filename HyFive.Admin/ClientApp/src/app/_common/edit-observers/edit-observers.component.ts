import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { UserService } from '../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../models/api/User';
import { KeyEventService } from '../../services/events/key-event.service';
import { LoggedInUser } from '../../models/api/LoggedInUser';
import { AuthorizationService } from '../services/authorization.service';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';
import { TranslateService } from '@ngx-translate/core';
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { SortHelper } from 'src/app/utils/sort-helper';

@Component({
  selector: 'app-edit-observers',
  templateUrl: './edit-observers.component.html'
})
export class EditObserversComponent implements OnInit, OnDestroy {

  @Input() facilityId: 0;
  observers: User[];

  observerAsChanged: User = null;
  user: LoggedInUser = null;
  keyword: string = '';
  filteredObservers: User[];
  mailValidatorHelper;
  showCreateForm: boolean = false;

  constructor(private readonly facilityService: FacilityService,
              private readonly userService: UserService,
              private readonly toastrService: ToastrService,
              private readonly keyEventService: KeyEventService,
              private readonly authorizationService: AuthorizationService,
              private readonly translate: TranslateService,
              private readonly dialogMessageService : DialogMessageService
  ) {
    this.mailValidatorHelper = MailValidatorHelper;
   }


  ngOnInit(): void {
    this.authorizationService.getUser().subscribe(
      (user) =>{
        this.user = user;
    });

    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });
    this.loadObservers();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadObservers() {
    this.facilityService.getObservers(this.facilityId).subscribe(
      (observers) => {
        this.observers = observers;
        this.filteredObservers = this.observers;
      },
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading observers:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

    toggleCreateForm() {
    this.showCreateForm = !this.showCreateForm;
    this.observerAsChanged = null;
  }

  createObserver(newObserver) {
    if (newObserver.identityPseudonym == "") {
      newObserver.identityPseudonym = null;
    }
    this.userService.createObserver(newObserver).subscribe({
      next: () => {
        this.toastrService.success(
          this.translate.instant('Observer created')
        );
      },
      error: (error) => {
        this.toastrService.error(
          this.translate.instant('An error occurred while creating observer:') +
            ' ' +
            error?.error?.message,
          '',
          { disableTimeOut: true }
        );
      },
      complete: () => {
        this.loadObservers();
        this.showCreateForm = false;
      }
    });
  }

  setObserverAsChanged(observer: User) {
    this.cancelEdit();
    if (this.observerAsChanged?.id == observer.id) return;
    this.observerAsChanged = structuredClone(observer);
  }

  updateObserver(observer: User) {
    if (observer.identityPseudonym == "") {
      observer.identityPseudonym = null;
    }
    this.userService.updateObserver(observer).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success(this.translate.instant('Observer updated'));
        this.loadObservers();
      },
      error => this.toastrService.error(this.translate.instant('An error occurred while updating observer:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
      () => this.observerAsChanged = null
    );
  }

  deleteObserver(observatorId: number) {

    this.userService.hasTransferredSessionToFHI(observatorId).subscribe(
      (hasTranferedSession) => { 
        if (hasTranferedSession) {
          this.toastrService.error(this.translate.instant('Observer has sessions transferred, and could not be deleted.'), '', { disableTimeOut: true});
        }
        else {
          this.userService.deleteObserver(observatorId).subscribe(
            () => this.toastrService.success(this.translate.instant('Observer deleted')),
            (error) => {
              if (error.error.includes("NotSupportedException")) {
                this.toastrService.error(this.translate.instant('The observer has sessions and could not be deleted.'), '', { disableTimeOut: true});
              }
              else {
                this.toastrService.error(this.translate.instant('Error deleting observer:') + ' ' + error?.error.message ? error.message : error, '', { disableTimeOut: true});
              }
            },
            () => this.loadObservers()
          );
        }
      },
      (error) => {
        this.toastrService.error(this.translate.instant('Error deleting observer:') + ' ' + error?.error.message, '', { disableTimeOut: true});
      }
    );
  }

  canChange(observer: User) {
    return observer.firstName.length > 0
      && observer.lastName.length > 0
      && observer.email.length > 0
      && this.mailValidatorHelper.validateMail(observer.email)
      && !this.observers
                      .filter(obs => obs.id !== observer.id)
                      .some(obs => obs?.email == observer?.email)
      //&& this.filteredObservers
      //                        .filter(fc => fc.id !== observer.id)
      ///                        .find(fc => fc.firstName == observer?.firstName && fc.lastName == observer?.lastName) == undefined
  }

    omitSpecialChar(event) {   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.observerAsChanged = null;
    this.showCreateForm = false;
  }

  cancelCreateEmitted() {
    this.cancelEdit();
  }

  filterObservers(): void {
    if(this.keyword.length >= 2)
      this.filteredObservers = SearchHelper.filterUsers(this.keyword, this.observers);
    else if (this.keyword.length === 0)
      this.filteredObservers = this.observers;
  }


  sort($event: IColumnSortedEvent) {
    const userSortConfig = {
      [this.translate.instant("First Name")]: (x: User) => x.firstName,
      [this.translate.instant("Last Name")]: (x: User) => x.lastName,
    };

    this.filteredObservers = SortHelper.sort(this.filteredObservers, $event, userSortConfig);
  }
}
