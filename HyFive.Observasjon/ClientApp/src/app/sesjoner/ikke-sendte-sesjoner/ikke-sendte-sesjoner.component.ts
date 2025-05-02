import { Component, OnInit, OnDestroy } from "@angular/core";
import { FireIndikasjonerSesjonService } from "../../services/data/fire-indikasjoner-sesjon.service";
import { Urls } from "../../konstanter/urls";
import { HandsmykkeSesjonService } from "../../services/data/handsmykke-sesjon.service";
import { Sesjon } from "../../models/api/Sesjon";
import { faCalendar } from "@fortawesome/free-solid-svg-icons";
import { SesjonTypeMapper } from "../../utils/type-sesjon-mapper";
import { BeskyttelsesutstyrSesjonService } from "../../services/data/beskyttelsesutstyr-sesjon.service";
import { SessionType } from "../../models/api/SessionType";
import { SesjonRapport } from "../../models/api/SesjonRapport";
import { ToastrService } from "ngx-toastr";
import { HanskeSesjonService } from "../../services/data/hansker-sesjon.service";
import { forkJoin, of } from "rxjs";
import { catchError, tap } from "rxjs/operators";

@Component({
  selector: "app-ikke-sendte-sessions",
  templateUrl: "./ikke-sendte-sessions.component.html",
})
export class IkkeSendteSesjonerComponent implements OnInit, OnDestroy {
  Urls = Urls;

  sessions: SesjonRapport[];
  sesjonerFiltrert: SesjonRapport[];
  sokeord: string = null;
  sesjonsnavnMap: Map<SessionType, string>;
  erOnline: boolean = true;

  harValgtEnSesjon: boolean = false;

  faCalendar = faCalendar;

  constructor(
    private fireIndikasjonerSesjonService: FireIndikasjonerSesjonService,
    private handsmykkeSesjonService: HandsmykkeSesjonService,
    private hanskeSesjonService: HanskeSesjonService,
    private beskyttelsesutstyrSesjonService: BeskyttelsesutstyrSesjonService,
    private toastrService: ToastrService
  ) {
    this.sesjonsnavnMap = SesjonTypeMapper.getNameMap();
  }

  ngOnInit(): void {
    this.lastSesjoner();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastSesjoner() {
    this.sessions = this.fireIndikasjonerSesjonService
      .hentSesjoner()
      .map((f) => this.lagSesjonsvisning(f, SessionType.FireIndikasjoner))
      .concat(
        this.handsmykkeSesjonService
          .hentSesjoner()
          .map((h) => this.lagSesjonsvisning(h, SessionType.Handsmykker))
      )
      .concat(
        this.hanskeSesjonService
          .hentSesjoner()
          .map((h) => this.lagSesjonsvisning(h, SessionType.Hansker))
      )
      .concat(
        this.beskyttelsesutstyrSesjonService
          .hentSesjoner()
          .map((b) => this.lagSesjonsvisning(b, SessionType.ProtectiveEquipment))
      )
      .sort((a, b) => {
        if (a.starttidspunkt > b.starttidspunkt) {
          return -1;
        }
        if (a.starttidspunkt < b.starttidspunkt) {
          return 1;
        }
        return 0;
      });
    this.sesjonerFiltrert = this.sessions;
  }

  filtrerSesjoner() {
    if (this.sokeord != null && this.sessions != null) {
      this.sesjonerFiltrert = this.sessions.filter(
        (s) =>
          s.avdelingsnavn?.toLowerCase().indexOf(this.sokeord.toLowerCase()) !=
          -1 ||
          this.sesjonsnavnMap
            .get(s.type)
            ?.toLowerCase()
            .indexOf(this.sokeord.toLowerCase()) != -1
      );
    } else {
      this.sesjonerFiltrert = this.sessions;
    }
  }

  lagSesjonsvisning(
    sesjon: Sesjon<any>,
    sesjonstype: SessionType
  ): SesjonRapport {
    return {
      avdelingsnavn: sesjon.department?.name,
      starttidspunkt: sesjon.starttidspunkt,
      type: sesjonstype,
      id: sesjon.id,
      institusjonsnavn: sesjon.institusjonsnavn,
    };
  }

  getSesjonstypeUrl(sesjonstype: SessionType): string {
    switch (sesjonstype) {
      case SessionType.FireIndikasjoner:
        return Urls.FireIndikasjonerSesjonUrl;
      case SessionType.Handsmykker:
        return Urls.HandsmykkeSesjonUrl;
      case SessionType.Hansker:
        return Urls.HanskeSesjonUrl;
      case SessionType.ProtectiveEquipment:
        return Urls.BeskyttelsesutstyrSesjonUrl;
      default:
        return "";
    }
  }

  sendValgteSesjonerTilServer() {
    const observables = [];
   
    this.sesjonerFiltrert.forEach((s) => {
      if (s.erValgt) {
        let observable;
        switch (s.type) {
          case SessionType.FireIndikasjoner:
            observable = this.fireIndikasjonerSesjonService
              .sendTilServer(s.id).pipe(
                tap(() => {
                  const index = this.sesjonerFiltrert.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sesjonerFiltrert.splice(index, 1);
                  }
                  this.fireIndikasjonerSesjonService.slettSesjon(s.id);
                }),
                catchError(error => {
                  console.error('Error i sesjon:', error);
                  return of(null);  // Return a null value so forkJoin still completes
                })
              );
            break;
   
          case SessionType.Handsmykker:
            observable = this.handsmykkeSesjonService
              .sendTilServer(s.id).pipe(
                tap(() => {
                  const index = this.sesjonerFiltrert.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sesjonerFiltrert.splice(index, 1);
                  }
                  this.handsmykkeSesjonService.slettSesjon(s.id);
                }),
                catchError(error => {
                  console.error('Error i sesjon:', error);
                  return of(null);
                })
              );
            break;
   
          case SessionType.Hansker:
            observable = this.hanskeSesjonService
              .sendTilServer(s.id).pipe(
                tap(() => {
                  const index = this.sesjonerFiltrert.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sesjonerFiltrert.splice(index, 1);
                  }
                  this.hanskeSesjonService.slettSesjon(s.id);
                }),
                catchError(error => {
                  console.error('Error i sesjon:', error);
                  return of(null);
                })
              );
            break;
   
          case SessionType.ProtectiveEquipment:
            observable = this.beskyttelsesutstyrSesjonService
              .sendTilServer(s.id).pipe(
                tap(() => {
                  const index = this.sesjonerFiltrert.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sesjonerFiltrert.splice(index, 1);
                  }
                  this.beskyttelsesutstyrSesjonService.slettSesjon(s.id);
                }),
                catchError(error => {
                  console.error('Error i sesjon:', error);
                  return of(null);
                })
              );
            break;
        }
   
        if (observable) {
          observables.push(observable);
        }
      }
    });
   
    forkJoin(observables).subscribe({
      next: () => {
        this.toastrService.success("Sesjonene ble sendt til server");
      },
      error: (err) => {
        this.toastrService.error("Feil ved sending av sessions til server");
      }
    });
  }

    
  merkSesjon(sesjon: SesjonRapport) {
    sesjon.erValgt = !sesjon.erValgt;
    this.harValgtEnSesjon = this.sesjonerFiltrert.some((s) => s.erValgt);
  }

  merkAlleSesjoner() {
    this.sesjonerFiltrert.forEach((s) => (s.erValgt = true));
    this.harValgtEnSesjon = true;
  }
}
