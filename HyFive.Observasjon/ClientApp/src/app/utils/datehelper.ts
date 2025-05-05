export class DateHelper {
//create a local string of a date
static dateTimeSomLocaleStringReplacer(key, value) {
    if (this[key] instanceof Date) {
      let date = this[key];
//string in format: dd-mm-YYT13:30:00 - +1 for correct month, add '0' and slice to ensure two numbers in month and day.
let localeDateTimeString = 
        date.getFullYear() + '-' + 
        DateHelper.padToTwoDigits(date.getMonth() + 1) + '-' + 
        DateHelper.padToTwoDigits(date.getDate()) + 'T' + 
        DateHelper.padToTwoDigits(date.getHours()) + ':' + 
        DateHelper.padToTwoDigits(date.getMinutes()) + ':' + 
        DateHelper.padToTwoDigits(date.getSeconds());
      return localeDateTimeString;
    }
    return value;
  }

  static padToTwoDigits(digit: number) {
    return ('0' + (digit)).slice(-2)
  }
}
