import {Department} from '../api/Department';

export interface DepartmentSelection {
    department: Department;
    isSelected: boolean;
    isAlreadyAtUnit: boolean;
}
