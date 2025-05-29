import { HandJewelryTypeConstants } from '../models/api/HandJewelryTypeConstants';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { faTshirt } from '@fortawesome/free-solid-svg-icons';
import { faThumbsUp } from '@fortawesome/free-regular-svg-icons';
import { HandJewelrySelection } from '../models/registration/handJewelry-selection.model';
import { HandJewelryType } from '../models/api/HandJewelryType';
import { faLangermet, faKunstigNegl, faLangNegl, faKlokke, faRing } from './customIconer';

export class HandJewelryMapper {

  public static getIconTypeMap(): Map<HandJewelryTypeConstants, IconProp> {
    var iconmap = new Map<HandJewelryTypeConstants, IconProp>();
    iconmap.set(HandJewelryTypeConstants.AllOk, faThumbsUp);
    iconmap.set(HandJewelryTypeConstants.Ring, faRing);
    iconmap.set(HandJewelryTypeConstants.WatchBracelet, faKlokke);
    iconmap.set(HandJewelryTypeConstants.LongNail, faLangNegl);
    iconmap.set(HandJewelryTypeConstants.ArtificialNailShellac, faKunstigNegl);
    iconmap.set(HandJewelryTypeConstants.ShortSleeves, faTshirt);
    iconmap.set(HandJewelryTypeConstants.LongSleeves, faLangermet);
    return iconmap;
  }

  public static getHandjewelrySelection(handjewelryChoices: HandJewelryType[], handjewelrySelection: string[]): HandJewelrySelection[] {
    return handjewelryChoices.reduce((acc, item) => {
      acc.push({
        isSelected: handjewelrySelection.indexOf(item.code) != -1,
        name: item.name,
        type: item.code,
        disabled: false
      });
      return acc;
    }, [] as HandJewelrySelection[]);
  }
}
