import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { LoggedinUser } from '../../models/api/LoggedinUser';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { CoordinatorForHealthcareCompanies } from '../../models/api/CoordinatorForHealthcareCompanies';
import { UserService } from '../../services/data/user.service';
import { HealthcareOrganizationService } from '../../services/data/healthcareOrganization.service';
import { InstitusjonForKoordinatorEventService } from '../../services/events/institusjon-for-koordinator-event.service';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../../_felles/services/authorization.service';
import { ObservationService } from 'src/app/services/data/observation.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';


@Component({
  selector: 'app-edit-coordinators-for-healthcareCompany',
  templateUrl: './rediger-koordinatorer-for-helseforetak.component.html'
})
export class RedigerKoordinatorerForHelseforetakComponent implements OnInit, OnDestroy {

  @Input() institution: InstitutionReport;
  koordinatorer: CoordinatorForHealthcareCompanies[];
  institusjonerIHelseforetak: InstitutionReport[];

  koordinatorSomEndres: CoordinatorForHealthcareCompanies = null;
  nyKoordinator: CoordinatorForHealthcareCompanies = null;

  dropdownSettings: IDropdownSettings;
  valgteInstitusjoner: InstitutionReport[] = [];
  user: LoggedinUser = null;
  sokeord: string = '';
  filtrertKoordinatorer: CoordinatorForHealthcareCompanies[];

  constructor(
    private healthcareOrganizationService: HealthcareOrganizationService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private institusjonForKoordinatorEventService: InstitusjonForKoordinatorEventService,
    private authorizationService: AuthorizationService,
    private observationService: ObservationService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    });

    this.lastKoordinatorer();
    this.lastInstitusjoner();

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Velg alle',
      unSelectAllText: 'Velg alle',
      itemsShowLimit: 3
    };

  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastKoordinatorer() {
    this.healthcareOrganizationService.getCoordinators(this.institution.healthcareEnterprise.id).subscribe(
      (koordinatorer) => {
        this.koordinatorer = koordinatorer;
        this.filtrertKoordinatorer = this.koordinatorer
      },
      (error) => this.toastrService.error('Det oppstod en feil under lasting av koordinatorer: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  lastInstitusjoner() {
    this.healthcareOrganizationService.getInstitutions(this.institution.healthcareEnterprise.id).subscribe(
      (institutions) => {
        this.institusjonerIHelseforetak = institutions
      },
      (error) => this.toastrService.error('Det oppstod en feil under lasting av institutions: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  opprettTomKoordinator() {
    this.cancelEdit();
    this.nullstillValgteInstitusjoner();

    this.nyKoordinator = {
      lastName: '',
      firstName: '',
      email: '',
      hprNummer: null,
      identityPseudonym: null,
      timeOfCreation: new Date(),
      isDisabled: false,
      institutions: [this.institution]
    };
  }

  createCoordinator() {
    this.nyKoordinator.institutions = this.valgteInstitusjoner;
    this.healthcareOrganizationService.createCoordinator(this.institution.healthcareEnterprise.id, this.nyKoordinator).subscribe(
      (status) => {
        if (status.suksess) {
          this.toastrService.success('Koordinator(er) og observer(er) opprettet');
          this.nyKoordinator = null;
          this.lastKoordinatorer();
        }
        else {
          this.toastrService.error(status.feilmelding, '', { disableTimeOut: true });
        }
      },
      (error) => this.toastrService.error('Det oppstod en feil under opprettelse av coordinator(er) og/eller Observatør(er): ' + error?.message, '', { disableTimeOut: true })
    );
  }

  setKoordinatorSomEndres(coordinator: CoordinatorForHealthcareCompanies) {
    if (this.erKoordinatorSomEndres(coordinator)) return;

    this.nyKoordinator = null;
    this.nullstillValgteInstitusjoner();
    let me = this;
    coordinator.institutions.forEach(function (institujon) {
      me.valgteInstitusjoner.push(institujon);
    });

    coordinator.changedHPRNumber = coordinator.hprNummer;
    coordinator.changedIdentityPseudonym = coordinator.identityPseudonym

    this.koordinatorSomEndres = JSON.parse(JSON.stringify(coordinator));
  }

  erKoordinatorSomEndres(coordinator: CoordinatorForHealthcareCompanies) {
    if (this.koordinatorSomEndres?.hprNummer?.length > 0 &&
      this.koordinatorSomEndres.hprNummer === coordinator.hprNummer)
      return true;
    if (this.koordinatorSomEndres?.identityPseudonym?.length > 0 &&
      this.koordinatorSomEndres.identityPseudonym === coordinator.identityPseudonym)
      return true;

    return false;
  }

  updateCoordinator(coordinator: CoordinatorForHealthcareCompanies) {
    coordinator.institutions = this.valgteInstitusjoner;
    let nåværendeInstitusjonErFortsattValgt = this.valgteInstitusjoner.some(i => i.id == this.institution.id);
    let erKoordinatorSomEndresLikInnloggetBruker = this.erKoordinatorSomEndresLikInnloggetBruker(coordinator);
    this.healthcareOrganizationService.updateCoordinator(this.institution.healthcareEnterprise.id, coordinator).subscribe(
      (status) => {
        if (status.suksess) {
          this.toastrService.success('Koordinator oppdatert');

          if (erKoordinatorSomEndresLikInnloggetBruker) {
            if (coordinator.isDisabled)
              this.authorizationService.logout();

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
      (error) => this.toastrService.error('En feil skjedde under oppdatering av coordinator: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  erKoordinatorSomEndresLikInnloggetBruker(coordinator: CoordinatorForHealthcareCompanies) {
    if (this.user.hprNummer && this.user.hprNummer === coordinator.hprNummer)
      return true;
    if (this.user.identityPseudonym && this.user.identityPseudonym === coordinator.identityPseudonym)
      return true;

    return false;
  }

  kanOpprettes() {
    return this.nyKoordinator.firstName.length > 0
      && this.nyKoordinator.lastName.length > 0
      && this.userService.hasCoordinatorValidHprnumberOrPseudonym(this.nyKoordinator)
      && this.valgteInstitusjoner?.length > 0;
  }

  kanEndres(coordinator: CoordinatorForHealthcareCompanies) {
    return coordinator.firstName.length > 0
      && coordinator.lastName.length > 0
      && this.userService.hasCoordinatorValidHprnumberOrPseudonym(coordinator)
      && this.valgteInstitusjoner?.length > 0;
  }

  cancelEdit($event: Event = null) {
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.koordinatorSomEndres = null;
    this.nyKoordinator = null;
  }

  visInstitusjonerForKoordinator(coordinator: CoordinatorForHealthcareCompanies): string {
    const institutions = coordinator.institutions.map(institution => institution.name);
    return institutions.toString();
  }

  nullstillValgteInstitusjoner() {
    this.valgteInstitusjoner.splice(0, this.valgteInstitusjoner.length);
  }

  identPseudonymEndret(coordinator: CoordinatorForHealthcareCompanies, identityPseudonym: string) {
    coordinator.changedIdentityPseudonym = identityPseudonym;
  }

  filtrerKoordinatorer(): void {
    if (this.sokeord.length >= 2)
    {
      this.filtrertKoordinatorer = this.koordinatorer.filter(k => 
                                    k.firstName?.toLowerCase().includes(this.sokeord.toLowerCase()) || 
                                    k.lastName?.toLocaleLowerCase().includes(this.sokeord.toLowerCase()) ||
                                    k.hprNummer?.includes(this.sokeord) ||
                                    k.institutions?.some(i => i.name.toLowerCase().includes(this.sokeord.toLowerCase())));
    }
    else if (this.sokeord.length === 0)
      this.filtrertKoordinatorer = this.koordinatorer;
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: CoordinatorForHealthcareCompanies) => any;
    switch ($event.columnName) {
      case "Firstname":
        propertyOf = (x: CoordinatorForHealthcareCompanies) => x.firstName;
        break;
      case "Lastname":
        propertyOf = (x: CoordinatorForHealthcareCompanies) => x.lastName;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: CoordinatorForHealthcareCompanies, b: CoordinatorForHealthcareCompanies) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filtrertKoordinatorer = this.filtrertKoordinatorer.sort(sortFunc);
  }
}
