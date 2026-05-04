import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/models/api/User';
import { FacilityReport } from 'src/app/models/api/FacilityReport';
import { FacilityService } from 'src/app/services/data/facility.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { AuthorizationService } from '../../_common/services/authorization.service';
import { TranslateService } from '@ngx-translate/core';
import { SortHelper } from 'src/app/utils/sort-helper';

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

 
  

  facilityId: number;
  facilities: FacilityReport[];

  coordinatorsSelected: boolean;
  observerSelected: boolean;

  constructor(private readonly facilityService: FacilityService, 
              private readonly authorizationService: AuthorizationService,
              private readonly translate: TranslateService) {}

  ngOnInit(): void {
    this.coordinatorList = [];
    this.observerList = [];
    this.filteredUserList = [];
    this.emailList = [];
    this.allUsersList = [];

    this.facilityService.getFacilities().subscribe((facilities) => {
      
      for (const facility of facilities) {
        this.GetCoordinatorsForFacility(facility.id);
        this.getObserversForFacility(facility.id);
      };

      this.facilities = [ 
                            { name: '', 
                              id: null,
                              abbreviation: null,
                              type: null,
                              city: null 
                            },
                            ...facilities
        ];
    });
  }

  GetCoordinatorsForFacility(id: number) {
    this.facilityService.getCoordinators(id).subscribe((coordinators) => {
      for (const coordinator of coordinators) {
        if(coordinator.email != null && coordinator.email != "") {
          this.coordinatorList.push(coordinator);
          this.allUsersList.push(coordinator);
        }
      };
    });
  }

  getObserversForFacility(id: number) {
    this.facilityService.getObservers(id).subscribe((observers) => {
      for (const observer of observers) {
        if(observer.email != null && observer.email != "") {
          this.observerList.push(observer);
          this.allUsersList.push(observer);
        }
      };
    });
  }

  updateUserList() {

    this.filteredUserList = [];

    if (this.coordinatorsSelected) { 
      this.filteredUserList = this.allUsersList.filter(user => this.coordinatorList.includes(user) && user.userPermissions.some(userPemr => userPemr.organisationUnitId === this.facilityId));
    }
    if (this.observerSelected) { 
      this.filteredUserList = this.allUsersList.filter(user => this.observerList.includes(user) && user.userPermissions.some(userPemr => userPemr.organisationUnitId === this.facilityId));
    }
    if (this.coordinatorsSelected && this.observerSelected) {
      this.filteredUserList = this.allUsersList.filter(user => user.userPermissions.some(userPemr => userPemr.organisationUnitId === this.facilityId));
      this.filteredUserList = this.filteredUserList.toSorted((a, b) => a.lastName.localeCompare(b.lastName));
    }
  }

  updateEmailList() {
    this.emailList = [];

    for (const user of this.filteredUserList) {
      this.emailList.push(user.email)
    };
  }

  OpenEmailClient() {
    if (!this.facilityId || (!this.coordinatorsSelected && !this.observerSelected)) {
    console.warn(this.translate.instant('Please select a facility and at least one user type before opening email client.'));
    return;
  }
    this.updateEmailList();
    globalThis.location.href = `mailto:?bcc=${this.emailList.join(';')}`
  }

  sort($event: IColumnSortedEvent) {
    const userSortConfig = {
      [this.translate.instant("First name")]: (x: User) => x.firstName,
      [this.translate.instant("Last name")]: (x: User) => x.lastName,
    };
    this.updateUserList();
    this.filteredUserList = SortHelper.sort(this.filteredUserList, $event, userSortConfig);
  }
}
