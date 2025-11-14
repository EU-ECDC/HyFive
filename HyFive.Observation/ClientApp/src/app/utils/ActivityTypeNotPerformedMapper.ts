import { ActivityTypeNotPerformed } from '../models/api/ActivityTypeNotPerformed';

export class ActivityTypeNotPerformedMapper {

  public static getNameMap(): ActivityTypeNotPerformed[]{
    let nameMap : ActivityTypeNotPerformed[] = [
                { id: 1, name: "Not observed"},
                { id: 2, name: "Observed, was used"},
                { id: 3, name: "Observed, was not used"}
    ];
    return nameMap;
  }
}
