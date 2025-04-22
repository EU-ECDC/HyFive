import { Component, Input, OnInit } from '@angular/core';
import { Department} from '../../models/api/Department';
import { InstitutionService } from '../../services/data/institution.service';
import { DepartmentType } from "../../models/api/DepartmentType";
import { AuthorizationService } from 'src/app/_felles/services/authorization.service';
import { AuthorizedRole } from 'src/app/_felles/authorization/authorized-role';
import { DepartmentService } from 'src/app/services/data/department.service';
import { Role } from 'src/app/models/api/Role';
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
  
  @Input() institutionId: number;

  avdelinger: Department[] = [];
  filtrerteAvdelinger: Department[] = [];
  avdelingId = 0;
  avdelingSomEndres: Department;
  departmentTypes: DepartmentType[];
  roles: Role[];
  valgteRoller: Role[] = [];
  kanRedigere: boolean;
  institusjonNavn: string;
  sokeord: string;
  laster: boolean = false;
  dropdownSettings: IDropdownSettings;

  constructor(private institusjonService: InstitutionService,
    private authorizationService: AuthorizationService,
    private departmentService: DepartmentService,
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
      textField: 'name',
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
    if(rolle === AuthorizedRole.Coordinator || rolle === AuthorizedRole.Administrator)
      return true;
    return false;
  }

  hentAvdelinger() {
    let valgtInstitusjonsId = this.institutionId ?? this.institusjonService.hentValgtInstitusjonId();
    this.institusjonService.hentInstitusjon(valgtInstitusjonsId).subscribe(
      (institution) => {
        this.institusjonNavn = institution.name;
        this.institutionId = institution.id;
        this.avdelinger = institution.departments;
        this.filtrerteAvdelinger = this.avdelinger;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Lasting av institusjoner feilet', {disableTimeOut: true});
    });
  }

  hentAvdelingstyper() {
    this.departmentService.getDepartmentTypes().subscribe(
      (departmentTypes) => {
        this.departmentTypes = departmentTypes;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Lasting av departmentType feilet', {disableTimeOut: true});
    });
  }

  hentRoller() {
    this.rolleService.hentRoller().subscribe(
      (roles) => {
        this.roles = roles;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Lasting av roles feilet', {disableTimeOut: true});
    });
  }

  kanOpprette() : boolean {
    return (this.institutionId > 0 && this.avdelingId == 0 && this.kanRedigere);
  }

  hentRollebeskrivelser(avdeling: Department) {
    return avdeling.roles?.map(r => r.name).join(', ');
  }

  filtrerAvdelinger() {
    if(this.sokeord.length >= 2) {
      this.filtrerteAvdelinger = this.avdelinger.filter(a => a.name.toLowerCase().includes(this.sokeord.toLowerCase()) || 
                                                            a.departmentType.name.toLowerCase().includes(this.sokeord.toLowerCase()));
    }
    else if(this.sokeord.length === 0)
      this.filtrerteAvdelinger = this.avdelinger;
  }

  setAvdelingSomEndres(avdeling: Department){
    if(!this.kanRedigere || this.avdelingSomEndres?.id === avdeling.id) return;

    this.nullstillValgteRoller();
    avdeling.roles.forEach((rolle) => 
      this.valgteRoller.push(rolle)
    );
    this.avdelingSomEndres = JSON.parse(JSON.stringify(avdeling)) ;
  }

  nullstillValgteRoller() {
    this.valgteRoller.splice(0, this.valgteRoller.length);
  }

  oppdaterAvdeling(avdeling: Department): void {
    avdeling.roles = this.valgteRoller;
    this.departmentService.updateDepartment(avdeling).subscribe(
      () => {
        this.avdelingSomEndres = null;
        this.hentAvdelinger();
        this.toastrService.success("Departmentoppdatert");
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Oppdatering av avdeling feilet', { disableTimeOut: true});
      }
    );
  }

  slettAvdeling(avdeling: Department): void {
    this.departmentService.hasTransferredSessionToFHI(avdeling.id).subscribe(
      (resultat) => {
        if (resultat) 
        {
          this.toastrService.error('Avdelingen har observasjoner og kan ikke slettes', 'Sletting av avdeling feilet', { disableTimeOut: true });
        }
        else
        {
          this.departmentService.deleteDepartment(avdeling.id).subscribe(
            () => {
              this.hentAvdelinger();
              this.toastrService.success("Departmentslettet");
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
    if(this.avdelingSomEndres?.name.length > 0 && this.avdelingSomEndres?.departmentTypeId > 0 && this.valgteRoller?.length > 0)
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
    let propertyOf: (x: Department) => any;
    switch ($event.columnName) {
      case "Navn":
        propertyOf = (x: Department) => x.name;
        break;
      case "Avdelingstype":
        propertyOf = (x: Department) => x.departmentType.name;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Department, b: Department) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filtrerteAvdelinger = this.filtrerteAvdelinger.sort(sortFunc);
  }

}
