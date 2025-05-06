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
import { SentSessionsService } from '../../../services/data/sendte-sessions.service';
import {FourIndicationsSession} from '../../../models/api/FourIndicationsSession';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-sendte-fire-indikasjoner',
  templateUrl: './sendte-fire-indikasjoner-sesjon.component.html',
})
export class SendteFireIndikasjonerSesjonComponent implements OnInit, OnDestroy {

  sesjon: FourIndicationsSession;
  sesjonErSendtTilServer = false;
  activityTypes: ActivityType[];
  isOnline: boolean = true;
  lasterNedSomExcel = false;

  faArrowLeft = faLongArrowAltLeft;
  faTrashAlt = faTrashAlt;
  DialogueTexts = DialogueTexts;
  Urls = Urls;

  constructor(
    private sesjonService: SentSessionsService,
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
        this.sesjonService.getFourIndicationsSession(sessionId).subscribe(
          (sesjon) => {
            this.sesjon = sesjon;
            if (!this.sesjon) this.router.navigate(['']);
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

  navigerTilSendteSesjoner(){
    this.router.navigate([Urls.SentSessionsUrl])
  }

  beregnAnledningerEtterlevd(sesjon: FourIndicationsSession) : number{
    if(sesjon?.observations?.length == 0)
      return 0;
    return sesjon?.observations?.filter(f => f.activity.activityType?.code != ActivityTypeConstants.NotExecuted).length
  }

  beregnAnledningerEtterlevdProsent(sesjon: FourIndicationsSession): number {
    if (sesjon?.observations?.length == 0)
      return 0;
    return (this.beregnAnledningerEtterlevd(sesjon) / sesjon?.observations?.length) * 100;
  }

  beregnAnledningerUtelatt(sesjon: FourIndicationsSession) : number{
    if(sesjon?.observations?.length == 0)
      return 0;
    return sesjon?.observations?.filter(f => f.activity.activityType?.code == ActivityTypeConstants.NotExecuted).length
  }

  beregnAnledningerUtelattProsent(sesjon: FourIndicationsSession) : number{
    if(sesjon?.observations?.length == 0)
      return 0;
    return (this.beregnAnledningerUtelatt(sesjon) / sesjon?.observations?.length)*100
  }

  hentIngress(observation: FourIndicationsObservation) {
    return this.activityTypes?.find(x => x.code === observation.activity.activityType?.code)?.name + ' - ' + observation.indicationTypes.map(i => i.name).join(', ');
  }

  mottattInternetStatus(harInternett: boolean){
    this.isOnline = harInternett;
    if(this.isOnline) {

    }
  }

  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.downloadFourIndicationsSessionAsExcel(this.sesjon.institutionId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, DialogueTexts.ErrorDuringDownloadSessionExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
