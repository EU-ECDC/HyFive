import { SessionType } from "../api/SessionType";

export interface SessionListView1 {
  id: string;
  departmentName : string,
  startTime: Date,
  type: SessionType
}
