import { Component, OnInit, OnDestroy } from '@angular/core';
import { FourIndicationsSessionService } from '../../services/data/four-indications-session.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FourIndicationsSessionView } from '../../models/registration/FourIndications-session-view.model';
import { FourIndicationsSession } from '../../models/api/FourIndicationsSession';
import { Queryparameters } from '../../constants/queryparameters';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { InstitutionService } from '../../services/data/InstitutionService';
import { Role } from '../../models/api/Role';
import { Uuid } from '../../utils/uuid';
import { Card } from '../../models/registration/card.model';
import { faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../constants/urls';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register-four-indications',
  templateUrl: './register-four-indications.component.html',
})
export class RegisterFourIndicationsComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sessionView: FourIndicationsSessionView;
  sessionsdata: FourIndicationsSession = null;
  roles: Role[];
  showRoleList: boolean = false;
  showEmptyForShortText: boolean = false;

  faPlus = faPlus;
  faCircle = faCircle;

  constructor(
    private sessionService: FourIndicationsSessionService,
    private router: Router,
    private route: ActivatedRoute,
    private institutionService: InstitutionService,
    private toastrService: ToastrService) {
    this.institutionService
      .getSelectedInstitution()
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

  async registerObservation(observation: FourIndicationsObservation) {
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

  updateSessionView(sessionView: FourIndicationsSessionView) {
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
