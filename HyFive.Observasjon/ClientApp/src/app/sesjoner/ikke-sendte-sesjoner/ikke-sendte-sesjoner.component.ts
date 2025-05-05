import { Component, OnInit, OnDestroy } from "@angular/core";
import { FourIndicationsSessionService } from "../../services/data/four-indications-session.service";
import { Urls } from "../../constants/urls";
import { HandsmykkeSesjonService } from "../../services/data/handsmykke-sesjon.service";
import { Session } from "../../models/api/Session";
import { faCalendar } from "@fortawesome/free-solid-svg-icons";
import { SesjonTypeMapper } from "../../utils/type-sesjon-mapper";
import { ProtectiveEquipmentSessionService } from "../../services/data/protectiveEquipment-session.service";
import { SessionType } from "../../models/api/SessionType";
import { SessionReport } from "../../models/api/SessionReport";
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

  sessions: SessionReport[];
  sesjonerFiltrert: SessionReport[];
  sokeord: string = null;
  sesjonsnavnMap: Map<SessionType, string>;
  erOnline: boolean = true;

  harValgtEnSesjon: boolean = false;

  faCalendar = faCalendar;

  constructor(
    private fourIndicationsSessionService: FourIndicationsSessionService,
    private handsmykkeSesjonService: HandsmykkeSesjonService,
    private hanskeSesjonService: HanskeSesjonService,
    private protectiveEquipmentSessionService: ProtectiveEquipmentSessionService,
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
    this.sessions = this.fourIndicationsSessionService
      .hentSesjoner()
      .map((f) => this.createSessionView(f, SessionType.FourIndications))
      .concat(
        this.handsmykkeSesjonService
          .hentSesjoner()
          .map((h) => this.createSessionView(h, SessionType.HandJewelry))
      )
      .concat(
        this.hanskeSesjonService
          .hentSesjoner()
          .map((h) => this.createSessionView(h, SessionType.Gloves))
      )
      .concat(
        this.protectiveEquipmentSessionService
          .hentSesjoner()
          .map((b) => this.createSessionView(b, SessionType.ProtectiveEquipment))
      )
      .sort((a, b) => {
        if (a.startTime > b.startTime) {
          return -1;
        }
        if (a.startTime < b.startTime) {
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
          s.departmentName?.toLowerCase().indexOf(this.sokeord.toLowerCase()) !=
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

  createSessionView(
    sesjon: Session<any>,
    sesjonstype: SessionType
  ): SessionReport {
    return {
      departmentName: sesjon.department?.name,
      startTime: sesjon.startTime,
      type: sesjonstype,
      id: sesjon.id,
      institutionsName: sesjon.institutionsName,
    };
  }

  getSesjonstypeUrl(sesjonstype: SessionType): string {
    switch (sesjonstype) {
      case SessionType.FourIndications:
        return Urls.FourIndicationsSessionUrl;
      case SessionType.HandJewelry:
        return Urls.HandsmykkeSesjonUrl;
      case SessionType.Gloves:
        return Urls.GloveSessionUrl;
      case SessionType.ProtectiveEquipment:
        return Urls.ProtectiveEquipmentSessionUrl;
      default:
        return "";
    }
  }

  sendValgteSesjonerTilServer() {
    const observables = [];
   
    this.sesjonerFiltrert.forEach((s) => {
      if (s.isSelected) {
        let observable;
        switch (s.type) {
          case SessionType.FourIndications:
            observable = this.fourIndicationsSessionService
              .sendToServer(s.id).pipe(
                tap(() => {
                  const index = this.sesjonerFiltrert.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sesjonerFiltrert.splice(index, 1);
                  }
                  this.fourIndicationsSessionService.slettSesjon(s.id);
                }),
                catchError(error => {
                  console.error('Error i sesjon:', error);
                  return of(null);  // Return a null value so forkJoin still completes
                })
              );
            break;
   
          case SessionType.HandJewelry:
            observable = this.handsmykkeSesjonService
              .sendToServer(s.id).pipe(
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
   
          case SessionType.Gloves:
            observable = this.hanskeSesjonService
              .sendToServer(s.id).pipe(
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
            observable = this.protectiveEquipmentSessionService
              .sendToServer(s.id).pipe(
                tap(() => {
                  const index = this.sesjonerFiltrert.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sesjonerFiltrert.splice(index, 1);
                  }
                  this.protectiveEquipmentSessionService.slettSesjon(s.id);
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

    
  merkSesjon(sesjon: SessionReport) {
    sesjon.isSelected = !sesjon.isSelected;
    this.harValgtEnSesjon = this.sesjonerFiltrert.some((s) => s.isSelected);
  }

  merkAlleSesjoner() {
    this.sesjonerFiltrert.forEach((s) => (s.isSelected = true));
    this.harValgtEnSesjon = true;
  }
}
