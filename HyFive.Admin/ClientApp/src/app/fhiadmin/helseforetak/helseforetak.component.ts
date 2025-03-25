import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import { RegionaltHelseforetakService } from 'src/app/services/data/regionalthelseforetak.service';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { OpprettHelseforetakRequest } from '../../models/api/OpprettHelseforetakRequest';
import { HelseforetakService } from '../../services/data/helseforetak.service';
import { Helseforetak } from 'src/app/models/api/Helseforetak';
import { RegionaltHelseforetak } from 'src/app/models/api/RegionaltHelseforetak';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-helseforetak',
  templateUrl: './helseforetak.component.html'
})
export class HelseforetakComponent implements OnInit, OnDestroy
{
  listAvHelseforetak: Helseforetak[] = null;
  nyHelseforetak = this.opprettTomHelseforetak();
  helseforetakSomEndres = null;
  regionaltHelseforetakListe: RegionaltHelseforetak[] = null;
  laster: boolean;

  constructor(
    private helseforetakService: HelseforetakService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private regionaltHelseforetakService: RegionaltHelseforetakService
  ) { }
  
  ngOnInit(): void {
    this.laster = true;
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering(event);
    });

    const helseforetakRequest = [
      this.helseforetakService.hentAlleHelseforetak(),
      this.regionaltHelseforetakService.hentAlleRegionaltHelseforetak()
    ];

    forkJoin(helseforetakRequest).subscribe((result) => {
      let i = 0;
      this.listAvHelseforetak = result[i++] as Helseforetak[];
      this.regionaltHelseforetakListe = result[i++] as RegionaltHelseforetak[];
      this.laster = false;
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();  
  }

  opprettTomHelseforetak() {
    return {
      navn: '',
      regionaltHelseforetakId: 0
    } as OpprettHelseforetakRequest;
  }

  opprettHelseforetak() {
    this.helseforetakService.opprettHelseforetak(this.nyHelseforetak).subscribe(
      (erOpprettet) => {
        if(erOpprettet){
          this.toastrService.success(this.nyHelseforetak.navn + " ble opprettet");
          this.nyHelseforetak = this.opprettTomHelseforetak();
          this.lastAlleHelseforetak();
        }
        else{
          this.toastrService.error("Helseforetaket finnes fra før", '', { disableTimeOut: true})
        }
      },
      () => {
        this.toastrService.error("Feil ved opprett av helseforetak", '', { disableTimeOut: true});
      }
    );
  }

  lastAlleHelseforetak() {
    this.helseforetakService.hentAlleHelseforetak().subscribe(
      (alleHelseforetak) => {
        this.listAvHelseforetak = alleHelseforetak;
      }
    );
  }

  valgtHelseforetakSomEndres(helseforetak: Helseforetak) {
    if (this.helseforetakSomEndres?.id == helseforetak.id) return;
    this.helseforetakSomEndres = JSON.parse(JSON.stringify(helseforetak));
  }

  oppdaterHelseforetak(helseforetak: Helseforetak) {
    this.helseforetakService.oppdaterHelseforetak(helseforetak).subscribe(
      (erOppdatert) => {
        if(erOppdatert) {
        this.toastrService.success(helseforetak.navn + " er oppdatert");
        this.helseforetakSomEndres = null;
        this.lastAlleHelseforetak();
        }
        else{
          this.toastrService.error(helseforetak.navn + " finnes fra før", '', { disableTimeOut: true})
        }
      }
    );
  }

  kanEndres(helseforetak: Helseforetak) {
    return helseforetak.navn.length > 0;
  }

  avbrytRedigering($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.helseforetakSomEndres = null;
  }

  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: Helseforetak) => any;
    switch ($event.columnName) {
      case "Navn":
        propertyOf = (x: Helseforetak) => x.navn.toLowerCase();
        break;
      case "Regionalt helseforetak":
        propertyOf = (x: Helseforetak) => x.regionaltHelseforetak?.navn;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Helseforetak, b: Helseforetak) => {

      let propertyOfA = propertyOf(a);
      let propertyOfB = propertyOf(b);

      if(propertyOfA == undefined) {
        return 1;
      }
      if(propertyOfB == undefined) {
        return -1;
      }

      const result = (propertyOfA < propertyOfB) ? -1 : (propertyOfA > propertyOfB) ? 1 : 0;
      return result * sortOrder;
    };

    this.listAvHelseforetak = this.listAvHelseforetak.sort(sortFunc);
  }
}











