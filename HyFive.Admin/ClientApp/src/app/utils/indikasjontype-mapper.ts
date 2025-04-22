import { IndicationType } from '../models/api/IndicationType';
import { IndikasjonTypeValg } from '../models/fireindikasjoner/indikasjontypevalg.model';

export class IndikasjonTypeMapper {

  public static getIndikasjonstypeValg(indikasjonstyper: IndicationType[], valgteIndikasjonTyper: IndicationType[]): IndikasjonTypeValg[] {
    var to = indikasjonstyper.reduce((acc, item) => {
      acc.push({
        erValgt: valgteIndikasjonTyper.some(x => x.code === item.code),
        name: item.name,
        code: item.code,
        nummer: item.number
      });
      return acc;
    }, [] as IndikasjonTypeValg[]).sort((a, b) => { return parseInt(a.nummer) > parseInt(b.nummer) ? 1 : -1 }); return to;
  }
}
