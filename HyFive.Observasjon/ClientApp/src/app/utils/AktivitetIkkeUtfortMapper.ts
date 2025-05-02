import { AktivitetTypeIkkeUtfort } from '../models/api/AktivitetTypeIkkeUtfort';

export class AktivitetIkkeUtfortMapper {

  public static getNavnMap(): AktivitetTypeIkkeUtfort[]{
    var navnMap : AktivitetTypeIkkeUtfort[] = [
                { id: 1, name: "Ikke observert"},
                { id: 2, name: "Observert, ble benyttet"},
                { id: 3, name: "Observert, ikke ble benyttet"}
    ];
    return navnMap;
  }
}
