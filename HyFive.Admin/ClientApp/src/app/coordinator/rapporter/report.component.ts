import { Component, OnInit } from '@angular/core';
import { FhiTreeViewNavigationItem } from '@folkehelseinstituttet/angular-components';
import { UrlPaths } from '../../_common/konstanter/url-paths';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html'
})

export class ReportComponent implements OnInit {

  treeNavItems: FhiTreeViewNavigationItem[] = [];

  constructor() { }

  ngOnInit() {
    this.treeNavItems = this.getTreeviewNavigationItems();
  }

  private getTreeviewNavigationItems(): FhiTreeViewNavigationItem[] {
    return [
      // {
      //   name: 'Four indications',
      //   children: [
      //     {
      //       name: 'Etterlevelse',
      //       routerLink: UrlPaths.fireindikasjonerEtterlevelse
      //     }
      //   ]
      // },
      {
        name: 'Predifined (PDF)',
        isExpanded: true,
        children: [
          {
            name: 'Compliance four indications',
            routerLink: UrlPaths.fourindicationsCompliancePdf
          },
          {
            name: 'Compliance handjewelry',
            routerLink: UrlPaths.handjewelryCompliancePdf
          }
        ]
      },
      {
        name: 'Downloads (Excel)',
        routerLink: UrlPaths.downloadExcel
      }
    ];
  }

}
