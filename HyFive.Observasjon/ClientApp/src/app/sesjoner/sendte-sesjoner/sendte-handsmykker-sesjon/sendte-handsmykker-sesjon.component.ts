import { Component, OnInit, OnDestroy } from '@angular/core';
import { SentSessionsService } from '../../../services/data/sendte-sessions.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../../constants/queryparameters';
import { Urls } from '../../../constants/urls';
import { HandJewelryMapper } from 'src/app/utils/handJewelry-mapper';
import { HandJewelryType } from 'src/app/models/api/HandJewelryType';
import { HandJewelrySession } from '../../../models/api/HandJewelrySession';
import { HandJewelryTypeService } from '../../../services/data/hand-jewelry-type.service';
import { faFileExcel } from '@fortawesome/free-regular-svg-icons';
import {ToastrService} from 'ngx-toastr';
import {DialogueTexts} from '../../../constants/dialogueTexts';

@Component({
  selector: 'app-sendte-handsmykker-sesjon',
  templateUrl: './sendte-handsmykker-sesjon.component.html'
})
export class SendteHandsmykkerSesjonComponent implements OnInit, OnDestroy {

  sesjon: HandJewelrySession;
  handJewelryTypes: HandJewelryType[] = [];
  isOnline: boolean = true;

  faFileExcel = faFileExcel;
  lasterNedSomExcel = false;
  DialogueTexts = DialogueTexts;

  constructor(private router: Router,
    private route: ActivatedRoute,
    private sesjonService: SentSessionsService,
    private handJewelryTypeService: HandJewelryTypeService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const sessionId = params[Queryparameters.SessionId] || 0;
      if (sessionId === 0) this.router.navigate(['']);
      this.sesjonService.getHandJewelrySession(sessionId).subscribe(
        (sesjon) => {
          this.sesjon = sesjon;
          if (!sesjon) this.router.navigate(['']);
        }
      );
    });
    this.handJewelryTypeService.getHandJewelryTypes().subscribe((handJewelryTypes) => {
      this.handJewelryTypes = handJewelryTypes;
    });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  visHandsmykker(handJewelry: HandJewelryType[]): string {
    return HandJewelryMapper.getHandjewelrySelection(this.handJewelryTypes, handJewelry.map(x => x.code)).filter(h => h.isSelected == true).map(h => h.name).join(', ');
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }

  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.downloadHandJewelrySessionAsExcel(this.sesjon.institutionId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, DialogueTexts.ErrorDuringDownloadSessionExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
