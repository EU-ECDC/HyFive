import { Component, OnInit, OnDestroy } from '@angular/core';
import { Rolle } from 'src/app/models/api/Rolle';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { Urls } from '../../konstanter/urls';
import { faEnvelope, faPlus } from '@fortawesome/free-solid-svg-icons';
import { faClipboard, faCircle } from '@fortawesome/free-regular-svg-icons';
import { Kort } from '../../models/registrering/kort.model';
import { Uuid } from '../../utils/uuid';
import { BeskyttelsesutstyrSesjonsvisning } from '../../models/registrering/beskyttelsesutstyr-sesjonsvisning.model';
import { BeskyttelsesutstyrSesjon } from '../../models/api/BeskyttelsesutstyrSesjon';
import { BeskyttelsesutstyrObservasjon } from '../../models/api/BeskyttelsesutstyrObservasjon';
import { BeskyttelsesutstyrSesjonService } from '../../services/data/beskyttelsesutstyr-sesjon.service';
import { MainMenuEventService } from '../../services/events/main-menu-event.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-registrere-beskyttelsesutstyr',
  templateUrl: './registrere-beskyttelsesutstyr.component.html'
})

export class RegistrereBeskyttelsesutstyrComponent implements OnInit, OnDestroy {

  Urls = Urls;
  sesjonsvisning: BeskyttelsesutstyrSesjonsvisning;
  sesjonsdata: BeskyttelsesutstyrSesjon = null;
  mainMenuIsOpen: boolean;
  visTomForKortTekst: boolean = false;

  roller: Rolle[];
  visRolleliste: boolean = false;
  endreSettingModus: boolean = false;

  faPlus = faPlus;
  faClipboard = faClipboard;
  faEnvelope = faEnvelope;
  faCircle = faCircle;

  constructor(
    private sesjonService: BeskyttelsesutstyrSesjonService,
    private router: Router,
    private route: ActivatedRoute,
    private institusjonService: InstitusjonService,
    private mainMenuService: MainMenuEventService,
    private toastrService: ToastrService
  ) {
    this.institusjonService.getValgtInstitusjon()
      .subscribe(i => this.roller = i.avdelinger.find(a => a.id === this.sesjonsvisning.avdeling?.id)?.roller);
    //this.mainMenuService.mainMenuIsOpenEvent.subscribe(m => this.mainMenuIsOpen = m);
  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sesjonId = params[Queryparameters.SesjonId] || 0;
        this.sesjonsvisning = this.sesjonService.hentSesjonsvisningForSesjon(sesjonId);
        if (!this.sesjonsvisning) this.router.navigate(['']);
        else this.lastSesjonsdata();
      });
      if(this.sesjonsvisning.kort?.length === 0)
        this.visTomForKortTekst = true;
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  async registrerObservasjon(observasjon: BeskyttelsesutstyrObservasjon) {
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
    this.sesjonsvisning.kort.push({ id: Uuid.generateUUID(), rolle: rolle, erAktivt: true, utstyr: this.sesjonsvisning.setting.utstyrstyper });
    this.oppdaterSesjonsvisning(this.sesjonsvisning);
    this.toggleRolleliste();
  }

  oppdaterSesjonsvisning(sesjonsvisning: BeskyttelsesutstyrSesjonsvisning) {
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

  settingOgUtstyrBleEndret(sesjonsvisning: BeskyttelsesutstyrSesjonsvisning) {
    this.oppdaterSesjonsvisning(sesjonsvisning);
    this.endreSettingModus = false;
  }

  onCloseNyttKortModal(result) {
    if (result) {
      result.forEach(x => this.leggTilNyttKort(x));
    }
  }

  onDismissNyttKortModal(reason) {
  }
}
