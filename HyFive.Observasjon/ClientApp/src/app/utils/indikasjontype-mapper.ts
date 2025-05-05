import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndicationTypeConstants } from '../models/api/IndicationTypeConstants';
import { IndicationType } from '../models/api/IndicationType';
import { IndicationTypeSelection } from '../models/registration/indicationType-selection.model';
import { faFour, faOne, faThree, faTwo } from './customIkoner';

export class IndikasjonTypeMapper {

  public static getIkontypeMap(): Map<IndicationTypeConstants, IconProp> {
    var ikonmap = new Map<IndicationTypeConstants, IconProp>();
    ikonmap.set(IndicationTypeConstants.BeforePatient, faOne);
    ikonmap.set(IndicationTypeConstants.AsepticProcedures, faTwo);
    ikonmap.set(IndicationTypeConstants.BodyFluid, faThree);
    ikonmap.set(IndicationTypeConstants.AfterPatient, faFour);
    return ikonmap;
  }

  public static getIndikasjonstypeValg(indicationTypes: IndicationType[], valgteIndikasjonTyper: IndicationType[]): IndicationTypeSelection[] {
    return indicationTypes.reduce((acc, item) => {
      acc.push({
        isSelected: valgteIndikasjonTyper.some(x => x.code === item.code),
        name: item.name,
        code: item.code
      });
      return acc;
    }, [] as IndicationTypeSelection[]);
  }
}
