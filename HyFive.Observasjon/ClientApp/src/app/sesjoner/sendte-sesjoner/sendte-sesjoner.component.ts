import { Component, OnInit } from "@angular/core";
import { Urls } from "../../constants/urls";
import { faCalendar, faSearch } from "@fortawesome/free-solid-svg-icons";
import { SessionTypeMapper } from "../../utils/session-type-mapper";
import { SentSessionsService } from "../../services/data/send-sessions.service";
import { SessionType } from "../../models/api/SessionType";
import { Observable, Subscription } from "rxjs";
import { SessionReport } from "../../models/api/SessionReport";

@Component({
  selector: "app-sendte-sessions",
  templateUrl: "./sendte-sessions.component.html",
})
export class SendteSesjonerComponent {
  Urls = Urls;

  sessions: SessionReport[];
  sesjonerFiltrert: SessionReport[];
  harLastetSesjoner = false;
  sokeord: string = null;
  sesjonsnavnMap: Map<SessionType, string>;
  offlineEvent: Observable<Event>;
  onlineEvent: Observable<Event>;
  subscriptions: Subscription[] = [];
  isOnline: boolean = true;

  faCalendar = faCalendar;
  faSearch = faSearch;

  constructor(private sentSessionsService: SentSessionsService) {
    this.sesjonsnavnMap = SessionTypeMapper.getNameMap();
  }

  lastSesjoner() {
    this.harLastetSesjoner = false;
    this.sentSessionsService.getSessions().subscribe((x) => {
      this.sessions = x.sort((a, b) => {
        if (a.startTime > b.startTime) {
          return -1;
        }
        if (a.startTime < b.startTime) {
          return 1;
        }
        return 0;
      });
      this.sesjonerFiltrert = this.sessions;
      this.harLastetSesjoner = true;
    });
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
            .indexOf(this.sokeord.toLowerCase()) != -1 ||
          s.institutionsName
            ?.toLowerCase()
            .indexOf(this.sokeord.toLowerCase()) != -1
      );
    } else {
      this.sesjonerFiltrert = this.sessions;
    }
  }

  getSesjonstypeUrl(sesjonstype: SessionType): string {
    switch (sesjonstype) {
      case SessionType.FourIndications:
        return Urls.SentFourIndicationsSessionUrl;
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
      this.lastSesjoner();
    }
  }
}
