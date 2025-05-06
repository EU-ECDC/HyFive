import { Component, OnInit, OnDestroy } from '@angular/core';
import { HandJewelrySession } from 'src/app/models/api/HandJewelrySession';
import { Role } from 'src/app/models/api/Role';
import { HandJewelrySessionService } from 'src/app/services/data/hand-Jewelry-session.service';
import { HandJewelrySessionView } from '../../models/registration/handJewelry-session-view.model';
import { InstitutionService } from '../../services/data/InstitutionService';
import { Router, ActivatedRoute } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { Urls } from '../../constants/urls';
import { faEnvelope, faPlus, faArrowDown, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faClipboard } from '@fortawesome/free-regular-svg-icons';
import { HandJewelryObservation } from '../../models/api/HandJewelryObservation';
import { Card } from '../../models/registration/card.model';
import { Uuid } from '../../utils/uuid';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registrere-handsmykker',
  templateUrl: './registrere-handsmykker.component.html'
})

export class RegistrereHandsmykkerComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sessionView: HandJewelrySessionView;
  sessionsdata: HandJewelrySession = null;
  roles: Role[];
  showRoleList: boolean = false;
  showEmptyForShortText: boolean = false;

  faPlus = faPlus;
  faClipboard = faClipboard;
  faEnvelope = faEnvelope;
  faCircle = faCircle;
  faArrowDown = faArrowDown;

  constructor(
    private sessionService: HandJewelrySessionService,
    private router: Router,
    private route: ActivatedRoute,
    private institutionService: InstitutionService,
    private toastrService: ToastrService) {
    this.institutionService.getSelectedInstitution()
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

    if (this.sessionView.card?.length === 0)
      this.showEmptyForShortText = true;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  async registerObservation(observation: HandJewelryObservation) {
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

  updateSessionView(sessionView: HandJewelrySessionView) {
    this.sessionView = this.sessionService.updateSessionViewForSession(sessionView);
    if (this.sessionView.card?.length === 0)
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



