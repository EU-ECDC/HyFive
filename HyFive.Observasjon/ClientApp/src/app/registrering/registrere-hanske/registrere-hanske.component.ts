import { Component, OnInit, OnDestroy } from '@angular/core';
import { Role } from 'src/app/models/api/Role';
import { InstitutionService } from '../../services/data/InstitutionService';
import { Router, ActivatedRoute } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { Urls } from '../../constants/urls';
import { faEnvelope, faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faClipboard } from '@fortawesome/free-regular-svg-icons';
import { Card } from '../../models/registration/card.model';
import { Uuid } from '../../utils/uuid';
import { GloveSessionView } from '../../models/registration/hansker-sessionView.model';
import { GloveSession } from '../../models/api/GloveSession';
import { GloveSessionService } from '../../services/data/glove-session.service';
import { GloveObservation } from '../../models/api/GloveObservation';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registrere-hanske',
  templateUrl: './registrere-hanske.component.html'
})

export class RegistrereHanskeComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sessionView: GloveSessionView;
  sessionsdata: GloveSession = null;
  roles: Role[];
  visRolleliste: boolean = false;
  visTomForKortTekst: boolean = false;

  faPlus = faPlus;
  faClipboard = faClipboard;
  faEnvelope = faEnvelope;
  faCircle = faCircle;

  constructor(
    private sesjonService: GloveSessionService,
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
        this.sessionView = this.sesjonService.hentSesjonsvisningForSesjon(sessionId);
        if (!this.sessionView) {
          this.router.navigate(['']);
        }
        else {
          this.lastSesjonsdata();
        }
      });

      if(this.sessionView.card?.length === 0)
        this.visTomForKortTekst = true;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  async registrerObservasjon(observasjon: GloveObservation) {
    await this.sesjonService.registrerObservasjon(observasjon);
    this.toastrService.success("Observasjonen ble lagret");
    this.lastSesjonsdata();
  }

  lastSesjonsdata() {
    this.sessionsdata = this.sesjonService.hentSesjon(this.sessionView.sessionId);
  }

  toggleRolleliste() {
    this.visRolleliste = !this.visRolleliste;
  }

  leggTilNyttKort(role: Role) {
    this.sessionView.card = this.sessionView.card.map((k) => { k.isActive = false; return k })
    this.sessionView.card.push({ id: Uuid.generateUUID(), role: role, isActive: true });
    this.oppdaterSesjonsvisning(this.sessionView);
    this.toggleRolleliste();
  }

  oppdaterSesjonsvisning(sessionView: GloveSessionView) {
    this.sessionView = this.sesjonService.oppdaterSesjonsvisningForSesjon(sessionView);
    if(this.sessionView.card?.length === 0)
      this.visTomForKortTekst = true;
    else 
      this.visTomForKortTekst = false;
  }

  kortErValgt(valgtKort: Card) {
    for (let i = 0; i < this.sessionView.card.length; i++) {
      if (this.sessionView.card[i] != valgtKort) {
        this.sessionView.card[i].isActive = false;
      }
    }
    this.sesjonService.oppdaterSesjonsvisningForSesjon(this.sessionView);
  }

  onCloseNyttKortModal(result) {
    if (result) {
      result.forEach(x => this.leggTilNyttKort(x));
    }
  }

  onDismissNyttKortModal(reason) {
  }
}



