import { Component, OnInit, OnDestroy } from '@angular/core';
import { Rolle } from 'src/app/models/api/Rolle';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { Urls } from '../../konstanter/urls';
import { faEnvelope, faPlus, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faClipboard } from '@fortawesome/free-regular-svg-icons';
import { Kort } from '../../models/registrering/kort.model';
import { Uuid } from '../../utils/uuid';
import { HanskeSesjonsvisning } from '../../models/registrering/hansker-sesjonsvisning.model';
import { HanskeSesjon } from '../../models/api/HanskeSesjon';
import { HanskeSesjonService } from '../../services/data/hansker-sesjon.service';
import { HanskeObservasjon } from '../../models/api/HanskeObservasjon';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registrere-hanske',
  templateUrl: './registrere-hanske.component.html'
})

export class RegistrereHanskeComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sesjonsvisning: HanskeSesjonsvisning;
  sesjonsdata: HanskeSesjon = null;
  roller: Rolle[];
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
      .subscribe(i => this.roller = i.avdelinger.find(a => a.id === this.sesjonsvisning.avdeling?.id)?.roller);
  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sesjonId = params[Queryparameters.SesjonId] || 0;
        this.sesjonsvisning = this.sesjonService.hentSesjonsvisningForSesjon(sesjonId);
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

  async registrerObservasjon(observasjon: HanskeObservasjon) {
    await this.sesjonService.registrerObservasjon(observasjon);
    this.toastrService.success("Observasjonen ble lagret");
    this.lastSesjonsdata();
  }

  lastSesjonsdata() {
    this.sesjonsdata = this.sesjonService.hentSesjon(this.sesjonsvisning.sesjonId);
  }

  toggleRolleliste() {
    this.visRolleliste = !this.visRolleliste;
  }

  leggTilNyttKort(rolle: Rolle) {
    this.sesjonsvisning.kort = this.sesjonsvisning.kort.map((k) => { k.erAktivt = false; return k })
    this.sesjonsvisning.kort.push({ id: Uuid.generateUUID(), rolle: rolle, erAktivt: true });
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



