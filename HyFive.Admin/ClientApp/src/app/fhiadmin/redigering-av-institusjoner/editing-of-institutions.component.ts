import { Component, OnInit } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { Institution } from '../../models/api/Institution';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { QueryParameters } from '../../_common/konstanter/queryparameters';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { User } from 'src/app/models/api/User';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-editing-of-institutions',
  templateUrl: './editing-of-institutions.component.html'
})
export class EditingOfInstitutionsComponent implements OnInit {

  institutionId: number = 0;
  institutions: InstitutionReport[] = [];
  filteredInstitutions: InstitutionReport[] = [];
  keyword: string = '';
  keywordPerson: string = '';
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
    this.institutionService.getInstitutions().subscribe((result) => {
      this.institutions = result;
      this.filteredInstitutions = this.institutions;
      
      this.institutions.forEach(i => {
        this.getUsers(i.id);
      });
    });
  }

  getUsers(institutionId: number) {
    this.institutionService.getObservers(institutionId).subscribe((result) => {
      this.users.push(...result);
    });
    this.institutionService.getCoordinators(institutionId).subscribe((result) => {
      this.users.push(...result);
    });
  }

  navigateToInstitution(institutionId: number) {

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

  updateInstitution(institution: Institution) {
    this.institutions[this.institutions.map(i => i.id).indexOf(institution.id)] = institution;
  }

  deleteInstitution(institutionId: number) {
    this.getInstitutions();
    this.toastrService.success('Deleted institution with id: ' + institutionId, 'Institution deleted');
    this.navigateToInstitution(0);
  }

  filterInstitutions(): void {
    if (this.keyword.length >= 2)
      this.filteredInstitutions = this.institutions.filter(i => i.name.toLowerCase().includes(this.keyword.toLowerCase()) ||
        i.healthcareOrganization?.name.toLowerCase().includes(this.keyword.toLowerCase()) ||
        i.municipality?.name.toLowerCase().includes(this.keyword.toLowerCase()));
    else if (this.keyword.length === 0)
      this.filteredInstitutions = this.institutions;
  }

  filterPersonsAtinstitutions(): void {
    if (this.keywordPerson.length >= 2) {
      this.filteredInstitutions = this.institutions.filter(i => {
        const persons = SearchHelper.filterUsers(this.keywordPerson, this.users);
        return persons.some(p => p.institutionId === i.id);
      });
    } else if (this.keywordPerson.length === 0) {
      this.filteredInstitutions = this.institutions;
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

    this.filteredInstitutions.sort(sortFunc);
  }

}
