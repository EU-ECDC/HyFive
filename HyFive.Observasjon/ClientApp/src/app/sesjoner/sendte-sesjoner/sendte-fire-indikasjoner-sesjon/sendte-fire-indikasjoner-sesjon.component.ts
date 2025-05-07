import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faTrashAlt, faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import { DialogueTexts } from '../../../constants/dialogueTexts';
import { Queryparameters } from '../../../constants/queryparameters';
import { Urls } from '../../../constants/urls';
import { FourIndicationsObservation } from '../../../models/api/FourIndicationsObservation';
import { ActivityType } from '../../../models/api/ActivityType';
import { ActivityService } from '../../../services/data/activity.service';
import { ActivityTypeConstants } from 'src/app/models/api/ActivityTypeConstants';
import { SentSessionsService } from '../../../services/data/sent-sessions.service';
import {FourIndicationsSession} from '../../../models/api/FourIndicationsSession';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-sendte-fire-indikasjoner',
  templateUrl: './sendte-fire-indikasjoner-sesjon.component.html',
})
export class SendteFireIndikasjonerSesjonComponent implements OnInit, OnDestroy {

  session: FourIndicationsSession;
  sessionIsSentToServer = false;
  activityTypes: ActivityType[];
  isOnline: boolean = true;
  downloadAsExcel = false;

  faArrowLeft = faLongArrowAltLeft;
  faTrashAlt = faTrashAlt;
  DialogueTexts = DialogueTexts;
  Urls = Urls;

  constructor(
    private sessionService: SentSessionsService,
    private router: Router,
    private route: ActivatedRoute,
    private activityService: ActivityService,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        if(sessionId === 0) this.router.navigate(['']);
        this.sessionService.getFourIndicationsSession(sessionId).subscribe(
          (session) => {
            this.session = session;
            if (!this.session) this.router.navigate(['']);
          }
        );
      });

      this.activityService.getActivityTypes().subscribe((activityTypes) => {
        this.activityTypes = activityTypes;
      });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  navigateToSentSessions(){
    this.router.navigate([Urls.SentSessionsUrl])
  }

  calculateOccasionsComplied(session: FourIndicationsSession) : number{
    if(session?.observations?.length == 0)
      return 0;
    return session?.observations?.filter(f => f.activity.activityType?.code != ActivityTypeConstants.NotExecuted).length
  }

  calculateOccasionsCompliedPercent(session: FourIndicationsSession): number {
    if (session?.observations?.length == 0)
      return 0;
    return (this.calculateOccasionsComplied(session) / session?.observations?.length) * 100;
  }

  calculateOccasionsOmitted(session: FourIndicationsSession) : number{
    if(session?.observations?.length == 0)
      return 0;
    return session?.observations?.filter(f => f.activity.activityType?.code == ActivityTypeConstants.NotExecuted).length
  }

  calculateOccasionsOmittedPercent(session: FourIndicationsSession) : number{
    if(session?.observations?.length == 0)
      return 0;
    return (this.calculateOccasionsOmitted(session) / session?.observations?.length)*100
  }

  getIngress(observation: FourIndicationsObservation) {
    return this.activityTypes?.find(x => x.code === observation.activity.activityType?.code)?.name + ' - ' + observation.indicationTypes.map(i => i.name).join(', ');
  }

  mottattInternetStatus(hasInternet: boolean){
    this.isOnline = hasInternet;
    if(this.isOnline) {

    }
  }

  downloadAsExcelFnct() {
    this.downloadAsExcel = true;
    this.sessionService.downloadFourIndicationsSessionAsExcel(this.session.institutionId, this.session.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, DialogueTexts.ErrorDuringDownloadSessionExcel, {disableTimeOut: true}),
      () => this.downloadAsExcel = false)
  }
}
