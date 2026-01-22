import { IndicationType } from '../models/api/IndicationType';
import { IndicationTypeChoice } from '../models/five-indications/IndicationTypeChoice.model';

export class IndicationTypeMapper {

  public static getIndicationTypeSelection(indicationTypes: IndicationType[], selectedIndicationTypes: IndicationType[]): IndicationTypeChoice[] {
    let to = indicationTypes.reduce((acc, item) => {
      acc.push({
        isSelected: selectedIndicationTypes.some(x => x.code === item.code),
        name: item.name,
        code: item.code,
        number: item.number
      });
      return acc;
    }, [] as IndicationTypeChoice[]).sort((a, b) => { return Number.parseInt(a.number) > Number.parseInt(b.number) ? 1 : -1 }); return to;
  }
}
