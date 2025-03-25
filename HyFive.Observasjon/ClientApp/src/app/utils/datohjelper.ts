export class DatoHjelper {
  //lager en lokal string av en dato
  static dateTimeSomLocaleStringReplacer(key, value) {
    if (this[key] instanceof Date) {
      let date = this[key];
      //string på format: dd-mm-YYT13:30:00 - +1 for riktig måned, legg til '0' og slice for å forsikre to tall i måned og dag.
      let localeDateTimeString = 
        date.getFullYear() + '-' + 
        DatoHjelper.padTilToSiffer(date.getMonth() + 1) + '-' + 
        DatoHjelper.padTilToSiffer(date.getDate()) + 'T' + 
        DatoHjelper.padTilToSiffer(date.getHours()) + ':' + 
        DatoHjelper.padTilToSiffer(date.getMinutes()) + ':' + 
        DatoHjelper.padTilToSiffer(date.getSeconds());
      return localeDateTimeString;
    }
    return value;
  }

  static padTilToSiffer(siffer: number) {
    return ('0' + (siffer)).slice(-2)
  }
}
