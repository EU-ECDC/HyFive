import { SessionType } from "../api/SessionType";

export interface Sesjonslistevisning1 {
  id: string;
  departmentName : string,
  startTime: Date,
  type: SessionType
}
