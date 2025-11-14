import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndicationTypeConstants } from '../models/api/IndicationTypeConstants';
import { IndicationType } from '../models/api/IndicationType';
import { IndicationTypeSelection } from '../models/registration/indicationType-selection.model';
import { faFour, faOne, faThree, faTwo } from './customIcons';

export class IndicationTypeMapper {

  public static getIconTypeMap(): Map<IndicationTypeConstants, IconProp> {
    let iconmap = new Map<IndicationTypeConstants, IconProp>();
    iconmap.set(IndicationTypeConstants.BeforePatient, faOne);
    iconmap.set(IndicationTypeConstants.AsepticProcedures, faTwo);
    iconmap.set(IndicationTypeConstants.BodyFluid, faThree);
    iconmap.set(IndicationTypeConstants.AfterPatient, faFour);
    return iconmap;
  }

  public static getIndicationTypeOption(indicationTypes: IndicationType[], selectedIndicationTypes: IndicationType[]): IndicationTypeSelection[] {
    return indicationTypes.reduce((acc, item) => {
      acc.push({
        isSelected: selectedIndicationTypes.some(x => x.code === item.code),
        name: item.name,
        code: item.code
      });
      return acc;
    }, [] as IndicationTypeSelection[]);
  }
}
