import { Component, OnInit, OnDestroy } from '@angular/core';
import { Role } from 'src/app/models/api/Role';
import { FacilityService } from '../../services/data/FacilityService';
import { Router, ActivatedRoute } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { Urls } from '../../constants/urls';
import { faEnvelope, faPlus } from '@fortawesome/free-solid-svg-icons';
import { faClipboard, faCircle } from '@fortawesome/free-regular-svg-icons';
import { Card } from '../../models/registration/card.model';
import { Uuid } from '../../utils/uuid';
import { ProtectiveEquipmentSessionView } from '../../models/registration/protectiveEquipment-sessionView.model';
import { ProtectiveEquipmentSession } from '../../models/api/ProtectiveEquipmentSession';
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import { ProtectiveEquipmentSessionService } from '../../services/data/protectiveEquipment-session.service';
import { MainMenuEventService } from '../../services/events/main-menu-event.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register-protective-equipment',
  templateUrl: './register-protective-equipment.component.html'
})

export class RegisterProtectiveEquipmentComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sessionView: ProtectiveEquipmentSessionView;
  sessionsdata: ProtectiveEquipmentSession = null;
  mainMenuIsOpen: boolean;
  showEmptyForShortText: boolean = false;

  roles: Role[];
  showRoleList: boolean = false;
  changeSettingMode: boolean = false;

  faPlus = faPlus;
  faClipboard = faClipboard;
  faEnvelope = faEnvelope;
  faCircle = faCircle;

  constructor(
    private readonly sessionService: ProtectiveEquipmentSessionService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly facilityService: FacilityService,
    private readonly mainMenuService: MainMenuEventService,
    private readonly toastrService: ToastrService
  ) {
    this.facilityService.getSelectedFacility()
      .subscribe(i => this.roles = i.departments.find(a => a.id === this.sessionView.department?.id)?.roles);
  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.sessionView = this.sessionService.getSessionViewForSession(sessionId);
        if (!this.sessionView) this.router.navigate(['']);
        else this.loadSessionData();
      });
      if(this.sessionView.card?.length === 0)
        this.showEmptyForShortText = true;
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  async registerObservation(observation: ProtectiveEquipmentObservation) {
    await this.sessionService.registerObservation(observation);
    this.toastrService.success("Observation was saved");
    this.loadSessionData();
  }

  loadSessionData() {
    this.sessionsdata = this.sessionService.getSession(this.sessionView.sessionId);
  }

  toggleRoleList() {
    this.showRoleList = !this.showRoleList;
  }

  addNewCard(role: Role) {
    this.sessionView.card = this.sessionView.card.map((k) => { k.isActive = false; return k })
    this.sessionView.card.push({ id: Uuid.generateUUID(), role: role, isActive: true, equipment: this.sessionView.setting.equipmentTypes });
    this.updateSessionView(this.sessionView);
    this.toggleRoleList();
  }

  updateSessionView(sessionView: ProtectiveEquipmentSessionView) {
    this.sessionView = this.sessionService.updateSessionViewForSession(sessionView);
    if(this.sessionView.card?.length === 0)
      this.showEmptyForShortText = true;
    else 
      this.showEmptyForShortText = false;
  }

  cardIsSelected(selectedCard: Card) {
    for (let i = 0; i < this.sessionView.card.length; i++) {
      if (this.sessionView.card[i] != selectedCard) {
        this.sessionView.card[i].isActive = false;
      }
    }
    this.sessionService.updateSessionViewForSession(this.sessionView);
  }

  settingEquipmentWasChanged(sessionView: ProtectiveEquipmentSessionView) {
    this.updateSessionView(sessionView);
    this.changeSettingMode = false;
  }

  onCloseNewCardModal(result) {
    if (result) {
      result.forEach(x => this.addNewCard(x));
    }
  }

  onDismissNewShortModal(reason) {
  }
}
