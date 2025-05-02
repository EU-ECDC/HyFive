import { Component, OnInit } from "@angular/core";
import { Urls } from "../../konstanter/urls";
import { faCalendar, faSearch } from "@fortawesome/free-solid-svg-icons";
import { SesjonTypeMapper } from "../../utils/type-sesjon-mapper";
import { SendteSesjonerService } from "../../services/data/sendte-sessions.service";
import { SessionType } from "../../models/api/SessionType";
import { Observable, Subscription } from "rxjs";
import { SesjonRapport } from "../../models/api/SesjonRapport";

@Component({
  selector: "app-sendte-sessions",
  templateUrl: "./sendte-sessions.component.html",
})
export class SendteSesjonerComponent {
  Urls = Urls;

  sessions: SesjonRapport[];
  sesjonerFiltrert: SesjonRapport[];
  harLastetSesjoner = false;
  sokeord: string = null;
  sesjonsnavnMap: Map<SessionType, string>;
  offlineEvent: Observable<Event>;
  onlineEvent: Observable<Event>;
  subscriptions: Subscription[] = [];
  erOnline: boolean = true;

  faCalendar = faCalendar;
  faSearch = faSearch;

  constructor(private sendteSesjonerService: SendteSesjonerService) {
    this.sesjonsnavnMap = SesjonTypeMapper.getNameMap();
  }

  lastSesjoner() {
    this.harLastetSesjoner = false;
    this.sendteSesjonerService.getSesjoner().subscribe((x) => {
      this.sessions = x.sort((a, b) => {
        if (a.starttidspunkt > b.starttidspunkt) {
          return -1;
        }
        if (a.starttidspunkt < b.starttidspunkt) {
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
          s.avdelingsnavn?.toLowerCase().indexOf(this.sokeord.toLowerCase()) !=
            -1 ||
          this.sesjonsnavnMap
            .get(s.type)
            ?.toLowerCase()
            .indexOf(this.sokeord.toLowerCase()) != -1 ||
          s.institusjonsnavn
            ?.toLowerCase()
            .indexOf(this.sokeord.toLowerCase()) != -1
      );
    } else {
      this.sesjonerFiltrert = this.sessions;
    }
  }

  getSesjonstypeUrl(sesjonstype: SessionType): string {
    switch (sesjonstype) {
      case SessionType.FireIndikasjoner:
        return Urls.SendteFireIndikasjonerSesjonUrl;
      case SessionType.Handsmykker:
        return Urls.SendteHandsmykkeSesjonUrl;
      case SessionType.ProtectiveEquipment:
        return Urls.SendteBeskyttelsesutstyrSesjonUrl;
      case SessionType.Hansker:
        return Urls.SendteHanskeSesjonUrl;
      default:
        return "";
    }
  }

  mottattInternettStatus(harInternett: boolean) {
    this.erOnline = harInternett;
    if (this.erOnline) {
      this.lastSesjoner();
    }
  }
}
