import { Component, OnInit, OnDestroy } from '@angular/core';
import { SendteSesjonerService } from '../../../services/data/sendte-sessions.service';
import { ProtectiveEquipmentSession } from '../../../models/api/ProtectiveEquipmentSession';
import { Queryparameters } from '../../../konstanter/queryparameters';
import { ActivatedRoute, Router } from '@angular/router';
import {faFileExcel, faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import { Urls } from 'src/app/konstanter/urls';
import { ProtectiveEquipmentObservation } from '../../../models/api/ProtectiveEquipmentObservation';
import { ProtectiveEquipment } from '../../../models/api/ProtectiveEquipment';
import {ToastrService} from 'ngx-toastr';
import {Dialogtekster} from '../../../konstanter/dialogtekster';

@Component({
  selector: 'app-sendte-beskyttelsesutstyr-sesjon',
  templateUrl: './sendte-beskyttelsesutstyr-sesjon.component.html'
})
export class SendteBeskyttelsesutstyrSesjonComponent implements OnInit, OnDestroy {

  sesjon: ProtectiveEquipmentSession = null;

  Urls = Urls;
  faArrowLeft = faLongArrowAltLeft;
  erOnline: boolean = true;
  faFileExcel = faFileExcel;
  lasterNedSomExcel: boolean;
  Dialogtekster = Dialogtekster;

  constructor(
    private sendteSesjonerService: SendteSesjonerService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SesjonId] || 0;
        if (sessionId === 0) this.router.navigate([Urls.SendteSesjonerUrl]);
        this.sendteSesjonerService.hentBeskyttelsesutstyrSesjon(sessionId).subscribe(
          (sesjon) => {
            this.sesjon = sesjon;
            if (!this.sesjon) this.router.navigate([Urls.SendteSesjonerUrl]);
          }
        );
      });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  hentIngress(observasjon: ProtectiveEquipmentObservation) {
    return observasjon.settingtype.name;
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl])
  }

  visUtstyr(protectiveEquipment: ProtectiveEquipment[]): string {
    if (protectiveEquipment?.length > 0) {
      return protectiveEquipment.filter(b => b.wasUsed).map(b => b.equipmentType.name).join(', ');
    }
    return "";
  }

  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sendteSesjonerService.lastNedBeskyttelsesutstyrSesjonSomExcel(this.sesjon.institutionId, this.sesjon.id).subscribe(
        () => {},
        error => this.toastrService.error(error?.message ? error.message : error, Dialogtekster.FeilUnderNedlastingSesjonExcel, {disableTimeOut: true}),
        () => this.lasterNedSomExcel = false)
  }
}
