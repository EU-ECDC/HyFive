import { Component, OnInit, OnDestroy } from "@angular/core";
import { FiveIndicationsSessionService } from "../../services/data/five-indications-session.service";
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
import { tap } from "rxjs/operators";
import { PageEvent } from "@angular/material/paginator";

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
  totalItems = 0; // total number of items, e.g. from API
  currentPage = 0;
  offset = 0;
  pageSize = 25;
  pageSizeOptions = [25,30];

  constructor(
    private fiveIndicationsSessionService: FiveIndicationsSessionService,
    private handJewelrySessionService: HandJewelrySessionService,
    private gloveSessionService: GloveSessionService,
    private protectiveEquipmentSessionService: ProtectiveEquipmentSessionService,
    private toastrService: ToastrService
  ) {
    this.sessionNameMap = SessionTypeMapper.getNameMap();
  }

  ngOnInit(): void {
    this.loadSessions(this.offset, this.pageSize);
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadSessions(offset, limit) {
    this.sessions = this.fiveIndicationsSessionService
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
        if (a.startDate > b.startDate) {
          return -1;
        }
        if (a.startDate < b.startDate) {
          return 1;
        }
        return 0;
      });
    this.totalItems = this.sessions?.length;
    if (this.totalItems && this.totalItems > this.pageSizeOptions.slice(-1)[0]) {
      this.pageSizeOptions.push(this.totalItems);
    }
    this.sessions = this.sessions.slice(offset, offset + limit);
    this.sessionsFiltered = this.sessions;
  }

  filterSessions() {
    if (this.keyword != null && this.sessions != null) {
      this.sessionsFiltered = this.sessions.filter(
        (s) =>
          s.departmentName?.toLowerCase().indexOf(this.keyword.toLowerCase()) !=
          -1 ||
          s.facilityName?.toLowerCase().indexOf(this.keyword.toLowerCase()) !=
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

  onPageChange(event: PageEvent) {
    this.currentPage = event.pageIndex;
    this.pageSize = event.pageSize;
    
    this.offset = this.currentPage * this.pageSize;
    this.loadSessions(this.offset, this.pageSize);
  }

  createSessionView(
    session: Session<any>,
    sessionType: SessionType
  ): SessionReport {
    return {
      departmentName: session.department?.name,
      startDate: session.createdDate,
      type: sessionType,
      id: session.id,
      facilityName: session.facilityName,
    };
  }

  getSessionTypeUrl(sessionType: SessionType): string {
    switch (sessionType) {
      case SessionType.FiveIndications:
        return Urls.FiveIndicationsSessionUrl;
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
            observable = this.fiveIndicationsSessionService
              .sendToServer(s.id).pipe(
                tap(() => {
                  const index = this.sessionsFiltered.findIndex((sf) => sf.id === s.id);
                  if (index > -1) {
                    this.sessionsFiltered.splice(index, 1);
                  }
                  this.fiveIndicationsSessionService.deleteSession(s.id);
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
