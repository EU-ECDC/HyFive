import * as dayjs from "dayjs";

export class DateHelper{
  static addHoursAndMinutes (date: Date, hours:number, minutes:number) : Date{
    var dayjsobject = dayjs(date);
    dayjsobject = dayjsobject.hour(hours)
    dayjsobject = dayjsobject.minute(minutes)
    var obj =  dayjsobject.toDate();
    return obj;
  }
}
