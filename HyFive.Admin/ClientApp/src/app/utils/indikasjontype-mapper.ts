import { IndikasjonType } from '../models/api/IndikasjonType';
import { IndikasjonTypeValg } from '../models/fireindikasjoner/indikasjontypevalg.model';

export class IndikasjonTypeMapper {

  public static getIndikasjonstypeValg(indikasjonstyper: IndikasjonType[], valgteIndikasjonTyper: IndikasjonType[]): IndikasjonTypeValg[] {
    var to = indikasjonstyper.reduce((acc, item) => {
      acc.push({
        erValgt: valgteIndikasjonTyper.some(x => x.code === item.code),
        name: item.name,
        code: item.code,
        nummer: item.nummer
      });
      return acc;
    }, [] as IndikasjonTypeValg[]).sort((a, b) => { return parseInt(a.nummer) > parseInt(b.nummer) ? 1 : -1 }); return to;
  }
}
