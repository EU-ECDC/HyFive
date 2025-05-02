import { ActivityTypeNotExecuted } from '../models/api/ActivityTypeNotExecuted';

export class ActivityTypeNotExecutedMapper {

  public static getNameMap(): ActivityTypeNotExecuted[]{
    var navnMap : ActivityTypeNotExecuted[] = [
                { id: 1, name: "Ikke observert"},
                { id: 2, name: "Observert, ble benyttet"},
                { id: 3, name: "Observert, ikke ble benyttet"}
    ];
    return navnMap;
  }
}
