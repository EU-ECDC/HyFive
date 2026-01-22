import { Department } from "../models/api/Department";
import { DepartmentType } from "../models/api/DepartmentType";
import { Facility } from "../models/api/Facility";

export class DownloadComplianceFacilitiesHelper {
    public static handleUniqueDepartments(facilities: Facility[]) {
        const allDepartments: Department[] = DownloadComplianceFacilitiesHelper.handleReducer(facilities);

        let uniqueDepartments: Department[] = Array.from(
                new Map(allDepartments.map(dep => [dep.id, dep])).values()
            );

        return uniqueDepartments
                    .toSorted((a,b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" }));
    }

    public static handleUniqueDepartmentTypes(facilities: Facility[]) {
        const allDepartments: Department[] = DownloadComplianceFacilitiesHelper.handleReducer(facilities);

        const uniqueDepartmentTypes: DepartmentType[] = Array.from(
        new Map(allDepartments.map(dep => [dep.departmentType.id, dep.departmentType])).values()
        );
        return uniqueDepartmentTypes;
    }

    private static handleReducer(facilities: Facility[]) {
        return facilities.reduce((all, inst) => {
            return all.concat(inst.departments);
        }, []);
    }
}