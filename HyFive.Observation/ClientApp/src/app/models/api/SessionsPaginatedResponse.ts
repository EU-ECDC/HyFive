import { SessionReport } from "./SessionReport";

export interface SessionsPaginatedResponse {
    totalPages: number;
    sessionReports: SessionReport[];

}