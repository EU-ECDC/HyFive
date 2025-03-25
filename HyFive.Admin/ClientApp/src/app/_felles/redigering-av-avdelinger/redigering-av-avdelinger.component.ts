import { Component, Input, OnInit } from '@angular/core';
import { Avdeling } from '../../models/api/Avdeling';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { AvdelingType } from "../../models/api/AvdelingType";
import { AuthorizationService } from 'src/app/_felles/services/authorization.service';
import { AuthorizedRole } from 'src/app/_felles/authorization/authorized-role';
import { AvdelingService } from 'src/app/services/data/avdeling.service';
import { Rolle } from 'src/app/models/api/Rolle';
import { RolleService } from 'src/app/services/data/rolle.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
@Component({
selector: 'app-redigering-av-avdelinger',
  templateUrl: './redigering-av-avdelinger.component.html'
})
export class RedigeringAvAvdelingerComponent implements OnInit {
  
  @Input() institusjonId: number;

  avdelinger: Avdeling[] = [];
  filtrerteAvdelinger: Avdeling[] = [];
  avdelingId = 0;
  avdelingSomEndres: Avdeling;
  avdelingstyper: AvdelingType[];
  roller: Rolle[];
  valgteRoller: Rolle[] = [];
  kanRedigere: boolean;
  institusjonNavn: string;
  sokeord: string;
  laster: boolean = false;
  dropdownSettings: IDropdownSettings;

  constructor(private institusjonService: InstitusjonService,
    private authorizationService: AuthorizationService,
    private avdelingService: AvdelingService,
    private rolleService: RolleService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService) { }

  ngOnInit(): void {
    this.laster = true;
    
    this.kanRedigere = this.kanBrukerRedigere();

    this.hentAvdelinger();
    this.hentAvdelingstyper();
    this.hentRoller();

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'navn',
      selectAllText: 'Velg alle',
      unSelectAllText: 'Velg alle',
      itemsShowLimit: 5
    };

    this.laster = false;
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering(event);
    });
  }

  private kanBrukerRedigere() {
    let rolle = this.authorizationService.hentValgtRolle();
    if(rolle === AuthorizedRole.Koordinator || rolle === AuthorizedRole.Administrator)
      return true;
    return false;
  }

  hentAvdelinger() {
    let valgtInstitusjonsId = this.institusjonId ?? this.institusjonService.hentValgtInstitusjonId();
    this.institusjonService.hentInstitusjon(valgtInstitusjonsId).subscribe(
      (institusjon) => {
        this.institusjonNavn = institusjon.navn;
        this.institusjonId = institusjon.id;
        this.avdelinger = institusjon.avdelinger;
        this.filtrerteAvdelinger = this.avdelinger;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Lasting av institusjoner feilet', {disableTimeOut: true});
    });
  }

  hentAvdelingstyper() {
    this.avdelingService.hentAvdelingstyper().subscribe(
      (avdelingstyper) => {
        this.avdelingstyper = avdelingstyper;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Lasting av avdelingstype feilet', {disableTimeOut: true});
    });
  }

  hentRoller() {
    this.rolleService.hentRoller().subscribe(
      (roller) => {
        this.roller = roller;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Lasting av roller feilet', {disableTimeOut: true});
    });
  }

  kanOpprette() : boolean {
    return (this.institusjonId > 0 && this.avdelingId == 0 && this.kanRedigere);
  }

  hentRollebeskrivelser(avdeling: Avdeling) {
    return avdeling.roller?.map(r => r.navn).join(', ');
  }

  filtrerAvdelinger() {
    if(this.sokeord.length >= 2) {
      this.filtrerteAvdelinger = this.avdelinger.filter(a => a.navn.toLowerCase().includes(this.sokeord.toLowerCase()) || 
                                                            a.avdelingType.navn.toLowerCase().includes(this.sokeord.toLowerCase()));
    }
    else if(this.sokeord.length === 0)
      this.filtrerteAvdelinger = this.avdelinger;
  }

  setAvdelingSomEndres(avdeling: Avdeling){
    if(!this.kanRedigere || this.avdelingSomEndres?.id === avdeling.id) return;

    this.nullstillValgteRoller();
    avdeling.roller.forEach((rolle) => 
      this.valgteRoller.push(rolle)
    );
    this.avdelingSomEndres = JSON.parse(JSON.stringify(avdeling)) ;
  }

  nullstillValgteRoller() {
    this.valgteRoller.splice(0, this.valgteRoller.length);
  }

  oppdaterAvdeling(avdeling: Avdeling): void {
    avdeling.roller = this.valgteRoller;
    this.avdelingService.oppdaterAvdeling(avdeling).subscribe(
      () => {
        this.avdelingSomEndres = null;
        this.hentAvdelinger();
        this.toastrService.success("Avdeling oppdatert");
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Oppdatering av avdeling feilet', { disableTimeOut: true});
      }
    );
  }

  slettAvdeling(avdeling: Avdeling): void {
    this.avdelingService.harOverfortSesjonTilFHI(avdeling.id).subscribe(
      (resultat) => {
        if (resultat) 
        {
          this.toastrService.error('Avdelingen har observasjoner og kan ikke slettes', 'Sletting av avdeling feilet', { disableTimeOut: true });
        }
        else
        {
          this.avdelingService.slettAvdeling(avdeling.id).subscribe(
            () => {
              this.hentAvdelinger();
              this.toastrService.success("Avdeling slettet");
            },
            (error) => {
              this.toastrService.error(error.error.message, 'Sletting av avdeling feilet', { disableTimeOut: true});
            }
          );
        }
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Sletting av avdeling feilet', { disableTimeOut: true});
      });
  }

  kanLagres(): boolean {
    if(this.avdelingSomEndres?.navn.length > 0 && this.avdelingSomEndres?.avdelingTypeId > 0 && this.valgteRoller?.length > 0)
      return true;
    else
      return false;
  }

  avbrytRedigering($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.avdelingSomEndres = null;
  }

  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: Avdeling) => any;
    switch ($event.columnName) {
      case "Navn":
        propertyOf = (x: Avdeling) => x.navn;
        break;
      case "Avdelingstype":
        propertyOf = (x: Avdeling) => x.avdelingType.navn;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Avdeling, b: Avdeling) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filtrerteAvdelinger = this.filtrerteAvdelinger.sort(sortFunc);
  }

}
