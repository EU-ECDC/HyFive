import { SesjonType } from '../models/api/SesjonType';

export class SesjonTypeMapper {

  public static getNavnMap(): Map<SesjonType, string>{
    var navnMap = new Map<SesjonType, string>();
    navnMap.set(SesjonType.Handsmykker, 'Håndsmykker');
    navnMap.set(SesjonType.FireIndikasjoner, 'Fire indikasjoner');
    navnMap.set(SesjonType.Hansker, 'Hansker');
    navnMap.set(SesjonType.InnUt, 'Inn/Ut');
    navnMap.set(SesjonType.Beskyttelsesutstyr, 'Verneutstyr');
    return navnMap;
  }
}
