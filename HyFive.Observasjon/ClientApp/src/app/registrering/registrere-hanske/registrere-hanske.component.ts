import { Component, OnInit, OnDestroy } from '@angular/core';
import { Role } from 'src/app/models/api/Role';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { Urls } from '../../konstanter/urls';
import { faEnvelope, faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faClipboard } from '@fortawesome/free-regular-svg-icons';
import { Kort } from '../../models/registrering/kort.model';
import { Uuid } from '../../utils/uuid';
import { HanskeSesjonsvisning } from '../../models/registrering/hansker-sesjonsvisning.model';
import { GloveSession } from '../../models/api/GloveSession';
import { HanskeSesjonService } from '../../services/data/hansker-sesjon.service';
import { GloveObservation } from '../../models/api/GloveObservation';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registrere-hanske',
  templateUrl: './registrere-hanske.component.html'
})

export class RegistrereHanskeComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sesjonsvisning: HanskeSesjonsvisning;
  sessionsdata: GloveSession = null;
  roles: Role[];
  visRolleliste: boolean = false;
  visTomForKortTekst: boolean = false;

  faPlus = faPlus;
  faClipboard = faClipboard;
  faEnvelope = faEnvelope;
  faCircle = faCircle;

  constructor(
    private sesjonService: HanskeSesjonService,
    private router: Router,
    private route: ActivatedRoute,
    private institusjonService: InstitusjonService,
    private toastrService: ToastrService) {
    this.institusjonService.getValgtInstitusjon()
      .subscribe(i => this.roles = i.departments.find(a => a.id === this.sesjonsvisning.department?.id)?.roles);
  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SesjonId] || 0;
        this.sesjonsvisning = this.sesjonService.hentSesjonsvisningForSesjon(sessionId);
        if (!this.sesjonsvisning) {
          this.router.navigate(['']);
        }
        else {
          this.lastSesjonsdata();
        }
      });

      if(this.sesjonsvisning.kort?.length === 0)
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
    this.sessionsdata = this.sesjonService.hentSesjon(this.sesjonsvisning.sessionId);
  }

  toggleRolleliste() {
    this.visRolleliste = !this.visRolleliste;
  }

  leggTilNyttKort(role: Role) {
    this.sesjonsvisning.kort = this.sesjonsvisning.kort.map((k) => { k.erAktivt = false; return k })
    this.sesjonsvisning.kort.push({ id: Uuid.generateUUID(), role: role, erAktivt: true });
    this.oppdaterSesjonsvisning(this.sesjonsvisning);
    this.toggleRolleliste();
  }

  oppdaterSesjonsvisning(sesjonsvisning: HanskeSesjonsvisning) {
    this.sesjonsvisning = this.sesjonService.oppdaterSesjonsvisningForSesjon(sesjonsvisning);
    if(this.sesjonsvisning.kort?.length === 0)
      this.visTomForKortTekst = true;
    else 
      this.visTomForKortTekst = false;
  }

  kortErValgt(valgtKort: Kort) {
    for (let i = 0; i < this.sesjonsvisning.kort.length; i++) {
      if (this.sesjonsvisning.kort[i] != valgtKort) {
        this.sesjonsvisning.kort[i].erAktivt = false;
      }
    }
    this.sesjonService.oppdaterSesjonsvisningForSesjon(this.sesjonsvisning);
  }

  onCloseNyttKortModal(result) {
    if (result) {
      result.forEach(x => this.leggTilNyttKort(x));
    }
  }

  onDismissNyttKortModal(reason) {
  }
}



