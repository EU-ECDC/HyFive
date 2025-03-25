import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { InnloggetBruker } from '../../models/api/InnloggetBruker';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';
import { KoordinatorForHelseforetak } from '../../models/api/KoordinatorForHelseforetak';
import { BrukerService } from '../../services/data/bruker.service';
import { HelseforetakService } from '../../services/data/helseforetak.service';
import { InstitusjonForKoordinatorEventService } from '../../services/events/institusjon-for-koordinator-event.service';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../../_felles/services/authorization.service';
import { ObservasjonService } from 'src/app/services/data/observasjon.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';


@Component({
  selector: 'app-rediger-koordinatorer-for-helseforetak',
  templateUrl: './rediger-koordinatorer-for-helseforetak.component.html'
})
export class RedigerKoordinatorerForHelseforetakComponent implements OnInit, OnDestroy {

  @Input() institusjon: InstitusjonRapport;
  koordinatorer: KoordinatorForHelseforetak[];
  institusjonerIHelseforetak: InstitusjonRapport[];

  koordinatorSomEndres: KoordinatorForHelseforetak = null;
  nyKoordinator: KoordinatorForHelseforetak = null;

  dropdownSettings: IDropdownSettings;
  valgteInstitusjoner: InstitusjonRapport[] = [];
  bruker: InnloggetBruker = null;
  sokeord: string = '';
  filtrertKoordinatorer: KoordinatorForHelseforetak[];

  constructor(
    private helseforetakService: HelseforetakService,
    private brukerService: BrukerService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private institusjonForKoordinatorEventService: InstitusjonForKoordinatorEventService,
    private authorizationService: AuthorizationService,
    private observasjonService: ObservasjonService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.authorizationService.getBruker().subscribe((bruker) => {
      this.bruker = bruker;
    });

    this.lastKoordinatorer();
    this.lastInstitusjoner();

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'navn',
      selectAllText: 'Velg alle',
      unSelectAllText: 'Velg alle',
      itemsShowLimit: 3
    };

  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastKoordinatorer() {
    this.helseforetakService.hentKoordinatorer(this.institusjon.helseforetak.id).subscribe(
      (koordinatorer) => {
        this.koordinatorer = koordinatorer;
        this.filtrertKoordinatorer = this.koordinatorer
      },
      (error) => this.toastrService.error('Det oppstod en feil under lasting av koordinatorer: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  lastInstitusjoner() {
    this.helseforetakService.hentInstitusjoner(this.institusjon.helseforetak.id).subscribe(
      (institusjoner) => {
        this.institusjonerIHelseforetak = institusjoner
      },
      (error) => this.toastrService.error('Det oppstod en feil under lasting av institusjoner: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  opprettTomKoordinator() {
    this.avbrytRedigering();
    this.nullstillValgteInstitusjoner();

    this.nyKoordinator = {
      etternavn: '',
      fornavn: '',
      epost: '',
      hprNummer: null,
      identPseudonym: null,
      opprettettidspunkt: new Date(),
      erDeaktivert: false,
      institusjoner: [this.institusjon]
    };
  }

  opprettKoordinator() {
    this.nyKoordinator.institusjoner = this.valgteInstitusjoner;
    this.helseforetakService.opprettKoordinator(this.institusjon.helseforetak.id, this.nyKoordinator).subscribe(
      (status) => {
        if (status.suksess) {
          this.toastrService.success('Koordinator(er) og observatør(er) opprettet');
          this.nyKoordinator = null;
          this.lastKoordinatorer();
        }
        else {
          this.toastrService.error(status.feilmelding, '', { disableTimeOut: true });
        }
      },
      (error) => this.toastrService.error('Det oppstod en feil under opprettelse av koordinator(er) og/eller Observatør(er): ' + error?.message, '', { disableTimeOut: true })
    );
  }

  setKoordinatorSomEndres(koordinator: KoordinatorForHelseforetak) {
    if (this.erKoordinatorSomEndres(koordinator)) return;

    this.nyKoordinator = null;
    this.nullstillValgteInstitusjoner();
    let me = this;
    koordinator.institusjoner.forEach(function (institujon) {
      me.valgteInstitusjoner.push(institujon);
    });

    koordinator.endretHPRNummer = koordinator.hprNummer;
    koordinator.endretIdentPseudonym = koordinator.identPseudonym

    this.koordinatorSomEndres = JSON.parse(JSON.stringify(koordinator));
  }

  erKoordinatorSomEndres(koordinator: KoordinatorForHelseforetak) {
    if (this.koordinatorSomEndres?.hprNummer?.length > 0 &&
      this.koordinatorSomEndres.hprNummer === koordinator.hprNummer)
      return true;
    if (this.koordinatorSomEndres?.identPseudonym?.length > 0 &&
      this.koordinatorSomEndres.identPseudonym === koordinator.identPseudonym)
      return true;

    return false;
  }

  oppdaterKoordinator(koordinator: KoordinatorForHelseforetak) {
    koordinator.institusjoner = this.valgteInstitusjoner;
    let nåværendeInstitusjonErFortsattValgt = this.valgteInstitusjoner.some(i => i.id == this.institusjon.id);
    let erKoordinatorSomEndresLikInnloggetBruker = this.erKoordinatorSomEndresLikInnloggetBruker(koordinator);
    this.helseforetakService.oppdaterKoordinator(this.institusjon.helseforetak.id, koordinator).subscribe(
      (status) => {
        if (status.suksess) {
          this.toastrService.success('Koordinator oppdatert');

          if (erKoordinatorSomEndresLikInnloggetBruker) {
            if (koordinator.erDeaktivert)
              this.authorizationService.loggUt();

            if (nåværendeInstitusjonErFortsattValgt)
              this.institusjonForKoordinatorEventService.oppdaterInstitusjonsListe.emit();
          }

          if (erKoordinatorSomEndresLikInnloggetBruker && !nåværendeInstitusjonErFortsattValgt)
            window.location.reload();
          else {
            this.koordinatorSomEndres = null;
            this.lastKoordinatorer();
          }

          this.institusjonForKoordinatorEventService.oppdaterInstitusjonsListe.emit();
        }
        else {
          this.toastrService.error(status.feilmelding, '', { disableTimeOut: true });
        }
      },
      (error) => this.toastrService.error('En feil skjedde under oppdatering av koordinator: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  erKoordinatorSomEndresLikInnloggetBruker(koordinator: KoordinatorForHelseforetak) {
    if (this.bruker.hprNummer && this.bruker.hprNummer === koordinator.hprNummer)
      return true;
    if (this.bruker.identPseudonym && this.bruker.identPseudonym === koordinator.identPseudonym)
      return true;

    return false;
  }

  kanOpprettes() {
    return this.nyKoordinator.fornavn.length > 0
      && this.nyKoordinator.etternavn.length > 0
      && this.brukerService.harKoordinatorGyldigHprnummerEllerPseudonym(this.nyKoordinator)
      && this.valgteInstitusjoner?.length > 0;
  }

  kanEndres(koordinator: KoordinatorForHelseforetak) {
    return koordinator.fornavn.length > 0
      && koordinator.etternavn.length > 0
      && this.brukerService.harKoordinatorGyldigHprnummerEllerPseudonym(koordinator)
      && this.valgteInstitusjoner?.length > 0;
  }

  avbrytRedigering($event: Event = null) {
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.koordinatorSomEndres = null;
    this.nyKoordinator = null;
  }

  visInstitusjonerForKoordinator(koordinator: KoordinatorForHelseforetak): string {
    const institusjoner = koordinator.institusjoner.map(institusjon => institusjon.navn);
    return institusjoner.toString();
  }

  nullstillValgteInstitusjoner() {
    this.valgteInstitusjoner.splice(0, this.valgteInstitusjoner.length);
  }

  identPseudonymEndret(koordinator: KoordinatorForHelseforetak, identPseudonym: string) {
    koordinator.endretIdentPseudonym = identPseudonym;
  }

  filtrerKoordinatorer(): void {
    if (this.sokeord.length >= 2)
    {
      this.filtrertKoordinatorer = this.koordinatorer.filter(k => 
                                    k.fornavn?.toLowerCase().includes(this.sokeord.toLowerCase()) || 
                                    k.etternavn?.toLocaleLowerCase().includes(this.sokeord.toLowerCase()) ||
                                    k.hprNummer?.includes(this.sokeord) ||
                                    k.institusjoner?.some(i => i.navn.toLowerCase().includes(this.sokeord.toLowerCase())));
    }
    else if (this.sokeord.length === 0)
      this.filtrertKoordinatorer = this.koordinatorer;
  }

  sorter($event: IColumnSortedEvent) {
    let propertyOf: (x: KoordinatorForHelseforetak) => any;
    switch ($event.columnName) {
      case "Fornavn":
        propertyOf = (x: KoordinatorForHelseforetak) => x.fornavn;
        break;
      case "Etternavn":
        propertyOf = (x: KoordinatorForHelseforetak) => x.etternavn;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: KoordinatorForHelseforetak, b: KoordinatorForHelseforetak) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filtrertKoordinatorer = this.filtrertKoordinatorer.sort(sortFunc);
  }
}
