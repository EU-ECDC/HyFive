import { Injectable } from '@angular/core';
import { AktivitetService } from './aktivitet.service';
import { IndikasjonService } from './indikasjon.service';
import { HandsmykkeTypeService } from './handsmykketype.service';
import { HandhygieneEtterHanskebrukTypeService } from './handhygiene-etter-hanskebruk-type.service';
import { HanskeMedIndikasjonTypeService } from './hanske-med-indikasjon-type.service';
import { HanskeUtenIndikasjonTypeService } from './hanske-uten-indikasjon-type.service';
import { BeskyttelsesutstyrKodeverkService } from './beskyttelsesutstyr-kodeverk.service';
import { forkJoin, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class KodeverkCacheService {

  constructor(private aktivitetService: AktivitetService,
    private indikasjonService: IndikasjonService,
    private handsmykkeTypeService: HandsmykkeTypeService,
    private handhygieneEtterHanskebrukTypeService: HandhygieneEtterHanskebrukTypeService,
    private hanskeMedIndikasjonTypeService: HanskeMedIndikasjonTypeService,
    private hanskeUtenIndikasjonTypeService: HanskeUtenIndikasjonTypeService,   
    private beskyttelsesutstyrKodeverkService: BeskyttelsesutstyrKodeverkService ) {
  }

  lastKodeverk(){
    const kodeverkRequests = [
      this.aktivitetService.getAktivitetTyper(),
      this.indikasjonService.getIndikasjonstyper(),
      this.handsmykkeTypeService.getHandsmykkeTyper(),
      this.handhygieneEtterHanskebrukTypeService.getHandhygieneEtterHanskebrukTyper(),
      this.hanskeMedIndikasjonTypeService.getHanskeMedIndikasjonTyper(),
      this.hanskeUtenIndikasjonTypeService.getHanskeUtenIndikasjonTyper(),
      this.beskyttelsesutstyrKodeverkService.hentBeskyttelsesutstyrSettinger()
    ];

    forkJoin(kodeverkRequests).subscribe( () => {});
  }
}
