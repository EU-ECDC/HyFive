import {IconProp} from '@fortawesome/fontawesome-svg-core';
import { faHandSpock } from '@fortawesome/free-regular-svg-icons';
import {BeskyttelsesutstyrsettingTypeKonstanter} from '../models/api/BeskyttelsesutstyrsettingTypeKonstanter';
import { faDroplet, faFaceSideCough, faBacteria } from './customIkoner';

export class BeskyttelsesutstyrsettingMapper {
  public static getIkontypeMap() : Map<string, IconProp>{
    var ikonmap = new Map<string, IconProp>();
    ikonmap.set(BeskyttelsesutstyrsettingTypeKonstanter.BasaleSmittevernrutiner, faBacteria);
    ikonmap.set(BeskyttelsesutstyrsettingTypeKonstanter.Kontaktsmitte, faHandSpock);
    ikonmap.set(BeskyttelsesutstyrsettingTypeKonstanter.Drapesmitte, faDroplet);
    ikonmap.set(BeskyttelsesutstyrsettingTypeKonstanter.Luftsmitte, faFaceSideCough);
    return ikonmap;
  }
}
