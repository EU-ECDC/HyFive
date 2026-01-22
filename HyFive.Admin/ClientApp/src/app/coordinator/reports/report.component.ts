import { Component, OnInit } from '@angular/core';
import { FhiTreeViewNavigationItem } from '@folkehelseinstituttet/angular-components';
import { UrlPaths } from '../../_common/constants/url-paths';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html'
})

export class ReportComponent implements OnInit {

  treeNavItems: FhiTreeViewNavigationItem[] = [];

  constructor(private readonly translate: TranslateService) { }

  ngOnInit() {
    this.treeNavItems = this.getTreeviewNavigationItems();
    this.translate.get(this.treeNavItems.map(item => item.name)).pipe(take(1)).subscribe(() => {
      this.treeNavItems = this.treeNavItems.map(item => {
        return {
          ...item,
          name: this.translate.instant(item.name),
          children: item.children?.map(child => {
            return  {...child, name: this.translate.instant(child.name) }
        })
      }
    })
    });
  }

  private getTreeviewNavigationItems(): FhiTreeViewNavigationItem[] {
    return [
      {
        name: 'Five Indications',
        isExpanded: true,
        children: [
          {
            name: 'Compliance',
            routerLink: UrlPaths.fiveIndicationsCompliance
          }
        ]
      },
      {
        name: 'Predefined (PDF)',
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
