import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndikasjonTypeKonstanter } from '../models/api/IndikasjonTypeKonstanter';
import { IndicationType } from '../models/api/IndicationType';
import { IndikasjonTypeValg } from '../models/registrering/indikasjontypevalg.model';
import { faFour, faOne, faThree, faTwo } from './customIkoner';

export class IndikasjonTypeMapper {

  public static getIkontypeMap(): Map<IndikasjonTypeKonstanter, IconProp> {
    var ikonmap = new Map<IndikasjonTypeKonstanter, IconProp>();
    ikonmap.set(IndikasjonTypeKonstanter.FoerPasient, faOne);
    ikonmap.set(IndikasjonTypeKonstanter.AseptiskeProsedyrer, faTwo);
    ikonmap.set(IndikasjonTypeKonstanter.Kroppsveske, faThree);
    ikonmap.set(IndikasjonTypeKonstanter.EtterPasient, faFour);
    return ikonmap;
  }

  public static getIndikasjonstypeValg(indicationTypes: IndicationType[], valgteIndikasjonTyper: IndicationType[]): IndikasjonTypeValg[] {
    return indicationTypes.reduce((acc, item) => {
      acc.push({
        erValgt: valgteIndikasjonTyper.some(x => x.code === item.code),
        name: item.name,
        code: item.code
      });
      return acc;
    }, [] as IndikasjonTypeValg[]);
  }
}
