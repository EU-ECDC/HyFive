import { Component, OnInit } from "@angular/core";
import { Urls } from "../../constants/urls";
import { faCalendar, faSearch } from "@fortawesome/free-solid-svg-icons";
import { SessionTypeMapper } from "../../utils/session-type-mapper";
import { SentSessionsService } from "../../services/data/sent-sessions.service";
import { SessionType } from "../../models/api/SessionType";
import { Observable, Subscription } from "rxjs";
import { SessionReport } from "../../models/api/SessionReport";

@Component({
  selector: "app-sent-sessions",
  templateUrl: "./sent-sessions.component.html",
})
export class SentSessionsComponent {
  Urls = Urls;

  sessions: SessionReport[];
  sessionsFiltered: SessionReport[];
  hasLoadedSessions = false;
  keyword: string = null;
  sessionNameMap: Map<SessionType, string>;
  offlineEvent: Observable<Event>;
  onlineEvent: Observable<Event>;
  subscriptions: Subscription[] = [];
  isOnline: boolean = true;

  faCalendar = faCalendar;
  faSearch = faSearch;

  constructor(private sentSessionsService: SentSessionsService) {
    this.sessionNameMap = SessionTypeMapper.getNameMap();
  }

  loadSessions() {
    this.hasLoadedSessions = false;
    this.sentSessionsService.getSessions().subscribe((x) => {
      this.sessions = x.sort((a, b) => {
        if (a.startDate > b.startDate) {
          return -1;
        }
        if (a.startDate < b.startDate) {
          return 1;
        }
        return 0;
      });
      this.sessionsFiltered = this.sessions;
      this.hasLoadedSessions = true;
    });
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
            .indexOf(this.keyword.toLowerCase()) != -1 ||
          s.facilityName
            ?.toLowerCase()
            .indexOf(this.keyword.toLowerCase()) != -1
      );
    } else {
      this.sessionsFiltered = this.sessions;
    }
  }

  getSessionTypeUrl(sessionType: SessionType): string {
    switch (sessionType) {
      case SessionType.FiveIndications:
        return Urls.SentFiveIndicationsSessionUrl;
      case SessionType.HandJewelry:
        return Urls.SentHandJewelrySessionUrl;
      case SessionType.ProtectiveEquipment:
        return Urls.SendProtectiveEquipmentSessionUrl;
      case SessionType.Gloves:
        return Urls.SentGloveSessionUrl;
      default:
        return "";
    }
  }

  receivedInternetStatus(hasInternet: boolean) {
    this.isOnline = hasInternet;
    if (this.isOnline) {
      this.loadSessions();
    }
  }
}
