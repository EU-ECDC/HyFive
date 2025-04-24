import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { UserService } from '../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../models/api/User';
import { KeyEventService } from '../../services/events/key-event.service';
import { LoggedinUser } from '../../models/api/LoggedinUser';
import { AuthorizationService } from '../services/authorization.service';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-edit-observers',
  templateUrl: './edit-observers.component.html'
})
export class EditObserversComponent implements OnInit, OnDestroy {

  @Input() institutionId: 0;
  observers: User[];

  observerAsChanged: User = null;
  newObserver: User = null;
  user: LoggedinUser = null;
  keyword: string = '';
  filteredObservers: User[];

  constructor(private institutionService: InstitutionService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private authorizationService: AuthorizationService
  ) { }

  identityPseudonymChanged(changedIdentityPseudonym: string) {
    this.observerAsChanged.identityPseudonym = changedIdentityPseudonym;
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
    this.institutionService.getObservers(this.institutionId).subscribe(
      (observers) => {
        this.observers = observers;
        this.filteredObservers = this.observers;
      },
      (error) => this.toastrService.error('An error occurred while loading observers: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  createEmptyObserver() {
    this.cancelEdit();
    this.newObserver = {
      id: 0,
      institutionId: this.institutionId,
      lastName: '',
      firstName: '',
      email: '',
      hprNummer: null,
      identityPseudonym: null,
      timeOfCreation: new Date(),
      isDisabled: false
    };
  }

  createObserver() {
    this.userService.createObserver(this.newObserver).subscribe(
      () => this.toastrService.success('Observer created'),
      error => this.toastrService.error('An error occurred while creating observer: ' + error?.message, '', { disableTimeOut: true}),
      () => { this.newObserver = null; this.loadObservers(); }
    );
  }

  setObserverAsChanged(observer: User) {
    this.cancelEdit();
    if (this.observerAsChanged?.id == observer.id) return;
    this.observerAsChanged = JSON.parse(JSON.stringify(observer));
  }

  updateObserver(observer: User) {
    this.userService.updateObserver(observer).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('Observer updated');
        this.loadObservers();
      },
      error => this.toastrService.error('An error occurred while updating observer: ' + error?.message, '', { disableTimeOut: true}),
      () => this.observerAsChanged = null
    );
  }

  deleteObserver(observatorId: number) {

    this.userService.hasTransferredSessionToFHI(observatorId).subscribe(
      (hasTranferedSession) => { 
        if (hasTranferedSession) {
          this.toastrService.error('Observer has sessions transferred to FHI, and could not be deleted.', '', { disableTimeOut: true});
          return;
        }
        else {
          this.userService.deleteObserver(observatorId).subscribe(
            () => this.toastrService.success('Observer deleted'),
            (error) => {
              if (error.error.includes("NotSupportedException")) {
                this.toastrService.error('The observer has sessions and could not be deleted.', '', { disableTimeOut: true});
              }
              else {
                this.toastrService.error('Error deleting observer: ' + error?.message ? error.message : error, '', { disableTimeOut: true});
              }
            },
            () => this.loadObservers()
          );
        }
      },
      (error) => {
        this.toastrService.error('Error deleting observer: ' + error?.message, '', { disableTimeOut: true});
      }
    );
  }

  canCreate() {
    return this.newObserver.firstName.length > 0
      && this.newObserver.lastName.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(this.newObserver);
  }

  canChange(observer: User) {
    return observer.firstName.length > 0
      && observer.lastName.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(observer);
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.observerAsChanged = null;
    this.newObserver = null;
  }

  filterObservers(): void {
    if(this.keyword.length >= 2)
      this.filteredObservers = SearchHelper.filterUsers(this.keyword, this.observers);
    else if (this.keyword.length === 0)
      this.filteredObservers = this.observers;
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

    this.filteredObservers = this.filteredObservers.sort(sortFunc);
  }
}
