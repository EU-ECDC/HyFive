import {BeskyttelsesutstyrTypeKonstanter} from "../models/api/BeskyttelsesutstyrTypeKonstanter";
import {faUserTimes} from "@fortawesome/free-solid-svg-icons";
import {IconProp} from "@fortawesome/fontawesome-svg-core";
import { BeskyttelsesutstyrType } from '../models/api/BeskyttelsesutstyrType';
import {Beskyttelsesutstyr} from "../models/api/Beskyttelsesutstyr";

export class BeskyttelsesutstyrMapper {

  public static getIkontypeMap(): Map<string, IconProp> {
    var ikonmap = new Map<string, IconProp>();
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Hansker, faUserTimes);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Smittefrakk, faUserTimes);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Munnbind, faUserTimes);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Oyebeskyttelse, faUserTimes);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Andedrettsvern, faUserTimes);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Hette, faUserTimes);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Plastforkle, faUserTimes);
    ikonmap.set(BeskyttelsesutstyrTypeKonstanter.Stellefrakk, faUserTimes);
    return ikonmap;
  }

  public static getBeskyttelsesutstyrvalg(beskyttelsesutstyr: BeskyttelsesutstyrType[], valg: Beskyttelsesutstyr ): Beskyttelsesutstyr[] {
    return beskyttelsesutstyr.reduce((acc, item) => {
      acc.push({
        id: valg.id,
        bleBenyttet: false,
        erIndikert: valg.erIndikert,
        utstyrstype: item,
        bleBenyttetRiktig: false,
        feilbruktyper: [],
        kommentar: ""
      } as Beskyttelsesutstyr);
      return acc;
    }, []) as Beskyttelsesutstyr[];
  }
}
