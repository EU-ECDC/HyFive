import { ActivityTypeNotExecuted } from '../models/api/ActivityTypeNotExecuted';

export class ActivityTypeNotExecutedMapper {

  public static getNameMap(): ActivityTypeNotExecuted[]{
    var navnMap : ActivityTypeNotExecuted[] = [
                { id: 1, name: "Not observed"},
                { id: 2, name: "Observed, was used"},
                { id: 3, name: "Observed, was not used"}
    ];
    return navnMap;
  }
}
