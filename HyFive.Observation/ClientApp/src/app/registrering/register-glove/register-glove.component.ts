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
    private sessionService: GloveSessionService,
    private router: Router,
    private route: ActivatedRoute,
    private facilityService: FacilityService,
    private toastrService: ToastrService) {
    this.facilityService.getSelectedFacility()
      .subscribe(i => this.roles = i.departments.find(a => a.id === this.sessionView.department?.id)?.roles);
  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.sessionView = this.sessionService.getSessionViewForSession(sessionId);
        if (!this.sessionView) {
          this.router.navigate(['']);
        }
        else {
          this.loadSessionData();
        }
      });

      if(this.sessionView.card?.length === 0)
        this.showEmptyForShortText = true;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  async registerObservation(observation: GloveObservation) {
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

  updateSessionView(sessionView: GloveSessionView) {
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



