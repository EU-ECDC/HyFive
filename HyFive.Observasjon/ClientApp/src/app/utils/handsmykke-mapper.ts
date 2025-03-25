import { HandsmykkeTypeKonstanter } from '../models/api/HandsmykkeTypeKonstanter';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { faTshirt } from '@fortawesome/free-solid-svg-icons';
import { faThumbsUp } from '@fortawesome/free-regular-svg-icons';
import { Handsmykkevalg } from '../models/registrering/handsmykkevalg.model';
import { HandsmykkeType } from '../models/api/HandsmykkeType';
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

  public static getHandsmykkevalg(handsmykkeAlternativer: HandsmykkeType[], handsmykkeValg: string[]): Handsmykkevalg[] {
    return handsmykkeAlternativer.reduce((acc, item) => {
      acc.push({
        erValgt: handsmykkeValg.indexOf(item.kode) != -1,
        navn: item.navn,
        type: item.kode,
        disabled: false
      });
      return acc;
    }, [] as Handsmykkevalg[]);
  }
}
