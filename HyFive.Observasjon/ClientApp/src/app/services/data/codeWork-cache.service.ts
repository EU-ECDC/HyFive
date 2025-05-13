import { Injectable } from '@angular/core';
import { ActivityService } from './activity.service';
import { IndicationService } from './indication.service';
import { HandJewelryTypeService } from './hand-jewelry-type.service';
import { HandHygieneAfterGloveUseTypeService } from './hand-hygiene-after-glove-useType-service';
import { GloveWithIndicationTypeService } from './glove-with-indication-type.service';
import { GloveWithoutIndicationTypeService } from './glove-without-indication-type.service';
import { ProtectiveEquipmentCodingService } from './protectiveEquipment-coding-service';
import { forkJoin, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CodeWorkCacheService {

  constructor(private activityService: ActivityService,
    private indicationService: IndicationService,
    private handJewelryTypeService: HandJewelryTypeService,
    private handHygieneAfterGloveUseTypeService: HandHygieneAfterGloveUseTypeService,
    private gloveWithIndicationTypeService: GloveWithIndicationTypeService,
    private gloveWithoutIndicationTypeService: GloveWithoutIndicationTypeService,   
    private protectiveEquipmentCodingService: ProtectiveEquipmentCodingService ) {
  }

  loadCodeworks(){
    const codeworkRequests = [
      this.activityService.getActivityTypes(),
      this.indicationService.getIndicationTypes(),
      this.handJewelryTypeService.getHandJewelryTypes(),
      this.handHygieneAfterGloveUseTypeService.getHandhygieneAfterGloveUseTypes(),
      this.gloveWithIndicationTypeService.getGloveWithIndicationTypes(),
      this.gloveWithoutIndicationTypeService.getGloveWithoutIndicationTypes(),
      this.protectiveEquipmentCodingService.getProtectiveEquipmentSettings()
    ];

    forkJoin(codeworkRequests).subscribe( () => {});
  }
}
