import { Role } from "./Role";

export interface UpdateDepartmentRequest
{
    id: number;
    facilityId: number;
    name: string;
    departmentTypeId: number;
    roles: Role[];
}