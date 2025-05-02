import { HandsmykkeTypeKonstanter } from '../models/api/HandsmykkeTypeKonstanter';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { faTshirt } from '@fortawesome/free-solid-svg-icons';
import { faThumbsUp } from '@fortawesome/free-regular-svg-icons';
import { Handsmykkevalg } from '../models/registrering/handsmykkevalg.model';
import { HandJewelryType } from '../models/api/HandJewelryType';
import { faLangermet, faKunstigNegl, faLangNegl, faKlokke, faRing } from './customIkoner';

export class HandsmykkeMapper {

  public static getIkontypeMap(): Map<HandsmykkeTypeKonstanter, IconProp> {
    var ikonmap = new Map<HandsmykkeTypeKonstanter, IconProp>();
    ikonmap.set(HandsmykkeTypeKonstanter.AltOk, faThumbsUp);
    ikonmap.set(HandsmykkeTypeKonstanter.Ring, faRing);
    ikonmap.set(HandsmykkeTypeKonstanter.KlokkeArmband, faKlokke);
    ikonmap.set(HandsmykkeTypeKonstanter.LangNegl, faLangNegl);
    ikonmap.set(HandsmykkeTypeKonstanter.KunstigNeglShellac, faKunstigNegl);
    ikonmap.set(HandsmykkeTypeKonstanter.Kortermet, faTshirt);
    ikonmap.set(HandsmykkeTypeKonstanter.Langermet, faLangermet);
    return ikonmap;
  }

  public static getHandsmykkevalg(handsmykkeAlternativer: HandJewelryType[], handsmykkeValg: string[]): Handsmykkevalg[] {
    return handsmykkeAlternativer.reduce((acc, item) => {
      acc.push({
        erValgt: handsmykkeValg.indexOf(item.code) != -1,
        name: item.name,
        type: item.code,
        disabled: false
      });
      return acc;
    }, [] as Handsmykkevalg[]);
  }
}
