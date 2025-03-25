import { Component, OnInit } from '@angular/core';
import { FhiTreeViewNavigationItem } from '@folkehelseinstituttet/angular-components';
import { UrlPaths } from '../../_felles/konstanter/url-paths';

@Component({
  selector: 'app-rapporter',
  templateUrl: './rapporter.component.html'
})

export class RapporterComponent implements OnInit {

  treeNavItems: FhiTreeViewNavigationItem[] = [];

  constructor() { }

  ngOnInit() {
    this.treeNavItems = this.getTreeviewNavigationItems();
  }

  private getTreeviewNavigationItems(): FhiTreeViewNavigationItem[] {
    return [
      // {
      //   name: 'Fire indikasjoner',
      //   children: [
      //     {
      //       name: 'Etterlevelse',
      //       routerLink: UrlPaths.fireindikasjonerEtterlevelse
      //     }
      //   ]
      // },
      {
        name: 'Predefinerte (PDF)',
        isExpanded: true,
        children: [
          {
            name: 'Etterlevelse fire indikasjoner',
            routerLink: UrlPaths.fireindikasjonerEtterlevelsePdf
          },
          {
            name: 'Etterlevelse håndsmykker',
            routerLink: UrlPaths.handsmykkerEtterlevelsePdf
          }
        ]
      },
      {
        name: 'Nedlastinger (Excel)',
        routerLink: UrlPaths.nedlastingExcel
      }
    ];
  }

}
