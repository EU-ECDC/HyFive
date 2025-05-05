import { SessionType } from "../api/SessionType";

export interface Sesjonslistevisning1 {
  id: string;
  avdelingsnavn : string,
  startTime: Date,
  type: SessionType
}
