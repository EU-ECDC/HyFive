import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { InstitutionService } from 'src/app/services/data/institution.service';
import { ForesporselOmBrukertilgang } from "../../models/api/ForesporselOmBrukertilgang";
import { ForesporselOmBrukertilgangService } from "../../services/data/foresporselombrukertilgang.service";
import { ForesporselStatus } from 'src/app/models/api/ForesporselStatus';

@Component({
  selector: 'app-request',
  templateUrl: './request.component.html'
})
export class RequestComponent implements OnInit, OnDestroy {

  requests: ForesporselOmBrukertilgang[];
  visAlleForesporsel: boolean = false;
  foresporselStatus = ForesporselStatus;

  constructor(
    private foresporselOmBrukertilgangService: ForesporselOmBrukertilgangService,
    private institutionService: InstitutionService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.lastForesporslerSomVenterPaGodkjenning();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  lastAlleForesporsler() {
    this.visAlleForesporsel = true;
    var institutionId = this.institutionService.getSelectedInstitutionId();
    this.foresporselOmBrukertilgangService.hentAlleForesporsler(institutionId).subscribe(
      (requests) => this.requests = requests,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av forespørsler om brukertilgang: ' + error?.message, '', { disableTimeOut: true})
    );
  }

  lastForesporslerSomVenterPaGodkjenning() {
    this.visAlleForesporsel = false;
    var institutionId = this.institutionService.getSelectedInstitutionId();
    this.foresporselOmBrukertilgangService.hentForesporslerSomVenterPaGodkjenning(institutionId).subscribe(
      (requests) => this.requests = requests,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av forespørsler om brukertilgang: ' + error?.message, '', { disableTimeOut: true})
    );
  }

  godkjennForesporsel(foresporselId: number) {
    this.foresporselOmBrukertilgangService.godkjennForesporsel(foresporselId).subscribe(
      () => {
        if(this.visAlleForesporsel)
          this.lastAlleForesporsler();
        else
          this.lastForesporslerSomVenterPaGodkjenning();
        this.toastrService.success("Forspørselen er godkjent");
      },
      (error) => this.toastrService.error('Det oppstod en feil under godkjenning av forespørsel om brukertilgang: ' + error?.message, '', { disableTimeOut: true})
    );
  }

  avvisForesporsel(foresporselId: number) {
    this.foresporselOmBrukertilgangService.avvisForesporsel(foresporselId).subscribe(
      () => {
        if(this.visAlleForesporsel)
          this.lastAlleForesporsler();
        else
          this.lastForesporslerSomVenterPaGodkjenning();
        this.toastrService.success("Forespørselen er avvist");
      },
      (error) => this.toastrService.error('Det oppstod en feil under avvisningen av forespørsel om brukertilgang: ' + error?.message, '', { disableTimeOut: true})
    );
  }

}
