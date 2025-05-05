import { Injectable } from '@angular/core';
import { ActivityService } from './activity.service';
import { IndikasjonService } from './indikasjon.service';
import { HandsmykkeTypeService } from './handsmykketype.service';
import { HandhygieneEtterHanskebrukTypeService } from './handhygiene-etter-hanskebruk-type.service';
import { HanskeMedIndikasjonTypeService } from './hanske-med-indikasjon-type.service';
import { HanskeUtenIndikasjonTypeService } from './hanske-uten-indikasjon-type.service';
import { ProtectiveEquipmentCodingService } from './protectiveEquipment-coding-service';
import { forkJoin, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class KodeverkCacheService {

  constructor(private activityService: ActivityService,
    private indikasjonService: IndikasjonService,
    private handsmykkeTypeService: HandsmykkeTypeService,
    private handhygieneEtterHanskebrukTypeService: HandhygieneEtterHanskebrukTypeService,
    private hanskeMedIndikasjonTypeService: HanskeMedIndikasjonTypeService,
    private hanskeUtenIndikasjonTypeService: HanskeUtenIndikasjonTypeService,   
    private protectiveEquipmentCodingService: ProtectiveEquipmentCodingService ) {
  }

  lastKodeverk(){
    const kodeverkRequests = [
      this.activityService.getActivityTypes(),
      this.indikasjonService.getIndikasjonstyper(),
      this.handsmykkeTypeService.getHandsmykkeTyper(),
      this.handhygieneEtterHanskebrukTypeService.getHandhygieneEtterHanskebrukTyper(),
      this.hanskeMedIndikasjonTypeService.getHanskeMedIndikasjonTyper(),
      this.hanskeUtenIndikasjonTypeService.getHanskeUtenIndikasjonTyper(),
      this.protectiveEquipmentCodingService.getProtectiveEquipmentSettings()
    ];

    forkJoin(kodeverkRequests).subscribe( () => {});
  }
}
