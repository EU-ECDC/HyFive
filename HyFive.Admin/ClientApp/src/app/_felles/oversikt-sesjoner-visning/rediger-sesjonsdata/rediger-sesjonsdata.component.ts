import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {SesjonOversiktRapport} from "../../../models/api/SesjonOversiktRapport";
import {SesjonService} from "../../../services/data/sesjon.service";
import {OppdaterSesjonRequest} from "../../../models/api/OppdaterSesjonRequest";
import {ToastrService} from "ngx-toastr";
import {DatoHjelper} from "../../../utils/datohjelper";

@Component({
  selector: 'app-rediger-sesjonsdata',
  templateUrl: './rediger-sesjonsdata.component.html'
})
export class RedigerSesjonsdataComponent implements OnInit {

  @Input() sesjon: SesjonOversiktRapport;


  @Input() institusjonId: number;
  @Input() kanRedigere: boolean;

  harEndretDato: boolean;
  harEndretKommentar: boolean;
  redigeringsmodus: boolean;

  sesjonkopi: SesjonOversiktRapport;

  constructor(private sesjonService : SesjonService, private toastrService: ToastrService) {
  }

  ngOnInit(): void {
    this.sesjonkopi = JSON.parse(JSON.stringify(this.sesjon));
    this.resetState();
  }

  lagre() {
    if(this.kanLagre()){
      var request: Partial<OppdaterSesjonRequest> = {
        sesjonId: this.sesjonkopi.id,
        institusjonId:  this.institusjonId,
      }

      if(this.harEndretKommentar){
        request.kommentar = this.sesjonkopi.kommentar;
      }

      console.log("Rett før innsending Request:", request);

      this.sesjonService.oppdaterSesjon(request).subscribe((result) => {
        this.toastrService.success("Oppdatering av sesjonsdata var vellykket");
          if(this.harEndretKommentar){
            this.sesjon.kommentar = this.sesjonkopi.kommentar;
          }
          this.resetState();
        },
        (error) => this.toastrService.error('Det oppstod en feil under oppdatering av sesjonsdata: ' + error?.message, '', { disableTimeOut: true})
      );
    }

  }

  private resetState() {
    this.harEndretKommentar = false;
    this.harEndretDato = false;
    this.redigeringsmodus = false;
    this.sesjonkopi = JSON.parse(JSON.stringify(this.sesjon))
  }

  kanLagre() {
    return this.harEndretDato || this.harEndretKommentar;
  }
}
