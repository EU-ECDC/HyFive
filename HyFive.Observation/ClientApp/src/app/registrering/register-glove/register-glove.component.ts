import { Component, OnInit, OnDestroy } from '@angular/core';
import { Role } from 'src/app/models/api/Role';
import { FacilityService } from '../../services/data/FacilityService';
import { Router, ActivatedRoute } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { Urls } from '../../constants/urls';
import { faEnvelope, faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faClipboard } from '@fortawesome/free-regular-svg-icons';
import { Card } from '../../models/registration/card.model';
import { Uuid } from '../../utils/uuid';
import { GloveSessionView } from '../../models/registration/glove-session-view.model';
import { GloveSession } from '../../models/api/GloveSession';
import { GloveSessionService } from '../../services/data/glove-session.service';
import { GloveObservation } from '../../models/api/GloveObservation';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-register-glove',
  templateUrl: './register-glove.component.html'
})

export class RegisterGloveComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sessionView: GloveSessionView;
  sessionsdata: GloveSession = null;
  roles: Role[];
  showRoleList: boolean = false;
  showEmptyForShortText: boolean = false;

  faPlus = faPlus;
  faClipboard = faClipboard;
  faEnvelope = faEnvelope;
  faCircle = faCircle;

  constructor(
    private readonly sessionService: GloveSessionService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly toastrService: ToastrService,
    private readonly translate: TranslateService,
    private readonly facilityService: FacilityService,) {
    this.facilityService.getSelectedFacility()
      .subscribe(i => this.roles = i.children.find(a => a.id === this.sessionView.department?.id)?.roles);
  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        this.sessionView = this.sessionService.getSessionViewForSession(params[Queryparameters.SessionId] || 0);
        if (!this.sessionView) {
          this.router.navigate(['']);
        }
        else {
          this.loadSessionData();
        }
      });

      if(this.sessionView.card?.length === 0) {
        this.showEmptyForShortText = true;
      }
  }
  
  ngOnDestroy() {
    this.toastrService.clear();
  }

  async registerObservation(observation: GloveObservation) {
    await this.sessionService.registerObservation(observation);
    this.toastrService.success(this.translate.instant("Observation was saved"));
    this.loadSessionData();
  }

    toggleRoleList() {
    this.showRoleList = !this.showRoleList;
  }

  loadSessionData() {
    this.sessionsdata = this.sessionService.getSession(this.sessionView.sessionId);
  }

  cardIsSelected(selectedCard: Card) {
    for (let i = 0; i < this.sessionView.card.length; i++) {
      if (this.sessionView.card[i] != selectedCard) {
        this.sessionView.card[i].isActive = false;
      }
    }
    this.sessionService.updateSessionViewForSession(this.sessionView);
  }

  updateSessionView(sessionView: GloveSessionView) {
    this.sessionView = this.sessionService.updateSessionViewForSession(sessionView);
    if(this.sessionView.card?.length === 0)
      this.showEmptyForShortText = true;
    else 
      this.showEmptyForShortText = false;
  }

  addNewCard(role: Role) {
    this.sessionView.card = this.sessionView.card.map((k) => { k.isActive = false; return k })
    this.sessionView.card.push({ id: Uuid.generateUUID(), role: role, isActive: true });
    this.updateSessionView(this.sessionView);
    this.toggleRoleList();
  }

  onDismissNewShortModal(reason) {
  }

  onCloseNewCardModal(result) {
    if (result) {
      result.forEach(x => this.addNewCard(x));
    }
  }
}



