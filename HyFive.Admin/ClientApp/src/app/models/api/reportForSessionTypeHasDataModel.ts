import { AuthorizedRole } from "src/app/_common/authorization/authorized-role";
import { SessionType } from "./SessionType";

export interface ReportForSessionTypeHasDataModel {
    sessionType: SessionType, 
    // facilityTypeIds: number[],
    facilityIds: number[], 
    // departmentTypeIds: number[], 
    departmentIds: number[],
    fromDate: Date, 
    toDate: Date, 
    roleId: AuthorizedRole 
    };