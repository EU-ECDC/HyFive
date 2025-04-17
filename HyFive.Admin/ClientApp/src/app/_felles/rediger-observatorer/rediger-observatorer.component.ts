import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { UserService } from '../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../models/api/User';
import { KeyEventService } from '../../services/events/key-event.service';
import { InnloggetBruker } from '../../models/api/InnloggetBruker';
import { AuthorizationService } from '../services/authorization.service';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-rediger-observatorer',
  templateUrl: './rediger-observatorer.component.html'
})
export class RedigerObservatorerComponent implements OnInit, OnDestroy {

  @Input() institutionId: 0;
  observatorer: User[];

  observatorSomEndres: User = null;
  nyObservator: User = null;
  user: InnloggetBruker = null;
  sokeord: string = '';
  filtrerteObservatorer: User[];

  constructor(private institusjonService: InstitutionService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private authorizationService: AuthorizationService
  ) { }

  identPseudonymEndret(endretIdentPseudonym: string) {
    this.observatorSomEndres.identityPseudonym = endretIdentPseudonym;
  }

  ngOnInit(): void {
    this.authorizationService.getBruker().subscribe(
      (user) =>{
        this.user = user;
    });

    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });
    this.lastObservatorer();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastObservatorer() {
    this.institusjonService.hentObservatorer(this.institutionId).subscribe(
      (observatorer) => {
        this.observatorer = observatorer;
        this.filtrerteObservatorer = this.observatorer;
      },
      (error) => this.toastrService.error('Det oppstod en feil under lasting av observatører: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  opprettTomObservator() {
    this.avbrytRedigering();
    this.nyObservator = {
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
    this.userService.createObserver(this.nyObservator).subscribe(
      () => this.toastrService.success('Observatør opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av observatør: ' + error?.message, '', { disableTimeOut: true}),
      () => { this.nyObservator = null; this.lastObservatorer(); }
    );
  }

  setObservatorSomEndres(observator: User) {
    this.avbrytRedigering();
    if (this.observatorSomEndres?.id == observator.id) return;
    this.observatorSomEndres = JSON.parse(JSON.stringify(observator));
  }

  updateObserver(observator: User) {
    this.userService.updateObserver(observator).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('Observatør oppdatert');
        this.lastObservatorer();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av observatør: ' + error?.message, '', { disableTimeOut: true}),
      () => this.observatorSomEndres = null
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
            () => this.lastObservatorer()
          );
        }
      },
      (error) => {
        this.toastrService.error('Feil under sletting av observatør: ' + error?.message, '', { disableTimeOut: true});
      }
    );
  }

  kanOpprettes() {
    return this.nyObservator.firstName.length > 0
      && this.nyObservator.lastName.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(this.nyObservator);
  }

  kanEndres(observator: User) {
    return observator.firstName.length > 0
      && observator.lastName.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(observator);
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.observatorSomEndres = null;
    this.nyObservator = null;
  }

  filtrerObservatorer(): void {
    if(this.sokeord.length >= 2)
      this.filtrerteObservatorer = SearchHelper.filterUsers(this.sokeord, this.observatorer);
    else if (this.sokeord.length === 0)
      this.filtrerteObservatorer = this.observatorer;
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

    this.filtrerteObservatorer = this.filtrerteObservatorer.sort(sortFunc);
  }
}
