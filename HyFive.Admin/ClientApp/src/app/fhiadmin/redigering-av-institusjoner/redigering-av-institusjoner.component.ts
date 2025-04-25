import { Component, OnInit } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { Institution } from '../../models/api/Institution';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { QueryParameters } from '../../_felles/konstanter/queryparameters';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { User } from 'src/app/models/api/User';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-redigering-av-institusjoner',
  templateUrl: './redigering-av-institusjoner.component.html'
})
export class RedigeringAvInstitusjonerComponent implements OnInit {

  institutionId: number = 0;
  institutions: InstitutionReport[] = [];
  filtrertInstitusjoner: InstitutionReport[] = [];
  sokeord: string = '';
  sokeordPerson: string = '';
  users: User[] = [];

  constructor(private institutionService: InstitutionService,
    private toastrService: ToastrService,
    private route: ActivatedRoute,
    private router: Router) { }


  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        this.institutionId = params[QueryParameters.id] || 0;
      }
    );

    this.getInstitutions();
  }

  getInstitutions() {
    this.institutionService.getInstitutions().subscribe((resultat) => {
      this.institutions = resultat;
      this.filtrertInstitusjoner = this.institutions;
      
      this.institutions.forEach(i => {
        this.hentBrukere(i.id);
      });
    });
  }

  hentBrukere(institutionId: number) {
    this.institutionService.getObservers(institutionId).subscribe((resultat) => {
      this.users.push(...resultat);
    });
    this.institutionService.getCoordinators(institutionId).subscribe((resultat) => {
      this.users.push(...resultat);
    });
  }

  navigerTilInstitusjon(institutionId: number) {

    if (institutionId === 0) {
      this.router.navigate([], { relativeTo: this.route });
    }

    const queryParams: Params = { id: institutionId };
    this.router.navigate(
      [],
      {
        relativeTo: this.route,
        queryParams,
        queryParamsHandling: 'merge'
      });
  }

  updateInstitution(institusjon: Institution) {
    this.institutions[this.institutions.map(i => i.id).indexOf(institusjon.id)] = institusjon;
  }

  deleteInstitution(institutionId: number) {
    this.getInstitutions();
    this.toastrService.success('Slettet institusjon med id: ' + institutionId, 'Institution slettet');
    this.navigerTilInstitusjon(0);
  }

  filtrerInstitusjoner(): void {
    if (this.sokeord.length >= 2)
      this.filtrertInstitusjoner = this.institutions.filter(i => i.name.toLowerCase().includes(this.sokeord.toLowerCase()) ||
        i.healthcareEnterprise?.name.toLowerCase().includes(this.sokeord.toLowerCase()) ||
        i.municipality?.name.toLowerCase().includes(this.sokeord.toLowerCase()));
    else if (this.sokeord.length === 0)
      this.filtrertInstitusjoner = this.institutions;
  }

  filtrerPersonerPaaInstitusjoner(): void {
    if (this.sokeordPerson.length >= 2) {
      this.filtrertInstitusjoner = this.institutions.filter(i => {
        const personer = SearchHelper.filterUsers(this.sokeordPerson, this.users);
        return personer.some(p => p.institutionId === i.id);
      });
    } else if (this.sokeordPerson.length === 0) {
      this.filtrertInstitusjoner = this.institutions;
    }
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: Institution) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: Institution) => x.name;
        break;
        case "Institutiontype":
          propertyOf = (x: Institution) => x.institutionType.name;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Institution, b: Institution) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filtrertInstitusjoner.sort(sortFunc);
  }

}
