import { SesjonType } from '../models/api/SesjonType';

export class SesjonstypeRapportUrlMapper {

  public static getRapportUrlMap() : Map<SesjonType, string> {
    let rapportUrlMap = new Map<SesjonType, string>();
    rapportUrlMap.set(SesjonType.Handsmykker,        '/api/v1/rapport/avdeling/handsmykker/excel');
    rapportUrlMap.set(SesjonType.FireIndikasjoner,   '/api/v1/rapport/avdeling/fireindikasjoner/excel');
    rapportUrlMap.set(SesjonType.Hansker,            '/api/v1/rapport/avdeling/hansker/excel');
    rapportUrlMap.set(SesjonType.Beskyttelsesutstyr, '/api/v1/rapport/avdeling/beskyttelsesutstyr/excel');
    return rapportUrlMap;

  }
}
