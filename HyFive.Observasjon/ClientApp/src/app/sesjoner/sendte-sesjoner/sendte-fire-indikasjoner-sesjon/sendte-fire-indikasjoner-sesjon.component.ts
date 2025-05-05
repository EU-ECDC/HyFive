import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faTrashAlt, faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import { Dialogtekster } from '../../../konstanter/dialogtekster';
import { Queryparameters } from '../../../konstanter/queryparameters';
import { Urls } from '../../../konstanter/urls';
import { FourIndicationsObservation } from '../../../models/api/FourIndicationsObservation';
import { ActivityType } from '../../../models/api/ActivityType';
import { AktivitetService } from '../../../services/data/aktivitet.service';
import { ActivityTypeConstants } from 'src/app/models/api/ActivityTypeConstants';
import { SendteSesjonerService } from '../../../services/data/sendte-sessions.service';
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
  erOnline: boolean = true;
  lasterNedSomExcel = false;

  faArrowLeft = faLongArrowAltLeft;
  faTrashAlt = faTrashAlt;
  Dialogtekster = Dialogtekster;
  Urls = Urls;

  constructor(
    private sesjonService: SendteSesjonerService,
    private router: Router,
    private route: ActivatedRoute,
    private aktivitetService: AktivitetService,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SesjonId] || 0;
        if(sessionId === 0) this.router.navigate(['']);
        this.sesjonService.hentFireIndikasjonerSesjon(sessionId).subscribe(
          (sesjon) => {
            this.sesjon = sesjon;
            if (!this.sesjon) this.router.navigate(['']);
          }
        );
      });

      this.aktivitetService.getAktivitetTyper().subscribe((activityTypes) => {
        this.activityTypes = activityTypes;
      });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  navigerTilSendteSesjoner(){
    this.router.navigate([Urls.SendteSesjonerUrl])
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

  hentIngress(observasjon: FourIndicationsObservation) {
    return this.activityTypes?.find(x => x.code === observasjon.activity.activityType?.code)?.name + ' - ' + observasjon.indicationTypes.map(i => i.name).join(', ');
  }

  mottattInternetStatus(harInternett: boolean){
    this.erOnline = harInternett;
    if(this.erOnline) {

    }
  }

  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.lastNedFireIndikasjonerSesjonSomExcel(this.sesjon.institutionId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, Dialogtekster.FeilUnderNedlastingSesjonExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
