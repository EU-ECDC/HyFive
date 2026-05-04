import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { HandHygieneObservation } from '../../models/api/HandHygieneObservation';
import { FacilityService } from '../../services/data/FacilityService';
import { Role } from '../../models/api/Role';
import { Uuid } from '../../utils/uuid';
import { Card } from '../../models/registration/card.model';
import { faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../constants/urls';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { HandHygieneSessionView } from 'src/app/models/registration/hand-hygiene-session-view.model';
import { HandHygieneSessionService } from 'src/app/services/data/hand-hygiene-session.service';
import { HandHygieneSession } from 'src/app/models/api/HandHygieneSession';

@Component({
  selector: 'app-register-hand-hygiene',
  templateUrl: './register-hand-hygiene.component.html',
})
export class RegisterHandHygieneComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sessionView: HandHygieneSessionView;
  sessionsdata: HandHygieneSession = null;
  roles: Role[];
  showRoleList: boolean = false;
  showEmptyForShortText: boolean = false;

  faPlus = faPlus;
  faCircle = faCircle;

  constructor(
    private readonly sessionService: HandHygieneSessionService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly facilityService: FacilityService,
    private readonly toastrService: ToastrService,
  private readonly translate: TranslateService) {
    this.facilityService
      .getSelectedFacility()
      .subscribe(i => this.roles = i?.children.find(a => a.id === this.sessionView?.department?.id)?.roles);
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

  async registerObservation(observation: HandHygieneObservation) {
    await this.sessionService.registerObservation(observation);
    this.toastrService.success(this.translate.instant("Observation was saved"));
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

  updateSessionView(sessionView: HandHygieneSessionView) {
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
