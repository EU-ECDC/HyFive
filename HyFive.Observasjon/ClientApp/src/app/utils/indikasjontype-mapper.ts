import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndicationTypeConstants } from '../models/api/IndicationTypeConstants';
import { IndicationType } from '../models/api/IndicationType';
import { IndikasjonTypeValg } from '../models/registrering/indikasjontypevalg.model';
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

  public static getIndikasjonstypeValg(indicationTypes: IndicationType[], valgteIndikasjonTyper: IndicationType[]): IndikasjonTypeValg[] {
    return indicationTypes.reduce((acc, item) => {
      acc.push({
        isSelected: valgteIndikasjonTyper.some(x => x.code === item.code),
        name: item.name,
        code: item.code
      });
      return acc;
    }, [] as IndikasjonTypeValg[]);
  }
}
