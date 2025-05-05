import { Component, OnInit, OnDestroy } from '@angular/core';
import { SendteSesjonerService } from '../../../services/data/sendte-sessions.service';
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
  selector: 'app-sendte-hanske-sesjon',
  templateUrl: './sendte-hanske-sesjon.component.html'
})
export class SendteHanskeSesjonComponent implements OnInit, OnDestroy {

  sesjon: GloveSession;
  handJewelryTypes: HandJewelryType[] = [];
  erOnline: boolean = true;
  faFileExcel = faFileExcel
  DialogueTexts = DialogueTexts;

  private lasterNedSomExcel: boolean;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private sesjonService: SendteSesjonerService,
              private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const sessionId = params[Queryparameters.SessionId] || 0;
      if (sessionId === 0) this.router.navigate(['']);
      this.sesjonService.hentHanskeSesjon(sessionId).subscribe(
        (sesjon) => {
          this.sesjon = sesjon;
          if (!sesjon) this.router.navigate(['']);
        }
      )
    });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  visIndikasjoner(item: GloveObservation): string { // TODO Velge mellom visning av indikasjoner/typer, eller vise om observasjon var med eller uten indikasjoner
    if (item.gloveWithIndicationTypes.length) return item.gloveWithIndicationTypes.map(x => x.name).join(', ');;
    return item.gloveWithoutIndicationTypes.map(x => x.name).join(', ');
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }
  
  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.lastNedHanskeSesjonSomExcel(this.sesjon.institutionId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, DialogueTexts.ErrorDuringDownloadSessionExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
