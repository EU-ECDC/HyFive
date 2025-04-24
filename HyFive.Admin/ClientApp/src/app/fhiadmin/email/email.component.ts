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

  emailList: string[];

  allUsersList : User[];
  coordinatorList: User[];
  observerList: User[];
  filteredUserList: User[];

  institutionId: number;
  institutions: InstitutionReport[];

  coordinatorsSelected: boolean;
  observerSelected: boolean;

  constructor(private institutionService: InstitutionService) {}

  ngOnInit(): void {
    this.coordinatorList = [];
    this.observerList = [];
    this.filteredUserList = [];
    this.emailList = [];
    this.allUsersList = [];

    this.institutionService.getInstitutions().subscribe((institutions) => {
      this.institutions = institutions;
      
      this.institutions.forEach(institution => {
        this.GetCoordinatorsForInstitution(institution.id);
        this.getObserversForInstitution(institution.id);
      });
    });
  }

  GetCoordinatorsForInstitution(id: number) {
    this.institutionService.getCoordinators(id).subscribe((coordinators) => {
      coordinators.forEach(coordinator => {
        if(coordinator.email != null && coordinator.email != "") {
          this.coordinatorList.push(coordinator);
          this.allUsersList.push(coordinator);
        }
      });
    });
  }

  getObserversForInstitution(id: number) {
    this.institutionService.getObservers(id).subscribe((observers) => {
      observers.forEach(observer => {
        if(observer.email != null && observer.email != "") {
          this.observerList.push(observer);
          this.allUsersList.push(observer);
        }
      });
    });
  }

  updateUserList() {

    this.filteredUserList = [];

    if (this.coordinatorsSelected) { 
      this.filteredUserList = this.allUsersList.filter(user => this.coordinatorList.includes(user) && user.institutionId === this.institutionId);
    }
    if (this.observerSelected) { 
      this.filteredUserList = this.allUsersList.filter(user => this.observerList.includes(user) && user.institutionId === this.institutionId);
    }
    if (this.coordinatorsSelected && this.observerSelected) {
      this.filteredUserList = this.allUsersList.filter(user => user.institutionId === this.institutionId);
      this.filteredUserList = this.filteredUserList.sort((a, b) => a.lastName.localeCompare(b.lastName));
    }
  }

  updateEmailList() {
    this.emailList = [];

    this.filteredUserList.forEach(user => {
      this.emailList.push(user.email)
    });
  }

  OpenEmailClient() {
    this.updateEmailList();
    window.location.href = `mailto:?bcc=${this.emailList.join(';')}`
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: User) => any;
    switch ($event.columnName) {
      case "Firstname":
        propertyOf = (x: User) => x.firstName;
        break;
      case "Lastname":
        propertyOf = (x: User) => x.lastName;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: User, b: User) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.updateUserList();
    this.filteredUserList = this.filteredUserList.sort(sortFunc);
  }
}
