import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { BeskyttelsesutstyrType } from '../models/api/BeskyttelsesutstyrType';

import { Beskyttelsesutstyr } from '../models/api/Beskyttelsesutstyr';
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

  public static getBeskyttelsesutstyrvalg(beskyttelsesutstyr: BeskyttelsesutstyrType[]): Beskyttelsesutstyr[] {
    return beskyttelsesutstyr.reduce((acc, item) => {
      acc.push({
        bleBenyttet: false,
        erIndikert: item.erIndikert,
        utstyrstype: item,
        bleBenyttetRiktig: false,
        feilbruktyper: [],
        kommentar: ""
      } as Beskyttelsesutstyr);
      return acc;
    }, []) as Beskyttelsesutstyr[];
  }
}
