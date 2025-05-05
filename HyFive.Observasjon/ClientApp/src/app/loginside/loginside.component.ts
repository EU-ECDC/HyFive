import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../services/data/authorization.service';
import { LoggedInUser } from '../models/api/LoggedInUser';
import { Urls } from '../constants/urls';
import {FourIndicationsSessionService} from '../services/data/four-indications-session.service';
import {HanskeSesjonService} from '../services/data/hansker-sesjon.service';
import {ProtectiveEquipmentSessionService} from '../services/data/protectiveEquipment-session.service';
import {HandsmykkeSesjonService} from '../services/data/handsmykke-sesjon.service';
import {ToastrService} from 'ngx-toastr';
import { Institution } from '../models/api/Institution';
import { ForesporselOmBrukertilgangService } from '../services/data/foresporselombrukertilgang.sevice';
import {ClipboardService} from 'ngx-clipboard';
import { KodeverkCacheService } from '../services/data/kodeverk-cache.service';
import { HjelpetekstComponent } from '../shared/hjelpetekst/hjelpetekst.component';
import { Localstoragepaths } from '../constants/localstoragepaths';

@Component({
  selector: 'app-loginside',
  templateUrl: './loginside.component.html'
})
export class LoginsideComponent implements OnInit, OnDestroy {

  isLoggedIn = false;
  erOnline = false;
  user: LoggedInUser;
  Urls = Urls;
  mottattBrukerStatusFraServer = false;
  institusjoner: Institution[];
  valgtInstitusjon: Institution = null;
  visFeilMelding: boolean = false;
  visForesporselErRegistrert = false;
  institusjon: Institution;
  visForesporselVenterPaaGodkjenning = false;
  visForesporselRegistrering = true;
  erVisPseudonym = false;

  constructor(
    private authorizationService: AuthorizationService,
    private fireIndikasjonerService: FourIndicationsSessionService,
    private hanskeService: HanskeSesjonService,
    private beskyttelsesutstyrService: ProtectiveEquipmentSessionService,
    private handsmykkeService: HandsmykkeSesjonService,
    private foresporselOmBrukertilgangService: ForesporselOmBrukertilgangService,
    private toastrService: ToastrService,
    private clipboardService: ClipboardService,
    private kodeverkCacheService: KodeverkCacheService
  ) { }

  ngOnInit(): void {
    this.authorizationService.isLoggedIn().subscribe((isLoggedIn) => {
      this.mottattBrukerStatusFraServer = true;
      this.isLoggedIn = isLoggedIn;
      if (isLoggedIn) {
        this.authorizationService.getUser().subscribe(user => {
          this.user = user;

          this.foresporselOmBrukertilgangService.hentForesporselSomSendtAllerede().subscribe(
            (forsporsel) => {
              if(forsporsel != null)
              {
                this.foresporselOmBrukertilgangService.hentInstitusjon(forsporsel.institutionId).subscribe(
                  (institusjon) => {
                    if(institusjon != null)
                    {
                      this.institusjon = institusjon;
                      this.visForesporselVenterPaaGodkjenning = true;
                      this.visForesporselRegistrering = false;
                    }
                  });
              }
          });

          // denne gjør en initiell last av kodeverk, slik at cachen blir fylt ut og man kan jobbe offline
          this.kodeverkCacheService.lastKodeverk();
        });

        this.foresporselOmBrukertilgangService.hentInstitusjoner().subscribe(
          (institusjoner) => {
            this.institusjoner = institusjoner;
          }
        );
      }
    });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loggUt() {
    this.unregisterSw().then(() => {
      window.location.href = "/account/logout";
    });
  }

  unregisterSw() : Promise<any> {
    return navigator.serviceWorker.getRegistrations().then(function(registrations) {
      for(let registration of registrations) {
        registration.unregister()
      }}).catch(function(err) {
    });

  }

  harLokaleSesjonerLiggende(): boolean {
    var harHanskesesjoner = this.hanskeService.numberOfSessions() > 0;
    var harbeskyttelsesutstyrsesjoner = this.beskyttelsesutstyrService.numberOfSessions() > 0;
    var harFireindikasjonersesjoner = this.fireIndikasjonerService.numberOfSessions() > 0;
    var harHandsmykkesesjoner = this.handsmykkeService.numberOfSessions() > 0;
    return harHanskesesjoner || harbeskyttelsesutstyrsesjoner || harFireindikasjonersesjoner || harHandsmykkesesjoner;
  }

  mottattInternettStatus(erOnline: boolean) {
    this.erOnline = erOnline;
  }

  sendForesporsel() {
    if(this.valgtInstitusjon)
    {
      var nyForsporselOmBrukertilgang = {
        institutionId: this.valgtInstitusjon?.id,
        userFirstName: this.user.firstName,
        userLastName: this.user.lastName,
        hprNumber: this.user.hprNumber,
        identityPseudonym: this.user.identityPseudonym
      }
      this.foresporselOmBrukertilgangService.sendForesporselOmBrukertilgang(nyForsporselOmBrukertilgang).subscribe(
        (erBrukerOpprettet) => {
          if (erBrukerOpprettet)
          {
            this.visForesporselErRegistrert = true;
            this.visForesporselRegistrering = false;
          }
          else
            this.visFeilMelding = true;
        },
        (error) =>{
          this.visFeilMelding = true;
        }
      );
    }
  }

  kopierPseudonymKlikk() {
    this.clipboardService.copy(this.user?.identityPseudonym);
    this.toastrService.success('Pseudonym kopiert til utklippstavle og kan limes inn andre steder ved bruk av Lim inn (CTRL+V)');
  }

  visPseudonym() : void {
    this.erVisPseudonym = true;
  }

  lukkInfoModal($event: boolean) {
    this.erVisPseudonym = $event;
  }
}
