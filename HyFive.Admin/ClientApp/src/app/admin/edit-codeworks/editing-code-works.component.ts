import { Component, ComponentFactoryResolver, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { CodeworksSidemenuModel } from '../../models/code-work/codework-sidemenu.model';
import { EditingActivityTypeComponent } from './editing-of-activitytype/editing-activitytype.component';
import { EditingIndicationTypesComponent } from './editing-indication-types/editing-indication-types.component';
import { EditingFacilityTypesComponent } from './editing-of-facility-types/editing-of-facility-types.component';
import { EditingProtectiveEquipmentTypeComponent } from './editing-of-protective-equipment-types/editing-of-protectiveequipment-types.component';
import { EditingByHandjewelryTypeComponent } from './editing-HandjewelryType/editing-of-handjewelry-type.component';
import { EditingOfProtectiveEquipmentSettingTypesComponent } from './editing-of-protective-equipment-setting-types/editing-of-protective-equipment-setting-types.component';
import { EditingGlovesWithIndicationTypesComponent } from './editing-gloves-with-indicationtypes/editing-gloves-with-indicationtypes.component';
import { EditingGlovewithoutindicationtypesComponent } from './editing-of-gloveswithoutindicationtypes/editing-of-gloveswithoutindicationtypes.component';
import { EditingHandHygieneAfterGloveUseTypesComponent } from './editing-of-hand-hygiene-after-glove-usetypes/editing-of-hand-hygiene-after-glove-usetypes.component';
import { EditingOfDepartmentTypesComponent } from './editing-of-departmenttypes/editing-of-departmentstype.component';
import { EditingOfRolesComponent } from './editing-of-roles/editing-of-roles.component';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';

@Component({
  selector: 'app-editing-code-works',
  templateUrl: './editing-code-works.component.html'
})
export class EditingCodeworkComponent implements OnInit {

  @ViewChild('codeworkContainer', { static: true, read: ViewContainerRef }) codeworkContainer: ViewContainerRef;

  codeworks = [
    { name: 'Activity Types', isActive: false, component: EditingActivityTypeComponent },
    { name: 'Department Types', isActive: false, component: EditingOfDepartmentTypesComponent },
    // { name: 'Protective Equipment Types', isActive: false, component: EditingProtectiveEquipmentTypeComponent },
    //{ name: 'Protective Equipment Setting Types', isActive: false, component: EditingOfProtectiveEquipmentSettingTypesComponent },
    { name: 'Hand Jewelry Types', isActive: false, component: EditingByHandjewelryTypeComponent },
    { name: 'Glove with Indication - Types', isActive: false, component: EditingGlovesWithIndicationTypesComponent },
    { name: 'Glove without Indication - Types', isActive: false, component: EditingGlovewithoutindicationtypesComponent },
    { name: 'Hand Hygiene after wearing Gloves - Types', isActive: false, component: EditingHandHygieneAfterGloveUseTypesComponent },
    { name: 'Indication Types', isActive: false, component: EditingIndicationTypesComponent },
    { name: 'Facility Types', isActive: false, component: EditingFacilityTypesComponent },
    { name: 'Roles', isActive: false, component: EditingOfRolesComponent }
  ] as CodeworksSidemenuModel[];

  codeworkSelected = false;

  constructor(
    private readonly resolver: ComponentFactoryResolver, // NOSONAR
    private readonly translate: TranslateService) { }

  ngOnInit(): void {
      this.translate.get(this.codeworks.map(item => item.name)).pipe(take(1)).subscribe(_res => {
        this.codeworks = this.codeworks.map( codework => {
          return {
            ...codework,
            name: this.translate.instant(codework.name)
          }
        });
      });
  }

  openComponent(codework: CodeworksSidemenuModel) {
    this.codeworkSelected = true;
    this.codeworkContainer.clear();

    this.codeworks = this.codeworks.map(x => { x.isActive = (x.name === codework.name); return x; });

    const factory = this.resolver.resolveComponentFactory(codework.component);
    this.codeworkContainer.createComponent(factory);
  }
}
