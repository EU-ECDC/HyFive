import { Injectable } from '@angular/core';
import { ActivityService } from './activity.service';
import { IndikasjonService } from './indikasjon.service';
import { HandJewelryTypeService } from './hand-jewelry-type.service';
import { HandHygieneAfterGloveUseTypeService } from './hand-hygiene-after-glove-useType-service';
import { GloveWithIndicationTypeService } from './glove-with-indication-type.service';
import { GloveWithoutIndicationTypeService } from './glove-without-indication-type.service';
import { ProtectiveEquipmentCodingService } from './protectiveEquipment-coding-service';
import { forkJoin, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class KodeverkCacheService {

  constructor(private activityService: ActivityService,
    private indikasjonService: IndikasjonService,
    private handJewelryTypeService: HandJewelryTypeService,
    private handHygieneAfterGloveUseTypeService: HandHygieneAfterGloveUseTypeService,
    private gloveWithIndicationTypeService: GloveWithIndicationTypeService,
    private gloveWithoutIndicationTypeService: GloveWithoutIndicationTypeService,   
    private protectiveEquipmentCodingService: ProtectiveEquipmentCodingService ) {
  }

  lastKodeverk(){
    const kodeverkRequests = [
      this.activityService.getActivityTypes(),
      this.indikasjonService.getIndikasjonstyper(),
      this.handJewelryTypeService.getHandJewelryTypes(),
      this.handHygieneAfterGloveUseTypeService.getHandhygieneAfterGloveUseTypes(),
      this.gloveWithIndicationTypeService.getGloveWithIndicationTypes(),
      this.gloveWithoutIndicationTypeService.getHanskeUtenIndikasjonTyper(),
      this.protectiveEquipmentCodingService.getProtectiveEquipmentSettings()
    ];

    forkJoin(kodeverkRequests).subscribe( () => {});
  }
}
