import { Component, OnInit, OnDestroy } from '@angular/core';
import { FiveIndicationsSessionService } from '../../services/data/five-indications-session.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FiveIndicationsSessionView } from '../../models/registration/FiveIndications-session-view.model';
import { FiveIndicationsSession } from '../../models/api/FiveIndicationsSession';
import { Queryparameters } from '../../constants/queryparameters';
import { FiveIndicationsObservation } from '../../models/api/FiveIndicationsObservation';
import { FacilityService } from '../../services/data/FacilityService';
import { Role } from '../../models/api/Role';
import { Uuid } from '../../utils/uuid';
import { Card } from '../../models/registration/card.model';
import { faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../constants/urls';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register-five-indications',
  templateUrl: './register-five-indications.component.html',
})
export class RegisterFiveIndicationsComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sessionView: FiveIndicationsSessionView;
  sessionsdata: FiveIndicationsSession = null;
  roles: Role[];
  showRoleList: boolean = false;
  showEmptyForShortText: boolean = false;

  faPlus = faPlus;
  faCircle = faCircle;

  constructor(
    private readonly sessionService: FiveIndicationsSessionService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly facilityService: FacilityService,
    private readonly toastrService: ToastrService) {
    this.facilityService
      .getSelectedFacility()
      .subscribe(i => this.roles = i?.departments.find(a => a.id === this.sessionView?.department?.id)?.roles);
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

  async registerObservation(observation: FiveIndicationsObservation) {
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
    this.sessionView.card.push({ id: Uuid.generateUUID(), role: role, isActive: true });
    this.updateSessionView(this.sessionView);
    this.toggleRoleList();
  }

  updateSessionView(sessionView: FiveIndicationsSessionView) {
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

  onCloseNewCardModal(result) {
    if (result) {
      result.forEach(x => this.addNewCard(x));
    }
  }

  onDismissNewShortModal(reason) {
  }
}
