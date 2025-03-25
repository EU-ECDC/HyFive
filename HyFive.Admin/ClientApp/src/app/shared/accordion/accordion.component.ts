import { Component, Input, Output, TemplateRef } from '@angular/core';
import { faAngleUp } from '@fortawesome/free-solid-svg-icons';
import { EventEmitter } from '@angular/core';

/*
   Eksempelbruk

    <app-accordion
      [items]="mineItems"
      [headerTemplate]="header"
      [contentTemplate]="content">
    </app-accordion>
    <ng-template #header let-item="item">
     {{item.navn}}
    </ng-template>
    <ng-template #content let-item="item">
     {{item.nummer}}
    </ng-template>

 */

@Component({
  selector: 'app-accordion',
  templateUrl: './accordion.component.html',
  styles: [":host {width: 100%;}"]
})


export class AccordionComponent {

  faAngleUp = faAngleUp;

  @Input() items: any[] = [];
  @Input() headerTemplate!: TemplateRef<any>;
  @Input() contentTemplate!: TemplateRef<any>;

  @Input() panelChange: Function = (item) => { };
  @Input() identifyItem: Function = (item: any): string => `Item-${item.id}`;

  @Output() shownEvent = new EventEmitter<any>();

  constructor() { }

  shown(item) {
    this.shownEvent.emit(item);
  }
}
