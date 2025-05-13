import { Component, ComponentFactoryResolver, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { CodeworksSidemenuModel } from '../../models/code-work/codework-sidemenu.model';
import { EditingActivityTypeComponent } from './editing-of-activitytype/editing-activitytype.component';
import { EditingIndicationTypesComponent } from './editing-indication-types/editing-indication-types.component';
import { EditingInstitutionTypesComponent } from './editing-of-institution-types/editing-of-institution-types.component';
import { EditingProtectiveEquipmentTypeComponent } from './redigering-av-beskyttelsesutstyrtyper/editing-of-protectiveequipment-types.component';
import { EditingByHandjewelryTypeComponent } from './editing-HandjewelryType/editing-of-handjewelry-type.component';
import { EditingOfProtectiveEquipmentSettingTypesComponent } from './editing-of-protective-equipment-setting-types/editing-of-protective-equipment-setting-types.component';
import { EditingGlovesWithIndicationTypesComponent } from './editing-gloves-with-indicationtypes/editing-gloves-with-indicationtypes.component';
import { EditingGlovewithoutindicationtypesComponent } from './editing-of-gloveswithoutindicationtypes/editing-of-gloveswithoutindicationtypes.component';
import { EditingHandHygieneAfterGloveUseTypesComponent } from './editing-of-hand-hygiene-after-glove-usetypes/editing-of-hand-hygiene-after-glove-usetypes.component';
import { EditingOfDepartmentTypesComponent } from './editing-of-departmenttypes/editing-of-departmentstype.component';
import { EditingRegionComponent } from './editing-region.component/editing-region.component';
import { EditingOfRolesComponent } from './editing-of-roles/editing-of-roles.component';

@Component({
  selector: 'app-editing-code-works',
  templateUrl: './editing-code-works.component.html'
})
export class EditingCodeworkComponent implements OnInit {

  @ViewChild('codeworkContainer', { static: true, read: ViewContainerRef }) codeworkContainer: ViewContainerRef;

  codework = [
    { name: 'Activitytypes', isActive: false, component: EditingActivityTypeComponent },
    { name: 'Departmenttypes', isActive: false, component: EditingOfDepartmentTypesComponent },
    { name: 'Protectiveequipmenttypes', isActive: false, component: EditingProtectiveEquipmentTypeComponent },
    { name: 'Protectiveequipmenttypes', isActive: false, component: EditingOfProtectiveEquipmentSettingTypesComponent },
    { name: 'handjewelrytypes', isActive: false, component: EditingByHandjewelryTypeComponent },
    { name: '\'Glove with indication\'-types', isActive: false, component: EditingGlovesWithIndicationTypesComponent },
    { name: '\'Glove without indication\'-types', isActive: false, component: EditingGlovewithoutindicationtypesComponent },
    { name: '\'Hand hygiene after wearing gloves\'-types', isActive: false, component: EditingHandHygieneAfterGloveUseTypesComponent },
    { name: 'Indicationtypes', isActive: false, component: EditingIndicationTypesComponent },
    { name: 'Institutiontypes', isActive: false, component: EditingInstitutionTypesComponent },
    { name: 'Region', isActive: false, component: EditingRegionComponent },
    { name: 'Roles', isActive: false, component: EditingOfRolesComponent }
  ] as CodeworksSidemenuModel[];

  codeworkSelected = false;

  constructor(
    private resolver: ComponentFactoryResolver) { }

  ngOnInit(): void {
  }

  openComponent(codework: CodeworksSidemenuModel) {
    this.codeworkSelected = true;
    this.codeworkContainer.clear();

    this.codework = this.codework.map(x => { x.isActive = (x.name === codework.name) ? true : false; return x; });

    const factory = this.resolver.resolveComponentFactory(codework.component);
    const componentRef = this.codeworkContainer.createComponent(factory);
  }
}
