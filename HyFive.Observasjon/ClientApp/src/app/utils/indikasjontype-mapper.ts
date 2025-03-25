import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndikasjonTypeKonstanter } from '../models/api/IndikasjonTypeKonstanter';
import { IndikasjonType } from '../models/api/IndikasjonType';
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

  public static getIndikasjonstypeValg(indikasjonstyper: IndikasjonType[], valgteIndikasjonTyper: IndikasjonType[]): IndikasjonTypeValg[] {
    return indikasjonstyper.reduce((acc, item) => {
      acc.push({
        erValgt: valgteIndikasjonTyper.some(x => x.kode === item.kode),
        navn: item.navn,
        kode: item.kode
      });
      return acc;
    }, [] as IndikasjonTypeValg[]);
  }
}
