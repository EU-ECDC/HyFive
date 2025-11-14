import { Component, Input, TemplateRef, ViewChild} from '@angular/core';
import {NgbModalConfig, NgbModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-help-text',
  templateUrl: './help-text.component.html'
})
export class HelpTextComponent{

  static readonly HelpTextPrefix = "helptext_";
  @Input() title: string;
  @Input() confirmButtonText: string = "Not show again";
  @ViewChild('content') modalContent: TemplateRef<any>;

  hasSetValue = "has_set";

  constructor(config: NgbModalConfig, private readonly modalService: NgbModal) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  private getHelpTextId() {
    return HelpTextComponent.HelpTextPrefix +this.camelize(this.title);
  }

  // Credits: vitaly-t (https://stackoverflow.com/a/57927739)
  private camelize(text: string) : string {
    text = text.replace(/[-_\s.]+(.)?/g, (_, c) => c ? c.toUpperCase() : '');
    return text.substr(0, 1).toLowerCase() + text.substr(1);
  }
}
