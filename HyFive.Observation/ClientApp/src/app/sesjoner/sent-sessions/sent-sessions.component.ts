import { Component, OnInit } from "@angular/core";
import { Urls } from "../../constants/urls";
import { faCalendar, faSearch } from "@fortawesome/free-solid-svg-icons";
import { SessionTypeMapper } from "../../utils/session-type-mapper";
import { SentSessionsService } from "../../services/data/sent-sessions.service";
import { SessionType } from "../../models/api/SessionType";
import { Observable, Subscription } from "rxjs";
import { SessionReport } from "../../models/api/SessionReport";
import { PageEvent } from '@angular/material/paginator';
import { PaginationRequest } from "src/app/models/api/PaginationRequest";
import { SessionsPaginatedResponse } from "src/app/models/api/SessionsPaginatedResponse";

@Component({
  selector: "app-sent-sessions",
  templateUrl: "./sent-sessions.component.html",
})
export class SentSessionsComponent {
  Urls = Urls;

  sessions: SessionReport[];
  sessionsFiltered: SessionReport[];
  loadSessionsCallEnded = false;
  keyword: string = null;
  sessionNameMap: Map<SessionType, string>;
  offlineEvent: Observable<Event>;
  onlineEvent: Observable<Event>;
  subscriptions: Subscription[] = [];
  isOnline: boolean = true;
  totalItems = 0; // total number of items, e.g. from API
  currentPage = 0;
  offset = 0;
  pageSize = 25;
  pageSizeOptions = [25,30];

  faCalendar = faCalendar;
  faSearch = faSearch;

  constructor(private sentSessionsService: SentSessionsService) {
    this.sessionNameMap = SessionTypeMapper.getNameMap();
  }


    loadSessionsPaginated(offset, limit) {
        const paginationRequest: PaginationRequest = {
          take: limit,
          skip: offset
        };
        return this.sentSessionsService.getSessionsPaginated(paginationRequest);
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

  onPageChange(event: PageEvent) {
    this.currentPage = event.pageIndex;
    this.pageSize = event.pageSize;
    this.offset = this.currentPage * this.pageSize;

    this.loadSessionsCallEnded = false;
    this.loadSessionsPaginated(this.offset, this.pageSize).subscribe((result: SessionsPaginatedResponse) => {
        this.sessions = result.sessionReports
          .sort((a, b) => {
            if (a.startDate > b.startDate) {
              return -1;
            }
            if (a.startDate < b.startDate) {
              return 1;
            }
            return 0;
          });
        this.sessionsFiltered = this.sessions;
        this.loadSessionsCallEnded = true;
      },
      (error) => {
        this.loadSessionsCallEnded = true;
      }
    );
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
      this.loadSessionsCallEnded = false;
      this.loadSessionsPaginated(this.offset, this.pageSize).subscribe((result: SessionsPaginatedResponse) => {
      this.totalItems = result?.totalCount ? result.totalCount : 0;
      if (this.totalItems && this.totalItems > this.pageSizeOptions.slice(-1)[0]) {
        this.pageSizeOptions.push(this.totalItems);
      }
      this.sessions = result.sessionReports
        .sort((a, b) => {
          if (a.startDate > b.startDate) {
            return -1;
          }
          if (a.startDate < b.startDate) {
            return 1;
          }
          return 0;
        });
      this.sessionsFiltered = this.sessions;
      this.loadSessionsCallEnded = true;
    },
    (error) => {
      this.loadSessionsCallEnded = true;
    }
    );
    }
  }
}
