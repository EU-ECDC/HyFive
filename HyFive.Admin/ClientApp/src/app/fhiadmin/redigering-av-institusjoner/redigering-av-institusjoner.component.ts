import { Component, OnInit } from '@angular/core';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { Institusjon } from '../../models/api/Institusjon';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { QueryParameters } from '../../_felles/konstanter/queryparameters';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';
import { Bruker } from 'src/app/models/api/Bruker';
import { SokHjelper } from 'src/app/utils/sokhjelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-redigering-av-institusjoner',
  templateUrl: './redigering-av-institusjoner.component.html'
})
export class RedigeringAvInstitusjonerComponent implements OnInit {

  institusjonId: number = 0;
  institusjoner: InstitusjonRapport[] = [];
  filtrertInstitusjoner: InstitusjonRapport[] = [];
  sokeord: string = '';
  sokeordPerson: string = '';
  brukere: Bruker[] = [];

  constructor(private institusjonService: InstitusjonService,
    private toastrService: ToastrService,
    private route: ActivatedRoute,
    private router: Router) { }


  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        this.institusjonId = params[QueryParameters.id] || 0;
      }
    );

    this.hentInstitusjoner();
  }

  hentInstitusjoner() {
    this.institusjonService.hentInstitusjoner().subscribe((resultat) => {
      this.institusjoner = resultat;
      this.filtrertInstitusjoner = this.institusjoner;
      
      this.institusjoner.forEach(i => {
        this.hentBrukere(i.id);
      });
    });
  }

  hentBrukere(institusjonId: number) {
    this.institusjonService.hentObservatorer(institusjonId).subscribe((resultat) => {
      this.brukere.push(...resultat);
    });
    this.institusjonService.hentKoordinatorer(institusjonId).subscribe((resultat) => {
      this.brukere.push(...resultat);
    });
  }

  navigerTilInstitusjon(institusjonId: number) {

    if (institusjonId === 0) {
      this.router.navigate([], { relativeTo: this.route });
    }

    const queryParams: Params = { id: institusjonId };
    this.router.navigate(
      [],
      {
        relativeTo: this.route,
        queryParams,
        queryParamsHandling: 'merge'
      });
  }

  oppdaterInstitusjon(institusjon: Institusjon) {
    this.institusjoner[this.institusjoner.map(i => i.id).indexOf(institusjon.id)] = institusjon;
  }

  slettInstitusjon(institusjonId: number) {
    this.hentInstitusjoner();
    this.toastrService.success('Slettet institusjon med id: ' + institusjonId, 'Institusjon slettet');
    this.navigerTilInstitusjon(0);
  }

  filtrerInstitusjoner(): void {
    if (this.sokeord.length >= 2)
      this.filtrertInstitusjoner = this.institusjoner.filter(i => i.navn.toLowerCase().includes(this.sokeord.toLowerCase()) ||
        i.helseforetak?.navn.toLowerCase().includes(this.sokeord.toLowerCase()) ||
        i.kommune?.navn.toLowerCase().includes(this.sokeord.toLowerCase()));
    else if (this.sokeord.length === 0)
      this.filtrertInstitusjoner = this.institusjoner;
  }

  filtrerPersonerPaaInstitusjoner(): void {
    if (this.sokeordPerson.length >= 2) {
      this.filtrertInstitusjoner = this.institusjoner.filter(i => {
        const personer = SokHjelper.filtrerBrukere(this.sokeordPerson, this.brukere);
        return personer.some(p => p.institusjonId === i.id);
      });
    } else if (this.sokeordPerson.length === 0) {
      this.filtrertInstitusjoner = this.institusjoner;
    }
  }

  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: Institusjon) => any;
    switch ($event.columnName) {
      case "Navn":
        propertyOf = (x: Institusjon) => x.navn;
        break;
        case "Institusjonstype":
          propertyOf = (x: Institusjon) => x.institusjontype.navn;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Institusjon, b: Institusjon) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filtrertInstitusjoner.sort(sortFunc);
  }

}
