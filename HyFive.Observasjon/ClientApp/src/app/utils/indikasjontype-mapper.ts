import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndicationTypeConstants } from '../models/api/IndicationTypeConstants';
import { IndicationType } from '../models/api/IndicationType';
import { IndicationTypeSelection } from '../models/registration/indicationType-selection.model';
import { faFour, faOne, faThree, faTwo } from './customIconer';

export class IndikasjonTypeMapper {

  public static getIconTypeMap(): Map<IndicationTypeConstants, IconProp> {
    var iconmap = new Map<IndicationTypeConstants, IconProp>();
    iconmap.set(IndicationTypeConstants.BeforePatient, faOne);
    iconmap.set(IndicationTypeConstants.AsepticProcedures, faTwo);
    iconmap.set(IndicationTypeConstants.BodyFluid, faThree);
    iconmap.set(IndicationTypeConstants.AfterPatient, faFour);
    return iconmap;
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
