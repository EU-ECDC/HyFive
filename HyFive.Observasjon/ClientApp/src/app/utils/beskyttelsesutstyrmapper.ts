import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { ProtectiveEquipmentType } from '../models/api/ProtectiveEquipmentType';

import { ProtectiveEquipment } from '../models/api/ProtectiveEquipment';
import { BeskyttelsesutstyrTypeKonstanter } from '../models/api/BeskyttelsesutstyrTypeKonstanter';
import { faHeadSideMask } from '@fortawesome/free-solid-svg-icons';
import { faHandPaper } from '@fortawesome/free-regular-svg-icons';
import { faAnderettsvern, faHette, faPlastforkle, faSmittefrakk, faStellefrakk, faVernebriller } from './customIkoner';

export class BeskyttelsesutstyrMapper {

  public static getIkontypeMap(): Map<string, IconProp> {
    var ikonmap = new Map<string, IconProp>();
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Hansker, faHandPaper);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Smittefrakk, faSmittefrakk);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Munnbind, faHeadSideMask);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse, faVernebriller);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Andedrettsvern, faAnderettsvern);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Hette, faHette);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Plastforkle, faPlastforkle);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Stellefrakk, faStellefrakk);
    return ikonmap;
  }

  public static getBeskyttelsesutstyrvalg(beskyttelsesutstyr: ProtectiveEquipmentType[]): ProtectiveEquipment[] {
    return beskyttelsesutstyr.reduce((acc, item) => {
      acc.push({
        wasUsed: false,
        isRequired: item.isRequired,
        equipmentType: item,
        wasUsedCorrectly: false,
        incorrectTypes: [],
        comment: ""
      } as ProtectiveEquipment);
      return acc;
    }, []) as ProtectiveEquipment[];
  }
}
