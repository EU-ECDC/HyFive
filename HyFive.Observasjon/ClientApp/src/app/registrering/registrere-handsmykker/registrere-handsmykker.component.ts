import { Component, OnInit, OnDestroy } from '@angular/core';
import { HandJewelrySession } from 'src/app/models/api/HandJewelrySession';
import { Role } from 'src/app/models/api/Role';
import { HandsmykkeSesjonService } from 'src/app/services/data/handsmykke-sesjon.service';
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
  visRolleliste: boolean = false;
  visTomForKortTekst: boolean = false;

  faPlus = faPlus;
  faClipboard = faClipboard;
  faEnvelope = faEnvelope;
  faCircle = faCircle;
  faArrowDown = faArrowDown;

  constructor(
    private sesjonService: HandsmykkeSesjonService,
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

    if (this.sessionView.card?.length === 0)
      this.visTomForKortTekst = true;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  async registrerObservasjon(observasjon: HandJewelryObservation) {
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

  oppdaterSesjonsvisning(sessionView: HandJewelrySessionView) {
    this.sessionView = this.sesjonService.oppdaterSesjonsvisningForSesjon(sessionView);
    if (this.sessionView.card?.length === 0)
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



