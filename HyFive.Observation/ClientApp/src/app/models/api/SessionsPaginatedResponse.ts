import { SessionReport } from "./SessionReport";

export interface SessionsPaginatedResponse {
    totalCount: number;
    sessionReports: SessionReport[];

}