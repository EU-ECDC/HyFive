import { Component, OnInit } from "@angular/core";
import { Urls } from "../../konstanter/urls";
import { faCalendar, faSearch } from "@fortawesome/free-solid-svg-icons";
import { SesjonTypeMapper } from "../../utils/type-sesjon-mapper";
import { SendteSesjonerService } from "../../services/data/sendte-sesjoner.service";
import { SesjonType } from "../../models/api/SesjonType";
import { Observable, Subscription } from "rxjs";
import { SesjonRapport } from "../../models/api/SesjonRapport";

@Component({
  selector: "app-sendte-sesjoner",
  templateUrl: "./sendte-sesjoner.component.html",
})
export class SendteSesjonerComponent {
  Urls = Urls;

  sesjoner: SesjonRapport[];
  sesjonerFiltrert: SesjonRapport[];
  harLastetSesjoner = false;
  sokeord: string = null;
  sesjonsnavnMap: Map<SesjonType, string>;
  offlineEvent: Observable<Event>;
  onlineEvent: Observable<Event>;
  subscriptions: Subscription[] = [];
  erOnline: boolean = true;

  faCalendar = faCalendar;
  faSearch = faSearch;

  constructor(private sendteSesjonerService: SendteSesjonerService) {
    this.sesjonsnavnMap = SesjonTypeMapper.getNavnMap();
  }

  lastSesjoner() {
    this.harLastetSesjoner = false;
    this.sendteSesjonerService.getSesjoner().subscribe((x) => {
      this.sesjoner = x.sort((a, b) => {
        if (a.starttidspunkt > b.starttidspunkt) {
          return -1;
        }
        if (a.starttidspunkt < b.starttidspunkt) {
          return 1;
        }
        return 0;
      });
      this.sesjonerFiltrert = this.sesjoner;
      this.harLastetSesjoner = true;
    });
  }

  filtrerSesjoner() {
    if (this.sokeord != null && this.sesjoner != null) {
      this.sesjonerFiltrert = this.sesjoner.filter(
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
      this.sesjonerFiltrert = this.sesjoner;
    }
  }

  getSesjonstypeUrl(sesjonstype: SesjonType): string {
    switch (sesjonstype) {
      case SesjonType.FireIndikasjoner:
        return Urls.SendteFireIndikasjonerSesjonUrl;
      case SesjonType.Handsmykker:
        return Urls.SendteHandsmykkeSesjonUrl;
      case SesjonType.Beskyttelsesutstyr:
        return Urls.SendteBeskyttelsesutstyrSesjonUrl;
      case SesjonType.Hansker:
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
