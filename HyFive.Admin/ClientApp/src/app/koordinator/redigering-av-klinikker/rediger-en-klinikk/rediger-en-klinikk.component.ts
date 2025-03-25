import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Klinikk } from '../../../models/api/Klinikk';
import { InstitusjonService } from '../../../services/data/institusjon.service';
import { AvdelingService } from '../../../services/data/avdeling.service';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { Avdelingsvalg } from '../../../models/kodeverk/avdelingsvalg.model';
import { KlinikkService } from '../../../services/data/klinikk.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-rediger-en-klinikk',
  templateUrl: './rediger-en-klinikk.component.html'
})
export class RedigerEnKlinikkComponent implements OnInit, OnDestroy {

  @Input() klinikk: Klinikk;
  klinikkKopi: Klinikk;
  avdelingsvalg: Avdelingsvalg[];
  UrlPaths = UrlPaths;

  klinikkerListe: Klinikk[] = [];

  fawarningicon = faExclamationTriangle;

  constructor(
    private institusjonService: InstitusjonService,
    private avdelingService: AvdelingService,
    private toastrService: ToastrService,
    private klinikkService: KlinikkService) { }

  ngOnInit(): void {
    if (this.klinikk) {
      this.klinikkKopi = JSON.parse(JSON.stringify(this.klinikk));
      this.lastAvdelinger();
    }
    else {
      this.toastrService.error('Avdeling ikke lastet', 'Teknisk feil', { disableTimeOut: true});
    }
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastAvdelinger() {

    this.klinikkService.hentKlinikkerForInstitusjon(this.klinikkKopi.institusjonId).subscribe((institusjon) => {
      this.klinikkerListe = institusjon;

      this.institusjonService.hentAvdelinger(this.klinikkKopi.institusjonId).subscribe(
        (avdelinger) => {
          this.avdelingsvalg = avdelinger.map(a => (
            {
              avdeling: a, erValgt: this.klinikkKopi.avdelinger.map(k => k.id).indexOf(a.id) !== -1,
              erAlleredePaKlinikk: this.klinikkerListe.some(k => k.avdelinger.some(av => av.id === a.id) && k.id !== this.klinikkKopi.id)
            }));
        },
        (err) => this.toastrService.error(`Kunne ikke laste inn klinikker: ${err?.message ? err.message : err}`, 'Teknisk feil', { disableTimeOut: true})
      );

     });

    
  }

  kanIkkeLagreKlinikk(): boolean {
    return this.kanLagreKlinikk() === false;
  }

  kanLagreKlinikk(): boolean {
    return this.klinikkKopi.institusjonId > 0
      && this.klinikkKopi.navn?.length > 0
      && this.avdelingsvalg?.filter(r => r.erValgt)?.length > 0;
  }

  lagreKlinikk() {
    this.klinikkKopi.avdelinger = this.avdelingsvalg.filter(m => m.erValgt).map(r => r.avdeling);
    this.klinikkService.oppdaterKlinikk(this.klinikkKopi).subscribe(
      (k) => {
        // Må replace verdier på original-objektet for å støtte oppdatering av liste når en navigerer tilbake til klinikk-oversikt
        this.klinikk.navn = k.navn;
        this.klinikk.institusjonId = k.institusjonId;
        this.klinikk.avdelinger = k.avdelinger;
        this.toastrService.success('Klinikk oppdatert');
      },
      (err) => this.toastrService.error(`Teknisk feil ved oppdatering: ${err?.message ? err.message : err}`, '', { disableTimeOut: true})
    );
  }
}
