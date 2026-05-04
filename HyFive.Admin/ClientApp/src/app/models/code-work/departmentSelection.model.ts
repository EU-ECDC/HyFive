import { OrganisationUnit } from '../api/OrganisationUnit';

export interface DepartmentSelection {
    department: OrganisationUnit;
    isSelected: boolean;
    isAlreadyAtUnit: boolean;
}
