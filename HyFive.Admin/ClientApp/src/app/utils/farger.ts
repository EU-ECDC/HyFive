export class Farger {

  public static hentFhiFargeHexFraTekst(tekst: string) : string {

    var fhiFarger = [
      "#BFD8DC",
      "#E6E1D9",
      "#CCCCCC",
      "#E0D9A9",
      "#C8CBDB",
      "#DCBFCF",
      "#C0C7B8",
    ]

    fhiFarger = fhiFarger.concat(fhiFarger)
    var charkodeSum = tekst.split('').map((c) => { return c.charCodeAt(0)}).reduce((a,b) => a+b,0)
    var fargeindeks =  charkodeSum % (fhiFarger.length-1) !== undefined
      ? charkodeSum % (fhiFarger.length-1)
      : 0;
    return fhiFarger[fargeindeks];
  }
}
