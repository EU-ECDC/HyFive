import { Component, OnInit, OnDestroy } from '@angular/core';
import { FireIndikasjonerSesjonService } from '../../services/data/fire-indikasjoner-sesjon.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FireIndikasjonerSesjonsvisning } from '../../models/registrering/fire-indikasjoner-sesjonsvisning.model';
import { FourIndicationsSession } from '../../models/api/FourIndicationsSession';
import { Queryparameters } from '../../konstanter/queryparameters';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { Role } from '../../models/api/Role';
import { Uuid } from '../../utils/uuid';
import { Kort } from '../../models/registrering/kort.model';
import { faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../konstanter/urls';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registrere-fire-indikasjoner-component',
  templateUrl: './registrere-fire-indikasjoner.component.html',
})
export class RegistrereFireIndikasjonerComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sesjonsvisning: FireIndikasjonerSesjonsvisning;
  sessionsdata: FourIndicationsSession = null;
  roles: Role[];
  visRolleliste: boolean = false;
  visTomForKortTekst: boolean = false;

  faPlus = faPlus;
  faCircle = faCircle;

  constructor(
    private sesjonService: FireIndikasjonerSesjonService,
    private router: Router,
    private route: ActivatedRoute,
    private institusjonService: InstitusjonService,
    private toastrService: ToastrService) {
    this.institusjonService
      .getValgtInstitusjon()
      .subscribe(i => this.roles = i.departments.find(a => a.id === this.sesjonsvisning.department?.id)?.roles);
  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SesjonId] || 0;
        this.sesjonsvisning = this.sesjonService.hentSesjonsvisningForSesjon(sessionId);
        if (!this.sesjonsvisning) this.router.navigate(['']);
        else this.lastSesjonsdata();
      });

    if(this.sesjonsvisning.kort?.length === 0)
      this.visTomForKortTekst = true;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  async registrerObservasjon(observasjon: FourIndicationsObservation) {
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

  oppdaterSesjonsvisning(sesjonsvisning: FireIndikasjonerSesjonsvisning) {
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
