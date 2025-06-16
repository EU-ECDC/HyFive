import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { UserService } from '../../services/data/user.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../../models/api/User';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../services/authorization.service';
import { AuthorizedRole } from '../authorization/authorized-role';
import { SearchHelper } from 'src/app/utils/searchHelper';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';

@Component({
  selector: 'app-edit-coordinators',
  templateUrl: './editCoordinators.component.html'
})
export class EditCoordinatorsComponent implements OnInit, OnDestroy {

  @Input() institutionId: 0;
  coordinators: User[];

  coordinatorIsChanged: User = null;
  newCoordinator: User = null;
  canDelete = false;
  searchWord: string = '';
  filteredCoordinators: User[];

  constructor(
    private institutionService: InstitutionService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private authorizationService: AuthorizationService
  ) { }

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
    this.institutionService.getCoordinators(this.institutionId).subscribe(
      (coordinators) => {
        this.coordinators = coordinators;
        this.filteredCoordinators = this.coordinators;
      },
      (error) => this.toastrService.error('An error occurred while loading the coordinators: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  createEmptyCoordinator() {
    this.cancelEdit();
    this.newCoordinator = {
      id: 0,
      institutionId: this.institutionId,
      lastName: '',
      firstName: '',
      email: '',
      hprNumber: null,
      identityPseudonym: null,
      createdTime: new Date(),
      isDisabled: false,
    };
  }

  createCoordinator() {
    this.userService.createCoordinator(this.newCoordinator).subscribe(
      () => {
        this.toastrService.success('Coordinator and observer created');
      },
      (error) => this.toastrService.error('An error occurred while creating a coordinator or observer: ' + error?.message, '', { disableTimeOut: true }),
      () => { this.newCoordinator = null; this.loadCoordinators(); }
    );
  }

  setCoordinatorAsChanged(coordinator: User) {
    if (this.coordinatorIsChanged?.id == coordinator.id) return;
    this.cancelEdit();
    this.coordinatorIsChanged = JSON.parse(JSON.stringify(coordinator));
  }

  updateCoordinator(coordinator: User) {
    this.userService.updateCoordinator(coordinator).subscribe(
      (updateUser) => {
        this.toastrService.success('Coordinator updated');
        this.loadCoordinators();
      },
      (error) => this.toastrService.error('An error occurred while updating coordinator: ' + error?.message, '', { disableTimeOut: true }),
      () => this.coordinatorIsChanged = null
    );
  }

  deleteCoordinator(coordinator: User) {
    this.userService.deleteCoordinator(coordinator.id).subscribe(
      () => this.toastrService.success('Coordinator deleted'),
      (error) => {
        if (error.error.includes('NotSupportedException')) {
          this.toastrService.error('The coordinator has sessions and could not be deleted', '', { disableTimeOut: true });
        }
        else {
          this.toastrService.error('An error occurred while deleting the coordinator: ' + error?.message, '', { disableTimeOut: true });
        }
      },
      () => this.loadCoordinators()
    );
  }

  canBeCreated() {
    return this.newCoordinator.firstName.length > 0
      && this.newCoordinator.lastName.length > 0
      && this.filteredCoordinators.find(fc => fc.firstName == this.newCoordinator?.firstName && fc.lastName == this.newCoordinator?.lastName) == undefined
      && this.newCoordinator.email?.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(this.newCoordinator);
  }

  canbeChanged(coordinator: User) {
    return coordinator.firstName.length > 0
      && coordinator.lastName.length > 0
      && this.filteredCoordinators
                                  .filter(fc => fc.id !== coordinator.id)
                                  .find(fc => fc.firstName == coordinator?.firstName && fc.lastName == coordinator?.lastName) == undefined
      && coordinator.email?.length > 0
      && this.userService.hasValidHprnumberOrPseudonym(coordinator);
  }

  ValidateMailCharacters(event: KeyboardEvent) {
    const allowedPattern = /^[a-zA-Z0-9@]$/;
    const key = event.key;

    if (!allowedPattern.test(key)) {
      event.preventDefault();
    }
  }

    omitSpecialChar(event) {   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  identityPseudonymChanged(coordinator: User, identityPseudonym: string) {
    coordinator.identityPseudonym = identityPseudonym;
  }

  cancelEdit($event: Event = null) {
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.coordinatorIsChanged = null;
    this.newCoordinator = null;
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
      case "First name":
        propertyOf = (x: User) => x.firstName;
        break;
      case "Last name":
        propertyOf = (x: User) => x.lastName;
        break;
      default:
        throw new Error("Invalid sorting column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: User, b: User) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filteredCoordinators = this.filteredCoordinators.sort(sortFunc);
  }
}
