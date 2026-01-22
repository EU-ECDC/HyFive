import * as moment from "moment";

export class DateMomentHelper {
    public static dateTimeToDate (date, format) {
        return moment(date).format(format);
    }
}