import { Component, OnInit, OnDestroy } from "@angular/core";
import { FourIndicationsSessionService } from "../../services/data/four-indications-session.service";
import { Urls } from "../../constants/urls";
import { HandJewelrySessionService } from "../../services/data/hand-Jewelry-session.service";
import { Session } from "../../models/api/Session";
import { faCalendar } from "@fortawesome/free-solid-svg-icons";
import { SessionTypeMapper } from "../../utils/session-type-mapper";
import { ProtectiveEquipmentSessionService } from "../../services/data/protectiveEquipment-session.service";
import { SessionType } from "../../models/api/SessionType";
import { SessionReport } from "../../models/api/SessionReport";
import { ToastrService } from "ngx-toastr";
import { GloveSessionService } from "../../services/data/glove-session.service";
import { forkJoin, of } from "rxjs";
import { catchError, tap } from "rxjs/operators";

@Component({
  selector: "app-not-sent-sessions",
  templateUrl: "./not-sent-sessions.component.html",
})
export class NotSentSessionsComponent implements OnInit, OnDestroy {
  Urls = Urls;

  sessions: SessionReport[];
  sessionsFiltered: SessionReport[];
  keyword: string = null;
  sessionNameMap: Map<SessionType, string>;
  isOnline: boolean = true;

  hasSelectedASession: boolean = false;

  faCalendar = faCalendar;

  constructor(
    private fourIndicationsSessionService: FourIndicationsSessionService,
    private handJewelrySessionService: HandJewelrySessionService,
    private gloveSessionService: GloveSessionService,
    private protectiveEquipmentSessionService: ProtectiveEquipmentSessionService,
    private toastrService: ToastrService
  ) {
    this.sessionNameMap = SessionTypeMapper.getNameMap();
  }

  ngOnInit(): void {
    this.loadSessions();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadSessions() {
    this.sessions = this.fourIndicationsSessionService
      .getSessions()
      .map((f) => this.createSessionView(f, SessionType.FiveIndications))
      .concat(
        this.handJewelrySessionService
          .getSessions()
          .map((h) => this.createSessionView(h, SessionType.HandJewelry))
      )
      .concat(
        this.gloveSessionService
          .getSessions()
          .map((h) => this.createSessionView(h, SessionType.Gloves))
      )
      .concat(
        this.protectiveEquipmentSessionService
          .getSessions()
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
    this.sessionsFiltered = this.sessions;
  }

  filterSessions() {
    if (this.keyword != null && this.sessions != null) {
      this.sessionsFiltered = this.sessions.filter(
        (s) =>
          s.departmentName?.toLowerCase().indexOf(this.keyword.toLowerCase()) !=
          -1 ||
          this.sessionNameMap
            .get(s.type)
            ?.toLowerCase()
            .indexOf(this.keyword.toLowerCase()) != -1
      );
    } else {
      this.sessionsFiltered = this.sessions;
    }
  }

  createSessionView(
    session: Session<any>,
    sessionType: SessionType
  ): SessionReport {
    return {
      departmentName: session.department?.name,
      startTime: session.startTime,
      type: sessionType,
      id: session.id,
      institutionsName: session.institutionsName,
    };
  }

  getSessionTypeUrl(sessionType: SessionType): string {
    switch (sessionType) {
      case SessionType.FiveIndications:
        return Urls.FourIndicationsSessionUrl;
      case SessionType.HandJewelry:
        return Urls.HandJewelrySessionUrl;
      case SessionType.Gloves:
        return Urls.GloveSessionUrl;
      case SessionType.ProtectiveEquipment:
        return Urls.ProtectiveEquipmentSessionUrl;
      default:
        return "";
    }
  }

  sendSelectedSessionsToServer() {
    const observables = [];
   
    this.sessionsFiltered.forEach((s) => {
      if (s.isSelected) {
        let observable;
        switch (s.type) {
          case SessionType.FiveIndications:
            observable = this.fourIndicationsSessionService
              .sendToServer(s.id).pipe(
                tap(() => {
                  const index = this.sessionsFiltered.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sessionsFiltered.splice(index, 1);
                  }
                  this.fourIndicationsSessionService.deleteSession(s.id);
                }),
                catchError(error => {
                  console.error('Error in session:', error);
                  return of(null);  // Return a null value so forkJoin still completes
                })
              );
            break;
   
          case SessionType.HandJewelry:
            observable = this.handJewelrySessionService
              .sendToServer(s.id).pipe(
                tap(() => {
                  const index = this.sessionsFiltered.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sessionsFiltered.splice(index, 1);
                  }
                  this.handJewelrySessionService.deleteSession(s.id);
                }),
                catchError(error => {
                  console.error('Error in session:', error);
                  return of(null);
                })
              );
            break;
   
          case SessionType.Gloves:
            observable = this.gloveSessionService
              .sendToServer(s.id).pipe(
                tap(() => {
                  const index = this.sessionsFiltered.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sessionsFiltered.splice(index, 1);
                  }
                  this.gloveSessionService.deleteSession(s.id);
                }),
                catchError(error => {
                  console.error('Error in session:', error);
                  return of(null);
                })
              );
            break;
   
          case SessionType.ProtectiveEquipment:
            observable = this.protectiveEquipmentSessionService
              .sendToServer(s.id).pipe(
                tap(() => {
                  const index = this.sessionsFiltered.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sessionsFiltered.splice(index, 1);
                  }
                  this.protectiveEquipmentSessionService.deleteSession(s.id);
                }),
                catchError(error => {
                  console.error('Error in session:', error);
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
        this.toastrService.success("The sessions were sent to the server");
      },
      error: (err) => {
        this.toastrService.error("Error sending sessions to server");
      }
    });
  }

    
  markSession(session: SessionReport) {
    session.isSelected = !session.isSelected;
    this.hasSelectedASession = this.sessionsFiltered.some((s) => s.isSelected);
  }

  markAllSessions() {
    this.sessionsFiltered.forEach((s) => (s.isSelected = true));
    this.hasSelectedASession = true;
  }
}
