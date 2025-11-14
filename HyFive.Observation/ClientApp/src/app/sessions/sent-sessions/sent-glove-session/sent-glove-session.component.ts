import { Component, OnInit, OnDestroy } from '@angular/core';
import { SentSessionsService } from '../../../services/data/sent-sessions.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../../constants/queryparameters';
import { Urls } from '../../../constants/urls';
import { HandJewelryType } from 'src/app/models/api/HandJewelryType';
import { GloveSession } from '../../../models/api/GloveSession';
import { GloveObservation } from '../../../models/api/GloveObservation';
import {ToastrService} from 'ngx-toastr';
import { faFileExcel } from '@fortawesome/free-regular-svg-icons';
import {DialogueTexts} from '../../../constants/dialogueTexts';

@Component({
  selector: 'app-sent-glove-session',
  templateUrl: './sent-glove-session.component.html'
})
export class SentGloveSessionComponent implements OnInit, OnDestroy {

  session: GloveSession;
  handJewelryTypes: HandJewelryType[] = [];
  isOnline: boolean = true;
  faFileExcel = faFileExcel
  DialogueTexts = DialogueTexts;

  private downloadAsExcel: boolean;

  constructor(private readonly router: Router,
              private readonly route: ActivatedRoute,
              private readonly sessionService: SentSessionsService,
              private readonly toastrService: ToastrService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const sessionId = params[Queryparameters.SessionId] || 0;
      if (sessionId === 0) this.router.navigate(['']);
      this.sessionService.getGloveSession(sessionId).subscribe(
        (session) => {
          this.session = session;
          if (!session) this.router.navigate(['']);
        }
      )
    });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  showIndications(item: GloveObservation): string { // Choose between displaying indications/types, or showing whether the observation was with or without indications
    if (item.gloveWithIndicationTypes?.length) return item.gloveWithIndicationTypes?.map(x => x.name).join(', ');
    return item.gloveWithoutIndicationTypes?.map(x => x.name).join(', ');
  }

  navigateToSentSessions() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }
  
  downloadAsExcelFnct() {
    this.downloadAsExcel = true;
    this.sessionService.downloadGloveSessionAsExcel(this.session.facilityId, this.session.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, DialogueTexts.ErrorDuringDownloadSessionExcel, {disableTimeOut: true}),
      () => this.downloadAsExcel = false)
  }
}
