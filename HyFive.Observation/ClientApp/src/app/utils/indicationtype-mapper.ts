import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndicationTypeConstants } from '../models/api/IndicationTypeConstants';
import { IndicationType } from '../models/api/IndicationType';
import { IndicationTypeSelection } from '../models/registration/indicationType-selection.model';
import { faFive, faFour, faOne, faThree, faTwo } from './customIconer';

export class IndicationTypeMapper {

  public static getIconTypeMap(): Map<IndicationTypeConstants, IconProp> {
    var iconmap = new Map<IndicationTypeConstants, IconProp>();
    iconmap.set(IndicationTypeConstants.BeforePatient, faOne);
    iconmap.set(IndicationTypeConstants.AsepticProcedures, faTwo);
    iconmap.set(IndicationTypeConstants.BodyFluid, faThree);
    iconmap.set(IndicationTypeConstants.AfterPatient, faFour);
    iconmap.set(IndicationTypeConstants.PatientsSurroundings, faFive);
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
