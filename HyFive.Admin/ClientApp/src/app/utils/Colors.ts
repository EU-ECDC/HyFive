export class Colors {

  public static getFhiColorHexFromText(text: string) : string {

    let fhiColors = [
      "#BFD8DC",
      "#E6E1D9",
      "#CCCCCC",
      "#E0D9A9",
      "#C8CBDB",
      "#DCBFCF",
      "#C0C7B8",
    ]

    fhiColors = fhiColors.concat(fhiColors)
    let charCodeSum = text.split('').map((c) => { return c.charCodeAt(0)}).reduce((a,b) => a+b,0)
    let colorIndex =  charCodeSum % (fhiColors.length-1) !== undefined
      ? charCodeSum % (fhiColors.length-1)
      : 0;
    return fhiColors[colorIndex];
  }
}
