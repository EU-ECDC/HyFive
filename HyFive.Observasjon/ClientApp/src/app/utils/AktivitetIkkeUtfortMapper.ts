import { AktivitetTypeIkkeUtfort } from '../models/api/AktivitetTypeIkkeUtfort';

export class AktivitetIkkeUtfortMapper {

  public static getNavnMap(): AktivitetTypeIkkeUtfort[]{
    var navnMap : AktivitetTypeIkkeUtfort[] = [
                { id: 1, navn: "Ikke observert"},
                { id: 2, navn: "Observert, ble benyttet"},
                { id: 3, navn: "Observert, ikke ble benyttet"}
    ];
    return navnMap;
  }
}
