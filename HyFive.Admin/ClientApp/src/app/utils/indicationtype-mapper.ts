import { IndicationType } from '../models/api/IndicationType';
import { IndicationTypeChoice } from '../models/four-indications/IndicationTypeChoice.model';

export class IndicationTypeMapper {

  public static getIndicationTypeSelection(indicationTypes: IndicationType[], selectedIndicationTypes: IndicationType[]): IndicationTypeChoice[] {
    var to = indicationTypes.reduce((acc, item) => {
      acc.push({
        isSelected: selectedIndicationTypes.some(x => x.code === item.code),
        name: item.name,
        code: item.code,
        number: item.number
      });
      return acc;
    }, [] as IndicationTypeChoice[]).sort((a, b) => { return parseInt(a.number) > parseInt(b.number) ? 1 : -1 }); return to;
  }
}
