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
    this.institutionService.hentObservatorer(this.institutionId).subscribe(
      (observers) => {
        this.observers = observers;
        this.filteredObservers = this.observers;
      },
      (error) => this.toastrService.error('Det oppstod en feil under lasting av observatører: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  opprettTomObservator() {
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
      () => this.toastrService.success('Observatør opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av observatør: ' + error?.message, '', { disableTimeOut: true}),
      () => { this.newObserver = null; this.loadObservers(); }
    );
  }

  setObservatorSomEndres(observator: User) {
    this.cancelEdit();
    if (this.observerAsChanged?.id == observator.id) return;
    this.observerAsChanged = JSON.parse(JSON.stringify(observator));
  }

  updateObserver(observator: User) {
    this.userService.updateObserver(observator).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('Observatør oppdatert');
        this.loadObservers();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av observatør: ' + error?.message, '', { disableTimeOut: true}),
      () => this.observerAsChanged = null
    );
  }

  deleteObserver(observatorId: number) {

    this.userService.hasTransferredSessionToFHI(observatorId).subscribe(
      (harOverfortSesjon) => { 
        if (harOverfortSesjon) {
          this.toastrService.error('Observatøren har sesjoner overført til FHI, og kunne ikke slettes.', '', { disableTimeOut: true});
          return;
        }
        else {
          this.userService.deleteObserver(observatorId).subscribe(
            () => this.toastrService.success('Observatør slettet'),
            (error) => {
              if (error.error.includes("NotSupportedException")) {
                this.toastrService.error('Observatøren har sesjoner, og kunne ikke slettes.', '', { disableTimeOut: true});
              }
              else {
                this.toastrService.error('Feil under sletting av observatør: ' + error?.message ? error.message : error, '', { disableTimeOut: true});
              }
            },
            () => this.loadObservers()
          );
        }
      },
      (error) => {
        this.toastrService.error('Feil under sletting av observatør: ' + error?.message, '', { disableTimeOut: true});
      }
    );
  }

  kanOpprettes() {
    return this.newObserver.firstName.length > 0
      && this.newObserver.lastName.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(this.newObserver);
  }

  kanEndres(observator: User) {
    return observator.firstName.length > 0
      && observator.lastName.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(observator);
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.observerAsChanged = null;
    this.newObserver = null;
  }

  filtrerObservatorer(): void {
    if(this.keyword.length >= 2)
      this.filteredObservers = SearchHelper.filterUsers(this.keyword, this.observers);
    else if (this.keyword.length === 0)
      this.filteredObservers = this.observers;
  }


  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: User) => any;
    switch ($event.columnName) {
      case "Fornavn":
        propertyOf = (x: User) => x.firstName;
        break;
      case "Etternavn":
        propertyOf = (x: User) => x.lastName;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: User, b: User) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filteredObservers = this.filteredObservers.sort(sortFunc);
  }
}
