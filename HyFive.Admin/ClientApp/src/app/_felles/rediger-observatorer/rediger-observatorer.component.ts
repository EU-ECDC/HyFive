import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { BrukerService } from '../../services/data/bruker.service';
import { ToastrService } from 'ngx-toastr';
import { Bruker } from '../../models/api/Bruker';
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
  observatorer: Bruker[];

  observatorSomEndres: Bruker = null;
  nyObservator: Bruker = null;
  bruker: InnloggetBruker = null;
  sokeord: string = '';
  filtrerteObservatorer: Bruker[];

  constructor(private institusjonService: InstitusjonService,
    private brukerService: BrukerService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private authorizationService: AuthorizationService
  ) { }

  identPseudonymEndret(endretIdentPseudonym: string) {
    this.observatorSomEndres.identPseudonym = endretIdentPseudonym;
  }

  ngOnInit(): void {
    this.authorizationService.getBruker().subscribe(
      (bruker) =>{
        this.bruker = bruker;
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

  opprettObservator() {
    this.brukerService.opprettObservator(this.nyObservator).subscribe(
      () => this.toastrService.success('Observatør opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av observatør: ' + error?.message, '', { disableTimeOut: true}),
      () => { this.nyObservator = null; this.lastObservatorer(); }
    );
  }

  setObservatorSomEndres(observator: Bruker) {
    this.avbrytRedigering();
    if (this.observatorSomEndres?.id == observator.id) return;
    this.observatorSomEndres = JSON.parse(JSON.stringify(observator));
  }

  oppdaterObservator(observator: Bruker) {
    this.brukerService.oppdaterObservator(observator).subscribe(
      (oppdatertBruker) => {
        this.toastrService.success('Observatør oppdatert');
        this.lastObservatorer();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av observatør: ' + error?.message, '', { disableTimeOut: true}),
      () => this.observatorSomEndres = null
    );
  }

  slettObservator(observatorId: number) {

    this.brukerService.harOverfortSesjonTilFHI(observatorId).subscribe(
      (harOverfortSesjon) => { 
        if (harOverfortSesjon) {
          this.toastrService.error('Observatøren har sesjoner overført til FHI, og kunne ikke slettes.', '', { disableTimeOut: true});
          return;
        }
        else {
          this.brukerService.slettObservator(observatorId).subscribe(
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
      && this.brukerService.harGyldigHprnummerEllerPseudonym(this.nyObservator);
  }

  kanEndres(observator: Bruker) {
    return observator.fornavn.length > 0
      && observator.etternavn.length > 0
      && this.brukerService.harGyldigHprnummerEllerPseudonym(observator);
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

    this.filtrerteObservatorer = this.filtrerteObservatorer.sort(sortFunc);
  }
}
