import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/api/User';
import { InstitutionReport } from 'src/app/models/api/InstitutionReport';
import { InstitutionService } from 'src/app/services/data/institution.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-email',
  templateUrl: './email.component.html'
})
export class EmailComponent implements OnInit {

  epostListe: string[];

  alleBrukereListe : User[];
  koordinatorListe: User[];
  observatorListe: User[];
  filtrertBrukerListe: User[];

  institutionId: number;
  institusjoner: InstitutionReport[];

  koordinaterValgt: boolean;
  observatorValgt: boolean;

  constructor(private institutionService: InstitutionService) {}

  ngOnInit(): void {
    this.koordinatorListe = [];
    this.observatorListe = [];
    this.filtrertBrukerListe = [];
    this.epostListe = [];
    this.alleBrukereListe = [];

    this.institutionService.getInstitutions().subscribe((institusjoner) => {
      this.institusjoner = institusjoner;
      
      this.institusjoner.forEach(institusjon => {
        this.hentKoordinaterForInstitusjon(institusjon.id);
        this.hentObservatorerForInstitusjon(institusjon.id);
      });
    });
  }

  hentKoordinaterForInstitusjon(id: number) {
    this.institutionService.getCoordinators(id).subscribe((koordinatorer) => {
      koordinatorer.forEach(koordinator => {
        if(koordinator.email != null && koordinator.email != "") {
          this.koordinatorListe.push(koordinator);
          this.alleBrukereListe.push(koordinator);
        }
      });
    });
  }

  hentObservatorerForInstitusjon(id: number) {
    this.institutionService.getObservers(id).subscribe((observatorer) => {
      observatorer.forEach(observator => {
        if(observator.email != null && observator.email != "") {
          this.observatorListe.push(observator);
          this.alleBrukereListe.push(observator);
        }
      });
    });
  }

  oppdaterBrukerListe() {

    this.filtrertBrukerListe = [];

    if (this.koordinaterValgt) { 
      this.filtrertBrukerListe = this.alleBrukereListe.filter(user => this.koordinatorListe.includes(user) && user.institutionId === this.institutionId);
    }
    if (this.observatorValgt) { 
      this.filtrertBrukerListe = this.alleBrukereListe.filter(user => this.observatorListe.includes(user) && user.institutionId === this.institutionId);
    }
    if (this.koordinaterValgt && this.observatorValgt) {
      this.filtrertBrukerListe = this.alleBrukereListe.filter(user => user.institutionId === this.institutionId);
      this.filtrertBrukerListe = this.filtrertBrukerListe.sort((a, b) => a.lastName.localeCompare(b.lastName));
    }
  }

  oppdaterEpostListe() {
    this.epostListe = [];

    this.filtrertBrukerListe.forEach(user => {
      this.epostListe.push(user.email)
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

    this.oppdaterBrukerListe();
    this.filtrertBrukerListe = this.filtrertBrukerListe.sort(sortFunc);
  }
}
