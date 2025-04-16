import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { UserService } from '../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../models/api/User';
import { KeyEventService } from '../../services/events/key-event.service';
import { InnloggetBruker } from '../../models/api/InnloggetBruker';
import { AuthorizationService } from '../services/authorization.service';
import { SokHjelper } from 'src/app/utils/sokhjelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-rediger-observatorer',
  templateUrl: './rediger-observatorer.component.html'
})
export class RedigerObservatorerComponent implements OnInit, OnDestroy {

  @Input() institusjonId: 0;
  observatorer: User[];

  observatorSomEndres: User = null;
  nyObservator: User = null;
  user: InnloggetBruker = null;
  sokeord: string = '';
  filtrerteObservatorer: User[];

  constructor(private institusjonService: InstitusjonService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private authorizationService: AuthorizationService
  ) { }

  identPseudonymEndret(endretIdentPseudonym: string) {
    this.observatorSomEndres.identPseudonym = endretIdentPseudonym;
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
    this.institusjonService.hentObservatorer(this.institusjonId).subscribe(
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
      institusjonId: this.institusjonId,
      etternavn: '',
      fornavn: '',
      epost: '',
      hprNummer: null,
      identPseudonym: null,
      opprettettidspunkt: new Date(),
      erDeaktivert: false
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

  oppdaterObservator(observator: User) {
    this.userService.oppdaterObservator(observator).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('Observatør oppdatert');
        this.lastObservatorer();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av observatør: ' + error?.message, '', { disableTimeOut: true}),
      () => this.observatorSomEndres = null
    );
  }

  slettObservator(observatorId: number) {

    this.userService.harOverfortSesjonTilFHI(observatorId).subscribe(
      (harOverfortSesjon) => { 
        if (harOverfortSesjon) {
          this.toastrService.error('Observatøren har sesjoner overført til FHI, og kunne ikke slettes.', '', { disableTimeOut: true});
          return;
        }
        else {
          this.userService.slettObservator(observatorId).subscribe(
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
    return this.nyObservator.fornavn.length > 0
      && this.nyObservator.etternavn.length > 0
      && this.userService.harGyldigHprnummerEllerPseudonym(this.nyObservator);
  }

  kanEndres(observator: User) {
    return observator.fornavn.length > 0
      && observator.etternavn.length > 0
      && this.userService.harGyldigHprnummerEllerPseudonym(observator);
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
      this.filtrerteObservatorer = SokHjelper.filtrerBrukere(this.sokeord, this.observatorer);
    else if (this.sokeord.length === 0)
      this.filtrerteObservatorer = this.observatorer;
  }


  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: User) => any;
    switch ($event.columnName) {
      case "Fornavn":
        propertyOf = (x: User) => x.fornavn;
        break;
      case "Etternavn":
        propertyOf = (x: User) => x.etternavn;
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
