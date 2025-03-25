import { Component, OnInit } from '@angular/core';
import { Avdeling } from '../../models/api/Avdeling';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Klinikk } from '../../models/api/Klinikk';
import { KlinikkService } from '../../services/data/klinikk.service';
import { QueryParameters } from "../../_felles/konstanter/queryparameters";
import { Institusjon } from '../../models/api/Institusjon';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-redigering-av-klinikker',
  templateUrl: './redigering-av-klinikker.component.html'
})
export class RedigeringAvKlinikkerComponent implements OnInit {

  klinikker: Klinikk[] = [];
  institusjonNavn: string;
  institusjonId: number;
  klinikkId = 0;
  klinikkSomRedigeres: Klinikk;

  laster: boolean = false;

  constructor(private institusjonService: InstitusjonService,
    private klinikkService: KlinikkService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.hentKlinikker();
  }

  hentKlinikker() {
    this.laster = true;
    let valgtInstitusjonsId = this.institusjonService.hentValgtInstitusjonId();
    this.institusjonService.hentInstitusjon(valgtInstitusjonsId).subscribe((result: Institusjon) => {
      this.institusjonNavn = result.navn;
      this.institusjonId = result.id;
      this.klinikkService.hentKlinikkerForInstitusjon(this.institusjonId).subscribe(klinikker => {
        this.laster = false;
        this.klinikker = klinikker;
        this.route.queryParams.subscribe(
          params => {
            const klinikkIdFromQuery = params[QueryParameters.id] || 0;
            this.klinikkId = parseInt(klinikkIdFromQuery, 0);
            this.klinikkSomRedigeres = this.klinikker.find(a => a.id === this.klinikkId);
          }
        );
      });
    });
  }

  hentAvdelingsnavn(klinikk: Klinikk) {
    return klinikk.avdelinger?.map(r => r.navn).join(',');
  }

  navigerTilKlinikk(id: number) {
    if (id === 0) {
      this.router.navigate([], { relativeTo: this.route });
    }

    const queryParams: Params = { id };
    this.router.navigate(
      [],
      {
        relativeTo: this.route,
        queryParams,
        queryParamsHandling: 'merge'
      });
  }

  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: Klinikk) => any;
    switch ($event.columnName) {
      case "Navn":
        propertyOf = (x: Klinikk) => x.navn;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Klinikk, b: Klinikk) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.klinikker = this.klinikker.sort(sortFunc);
  }
}
