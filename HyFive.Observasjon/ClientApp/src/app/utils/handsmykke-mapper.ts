import { HandJewelryTypeConstants } from '../models/api/HandJewelryTypeConstants';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { faTshirt } from '@fortawesome/free-solid-svg-icons';
import { faThumbsUp } from '@fortawesome/free-regular-svg-icons';
import { Handsmykkevalg } from '../models/registration/handJewelry-selection.model';
import { HandJewelryType } from '../models/api/HandJewelryType';
import { faLangermet, faKunstigNegl, faLangNegl, faKlokke, faRing } from './customIkoner';

export class HandsmykkeMapper {

  public static getIkontypeMap(): Map<HandJewelryTypeConstants, IconProp> {
    var ikonmap = new Map<HandJewelryTypeConstants, IconProp>();
    ikonmap.set(HandJewelryTypeConstants.AllClear, faThumbsUp);
    ikonmap.set(HandJewelryTypeConstants.Ring, faRing);
    ikonmap.set(HandJewelryTypeConstants.WatchBracelet, faKlokke);
    ikonmap.set(HandJewelryTypeConstants.LongNails, faLangNegl);
    ikonmap.set(HandJewelryTypeConstants.ArtificialNailsShellac, faKunstigNegl);
    ikonmap.set(HandJewelryTypeConstants.ShortSleeved, faTshirt);
    ikonmap.set(HandJewelryTypeConstants.LongSleeved, faLangermet);
    return ikonmap;
  }

  public static getHandsmykkevalg(handsmykkeAlternativer: HandJewelryType[], handsmykkeValg: string[]): Handsmykkevalg[] {
    return handsmykkeAlternativer.reduce((acc, item) => {
      acc.push({
        isSelected: handsmykkeValg.indexOf(item.code) != -1,
        name: item.name,
        type: item.code,
        disabled: false
      });
      return acc;
    }, [] as Handsmykkevalg[]);
  }
}
