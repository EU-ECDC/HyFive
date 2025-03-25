import * as dayjs from "dayjs";

export class DatoHjelper{
  static leggTilTimerOgMinutter (dato: Date, timer:number, minutter:number) : Date{
    var dayjsobject = dayjs(dato);
    dayjsobject = dayjsobject.hour(timer)
    dayjsobject = dayjsobject.minute(minutter)
    var obj =  dayjsobject.toDate();
    return obj;
  }
}
