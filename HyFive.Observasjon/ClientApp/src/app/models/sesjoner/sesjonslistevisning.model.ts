import { SesjonType } from "../api/SesjonType";

export interface Sesjonslistevisning1 {
  id: string;
  avdelingsnavn : string,
  starttidspunkt: Date,
  type: SesjonType
}
