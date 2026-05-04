import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { UserService } from '../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../models/api/User';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../services/authorization.service';
import { AuthorizedRole } from '../authorization/authorized-role';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';
import { TranslateService } from '@ngx-translate/core';
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { CreateCoordinatorRequest } from 'src/app/models/api/CreateCoordinatorRequest';

@Component({
  selector: 'app-edit-coordinators',
  templateUrl: './editCoordinators.component.html'
})
export class EditCoordinatorsComponent implements OnInit, OnDestroy {

  @Input() facilityId: 0;
  coordinators: User[];

  coordinatorIsChanged: User = null;
  canDelete = false;
  searchWord: string = '';
  filteredCoordinators: User[];
  mailValidatorHelper;
  showCreateForm: boolean = false;

  constructor(
    private readonly facilityService: FacilityService,
    private readonly userService: UserService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly  authorizationService: AuthorizationService,
    private readonly translate: TranslateService,
    private readonly dialogMessageService: DialogMessageService
  ) {
    this.mailValidatorHelper = MailValidatorHelper;
   }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadCoordinators();
    this.setCanDelete();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  setCanDelete() {
    let role = this.authorizationService.getSelectedRole();
    this.canDelete = (role === AuthorizedRole.Administrator);
  }

  loadCoordinators() {
    this.facilityService.getCoordinators(this.facilityId).subscribe(
      (coordinators) => {
        this.coordinators = coordinators;
        this.filteredCoordinators = this.coordinators;
      },
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading the coordinators:') + ' ' + error?.error.message, '', { disableTimeOut: true }),
    );
  }

  toggleCreateForm() {
    this.showCreateForm = !this.showCreateForm;
    this.coordinatorIsChanged = null;
  }

  createCoordinator(newCoordinator: User): void {
    if (newCoordinator.identityPseudonym === '') {
      newCoordinator.identityPseudonym = null;
    }

    const createCoordinatorRequest: CreateCoordinatorRequest= {
        id: newCoordinator.id,
        facilityId: this.facilityId,
        firstName: newCoordinator.firstName,
        lastName: newCoordinator.lastName,
        email: newCoordinator.email,
        identityPseudonym: newCoordinator.identityPseudonym,
        isDeactivated: newCoordinator.isDeactivated
    }

    this.userService.createCoordinator(createCoordinatorRequest).subscribe({
      next: () => {
        this.toastrService.success(
          this.translate.instant('Coordinator and observer created')
        );
      },
      error: (error) => {
        this.toastrService.error(
          this.translate.instant('An error occurred while creating a coordinator or observer:') +
            ' ' +
            error?.error?.message,
          '',
          { disableTimeOut: true }
        );
      },
      complete: () => {
        newCoordinator = null;
        this.loadCoordinators();
        this.showCreateForm = false;
      }
    });
  }

  setCoordinatorAsChanged(coordinator: User) {
    if (this.coordinatorIsChanged?.id == coordinator.id) return;
    this.cancelEdit();
    this.coordinatorIsChanged = structuredClone(coordinator);
  }

  updateCoordinator(coordinator: User) {
    if (coordinator.identityPseudonym == "") {
        coordinator.identityPseudonym = null;
    }
    const updateCoordinatorRequest: CreateCoordinatorRequest= {
      id: coordinator.id,
      facilityId: this.facilityId,
      firstName: coordinator.firstName,
      lastName: coordinator.lastName,
      email: coordinator.email,
      identityPseudonym: coordinator.identityPseudonym,
      isDeactivated: coordinator.isDeactivated
    }
    this.userService.updateCoordinator(updateCoordinatorRequest).subscribe(
      (updateUser) => {
        this.toastrService.success(this.translate.instant('Coordinator updated'));
        this.loadCoordinators();
      },
      (error) => this.toastrService.error(this.translate.instant('An error occurred while updating coordinator:') + ' ' + error?.error.message, '', { disableTimeOut: true }),
      () => this.coordinatorIsChanged = null
    );
  }

  deleteCoordinator(coordinator: User) {
    this.userService.deleteCoordinator(coordinator.id).subscribe(
      () => this.toastrService.success(this.translate.instant('Coordinator deleted')),
      (error) => {
        if (error.error.includes('NotSupportedException')) {
          this.toastrService.error(this.translate.instant('The coordinator has sessions and could not be deleted'), '', { disableTimeOut: true });
        }
        else {
          this.toastrService.error(this.translate.instant('An error occurred while deleting the coordinator:') + ' ' + error?.error.message, '', { disableTimeOut: true });
        }
      },
      () => this.loadCoordinators()
    );
  }


  canbeChanged(coordinator: User) {
    return coordinator.firstName.length > 0
      && coordinator.lastName.length > 0
     // && this.coordinators
     //                             .filter(fc => fc.id !== coordinator.id)
     //                             .find(fc => fc.firstName == coordinator?.firstName && fc.lastName == coordinator?.lastName) == undefined
      && !this.coordinators
                              .filter(fc => fc.id !== coordinator.id)
                              .some(fc => fc.email == coordinator?.email)
      && coordinator.email?.length > 0
      && this.mailValidatorHelper.validateMail(coordinator.email);
  }

  omitSpecialChar(event) {   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  cancelEdit($event: Event = null) {
    this.showCreateForm = false;
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.coordinatorIsChanged = null;
  }
  
  CancelCreateEmitted() {
    this.cancelEdit();
  }

  filterCoordinators(): void {
    if(this.searchWord.length >= 2)
      this.filteredCoordinators = SearchHelper.filterUsers(this.searchWord, this.coordinators);
    else if (this.searchWord.length === 0)
      this.filteredCoordinators = this.coordinators;
  }

  sorting($event: IColumnSortedEvent) {
    let propertyOf: (x: User) => any;
    switch ($event.columnName) {
      case this.translate.instant("First Name"):
        propertyOf = (x: User) => x.firstName;
        break;
      case this.translate.instant("Last Name"):
        propertyOf = (x: User) => x.lastName;
        break;
      default:
        throw new Error(this.translate.instant("Invalid sort column"));
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: User, b: User) => {
      const state = (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : state;
      return result * sortOrder;
    };

    this.filteredCoordinators = this.filteredCoordinators.toSorted(sortFunc);
  }
}
