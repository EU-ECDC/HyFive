import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProtectiveEquipmentSession } from '../../../models/api/ProtectiveEquipmentSession';
import { Queryparameters } from '../../../constants/queryparameters';
import { ActivatedRoute, Router } from '@angular/router';
import {faFileExcel, faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import { Urls } from 'src/app/constants/urls';
import { ProtectiveEquipmentObservation } from '../../../models/api/ProtectiveEquipmentObservation';
import { ProtectiveEquipment } from '../../../models/api/ProtectiveEquipment';
import {ToastrService} from 'ngx-toastr';
import {DialogueTexts} from '../../../constants/dialogueTexts';
import { SentSessionsService } from 'src/app/services/data/sent-sessions.service';

@Component({
  selector: 'app-sent-protective-equipment-session',
  templateUrl: './sent-protective-equipment-session.component.html'
})
export class SentProtectiveEquipmentSessionComponent implements OnInit, OnDestroy {

  session: ProtectiveEquipmentSession = null;

  Urls = Urls;
  faArrowLeft = faLongArrowAltLeft;
  isOnline: boolean = true;
  faFileExcel = faFileExcel;
  downloadAsExcel: boolean;
  DialogueTexts = DialogueTexts;

  constructor(
    private sentSessionsService: SentSessionsService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        if (sessionId === 0) this.router.navigate([Urls.SentSessionsUrl]);
        this.sentSessionsService.getProtectiveEquipmentSession(sessionId).subscribe(
          (session) => {
            this.session = session;
            if (!this.session) this.router.navigate([Urls.SentSessionsUrl]);
          }
        );
      });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  getIngress(observation: ProtectiveEquipmentObservation) {
    return observation?.settingType?.name;
  }

  navigateToSentSessions() {
    this.router.navigate([Urls.SentSessionsUrl])
  }

  showEquipment(protectiveEquipment: ProtectiveEquipment[]): string {
    if (protectiveEquipment?.length > 0) {
      return protectiveEquipment.filter(b => b.wasUsed).map(b => b.equipmentType.name).join(', ');
    }
    return "";
  }

  downloadAsExcelFnct() {
    this.downloadAsExcel = true;
    this.sentSessionsService.DownloadProtectiveEquipmentSessionAsExcel(this.session.facilityId, this.session.id).subscribe(
        () => {},
        error => this.toastrService.error(error?.message ? error.message : error, DialogueTexts.ErrorDuringDownloadSessionExcel, {disableTimeOut: true}),
        () => this.downloadAsExcel = false)
  }
}
