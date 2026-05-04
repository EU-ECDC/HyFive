import { OrganisationUnit } from "../models/api/OrganisationUnit";
import { OrganisationUnitType } from "../models/api/OrganisationUnitType";

export class DownloadComplianceFacilitiesHelper {
    public static handleUniqueDepartments(facilities: OrganisationUnit[]) {
        const allDepartments: OrganisationUnit[] = DownloadComplianceFacilitiesHelper.handleReducer(facilities);

        let uniqueDepartments: OrganisationUnit[] = Array.from(
                new Map(allDepartments.map(dep => [dep.id, dep])).values()
            );

        return uniqueDepartments
                    .toSorted((a,b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" }));
    }

    public static handleUniqueDepartmentTypes(facilities: OrganisationUnit[]) {
        const allDepartments: OrganisationUnit[] = DownloadComplianceFacilitiesHelper.handleReducer(facilities);

        const uniqueDepartmentTypes: OrganisationUnitType[] = Array.from(
        new Map(allDepartments.map(dep => [dep.type.id, dep.type])).values()
        );
        return uniqueDepartmentTypes;
    }

    private static handleReducer(facilities: OrganisationUnit[]) {
        return facilities.reduce((all, inst) => {
            return all.concat(inst.children);
        }, []);
    }
}