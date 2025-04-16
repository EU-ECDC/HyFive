import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/api/User';
import { InstitusjonRapport } from 'src/app/models/api/InstitusjonRapport';
import { InstitusjonService } from 'src/app/services/data/institusjon.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-epost',
  templateUrl: './epost.component.html'
})
export class EpostComponent implements OnInit {

  epostListe: string[];

  alleBrukereListe : User[];
  koordinatorListe: User[];
  observatorListe: User[];
  filtrertBrukerListe: User[];

  institusjonId: number;
  institusjoner: InstitusjonRapport[];

  koordinaterValgt: boolean;
  observatorValgt: boolean;

  constructor(private institusjonService: InstitusjonService) {}

  ngOnInit(): void {
    this.koordinatorListe = [];
    this.observatorListe = [];
    this.filtrertBrukerListe = [];
    this.epostListe = [];
    this.alleBrukereListe = [];

    this.institusjonService.hentInstitusjoner().subscribe((institusjoner) => {
      this.institusjoner = institusjoner;
      
      this.institusjoner.forEach(institusjon => {
        this.hentKoordinaterForInstitusjon(institusjon.id);
        this.hentObservatorerForInstitusjon(institusjon.id);
      });
    });
  }

  hentKoordinaterForInstitusjon(id: number) {
    this.institusjonService.hentKoordinatorer(id).subscribe((koordinatorer) => {
      koordinatorer.forEach(koordinator => {
        if(koordinator.epost != null && koordinator.epost != "") {
          this.koordinatorListe.push(koordinator);
          this.alleBrukereListe.push(koordinator);
        }
      });
    });
  }

  hentObservatorerForInstitusjon(id: number) {
    this.institusjonService.hentObservatorer(id).subscribe((observatorer) => {
      observatorer.forEach(observator => {
        if(observator.epost != null && observator.epost != "") {
          this.observatorListe.push(observator);
          this.alleBrukereListe.push(observator);
        }
      });
    });
  }

  oppdaterBrukerListe() {

    this.filtrertBrukerListe = [];

    if (this.koordinaterValgt) { 
      this.filtrertBrukerListe = this.alleBrukereListe.filter(user => this.koordinatorListe.includes(user) && user.institusjonId === this.institusjonId);
    }
    if (this.observatorValgt) { 
      this.filtrertBrukerListe = this.alleBrukereListe.filter(user => this.observatorListe.includes(user) && user.institusjonId === this.institusjonId);
    }
    if (this.koordinaterValgt && this.observatorValgt) {
      this.filtrertBrukerListe = this.alleBrukereListe.filter(user => user.institusjonId === this.institusjonId);
      this.filtrertBrukerListe = this.filtrertBrukerListe.sort((a, b) => a.etternavn.localeCompare(b.etternavn));
    }
  }

  oppdaterEpostListe() {
    this.epostListe = [];

    this.filtrertBrukerListe.forEach(user => {
      this.epostListe.push(user.epost)
    });
  }

  apneEpostKlient() {
    this.oppdaterEpostListe();
    window.location.href = `mailto:?bcc=${this.epostListe.join(';')}`
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

    this.oppdaterBrukerListe();
    this.filtrertBrukerListe = this.filtrertBrukerListe.sort(sortFunc);
  }
}
