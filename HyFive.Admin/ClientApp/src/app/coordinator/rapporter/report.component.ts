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
      {
        name: 'five indications',
        children: [
          {
            name: 'Etterlevelse',
            routerLink: UrlPaths.fiveIndicationsCompliance
          }
        ]
      },
      {
        name: 'Predifined (PDF)',
        isExpanded: true,
        children: [
          {
            name: 'Compliance five indications',
            routerLink: UrlPaths.fiveIndicationsCompliancePdf
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
