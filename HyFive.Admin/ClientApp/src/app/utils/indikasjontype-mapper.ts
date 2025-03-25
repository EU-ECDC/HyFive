import { IndikasjonType } from '../models/api/IndikasjonType';
import { IndikasjonTypeValg } from '../models/fireindikasjoner/indikasjontypevalg.model';

export class IndikasjonTypeMapper {

  public static getIndikasjonstypeValg(indikasjonstyper: IndikasjonType[], valgteIndikasjonTyper: IndikasjonType[]): IndikasjonTypeValg[] {
    var to = indikasjonstyper.reduce((acc, item) => {
      acc.push({
        erValgt: valgteIndikasjonTyper.some(x => x.kode === item.kode),
        navn: item.navn,
        kode: item.kode,
        nummer: item.nummer
      });
      return acc;
    }, [] as IndikasjonTypeValg[]).sort((a, b) => { return parseInt(a.nummer) > parseInt(b.nummer) ? 1 : -1 }); return to;
  }
}
