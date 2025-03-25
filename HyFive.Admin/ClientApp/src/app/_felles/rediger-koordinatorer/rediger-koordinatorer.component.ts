import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { BrukerService } from '../../services/data/bruker.service';
import { ToastrService } from 'ngx-toastr';
import { Bruker } from '../../models/api/Bruker';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../services/authorization.service';
import { AuthorizedRole } from '../authorization/authorized-role';
import { SokHjelper } from 'src/app/utils/sokhjelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-rediger-koordinatorer',
  templateUrl: './rediger-koordinatorer.component.html'
})
export class RedigerKoordinatorerComponent implements OnInit, OnDestroy {

  @Input() institusjonId: 0;
  koordinatorer: Bruker[];

  koordinatorSomEndres: Bruker = null;
  nyKoordinator: Bruker = null;
  kanSlette = false;
  sokeord: string = '';
  filtrerteKoordinatorer: Bruker[];

  constructor(
    private institusjonService: InstitusjonService,
    private brukerService: BrukerService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private authorizationService: AuthorizationService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastKoordinatorer();
    this.settKanSlette();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  settKanSlette() {
    let rolle = this.authorizationService.hentValgtRolle();
    this.kanSlette = (rolle === AuthorizedRole.Administrator);
  }

  lastKoordinatorer() {
    this.institusjonService.hentKoordinatorer(this.institusjonId).subscribe(
      (koordinatorer) => {
        this.koordinatorer = koordinatorer;
        this.filtrerteKoordinatorer = this.koordinatorer;
      },
      (error) => this.toastrService.error('Det oppstod en feil under lasting av koordinatorer: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  opprettTomKoordinator() {
    this.avbrytRedigering();
    this.nyKoordinator = {
      id: 0,
      institusjonId: this.institusjonId,
      etternavn: '',
      fornavn: '',
      epost: '',
      hprNummer: null,
      identPseudonym: null,
      opprettettidspunkt: new Date(),
      erDeaktivert: false,
    };
  }

  opprettKoordinator() {
    this.brukerService.opprettKoordinator(this.nyKoordinator).subscribe(
      () => {
        this.toastrService.success('Koordinator og observatør opprettet');
      },
      (error) => this.toastrService.error('Det oppstod en feil under opprettelse av koordinator eller observator: ' + error?.message, '', { disableTimeOut: true }),
      () => { this.nyKoordinator = null; this.lastKoordinatorer(); }
    );
  }

  setKoordinatorSomEndres(koordinator: Bruker) {
    if (this.koordinatorSomEndres?.id == koordinator.id) return;
    this.avbrytRedigering();
    this.koordinatorSomEndres = JSON.parse(JSON.stringify(koordinator));
  }

  oppdaterKoordinator(koordinator: Bruker) {
    this.brukerService.oppdaterKoordinator(koordinator).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('Koordinator oppdatert');
        this.lastKoordinatorer();
      },
      (error) => this.toastrService.error('Det oppstod en feil under oppdatering av koordinator: ' + error?.message, '', { disableTimeOut: true }),
      () => this.koordinatorSomEndres = null
    );
  }

  slettKoordinator(koordinator: Bruker) {
    this.brukerService.slettKoordinator(koordinator.id).subscribe(
      () => this.toastrService.success('Koordinator slettet'),
      (error) => {
        if (error.error.includes('NotSupportedException')) {
          this.toastrService.error('Koordinatoren har sesjoner, og kunne ikke slettes', '', { disableTimeOut: true });
        }
        else {
          this.toastrService.error('Det oppstod en feil under sletting av koordinator: ' + error?.message, '', { disableTimeOut: true });
        }
      },
      () => this.lastKoordinatorer()
    );
  }

  kanOpprettes() {
    return this.nyKoordinator.fornavn.length > 0
      && this.nyKoordinator.etternavn.length > 0
      && this.brukerService.harGyldigHprnummerEllerPseudonym(this.nyKoordinator);
  }

  kanEndres(koordinator: Bruker) {
    return koordinator.fornavn.length > 0
      && koordinator.etternavn.length > 0
      && this.brukerService.harGyldigHprnummerEllerPseudonym(koordinator);
  }

  identPseudonymEndret(koordinator: Bruker, identPseudonym: string) {
    koordinator.identPseudonym = identPseudonym;
  }

  avbrytRedigering($event: Event = null) {
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.koordinatorSomEndres = null;
    this.nyKoordinator = null;
  }

  filtrerKoordinatorer(): void {
    if(this.sokeord.length >= 2)
      this.filtrerteKoordinatorer = SokHjelper.filtrerBrukere(this.sokeord, this.koordinatorer);
    else if (this.sokeord.length === 0)
      this.filtrerteKoordinatorer = this.koordinatorer;
  }

  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: Bruker) => any;
    switch ($event.columnName) {
      case "Fornavn":
        propertyOf = (x: Bruker) => x.fornavn;
        break;
      case "Etternavn":
        propertyOf = (x: Bruker) => x.etternavn;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Bruker, b: Bruker) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filtrerteKoordinatorer = this.filtrerteKoordinatorer.sort(sortFunc);
  }
}
