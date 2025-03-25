import {AfterViewInit, Component, Input, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {NgbModalConfig, NgbModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-hjelpetekst',
  templateUrl: './hjelpetekst.component.html'
})
export class HjelpetekstComponent implements AfterViewInit{

  static HjelpetekstPrefiks = "hjelpetekst_";
  @Input() tittel: string;
  @Input() bekreftKnappTekst: string = "Ikke vis igjen";
  @ViewChild('content') modalContent: TemplateRef<any>;

  harSettVerdi = "har_sett";

  constructor(config: NgbModalConfig, private modalService: NgbModal) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  ngAfterViewInit(): void {
    // if(localStorage.getItem(this.getHjelpetekstId()) !== this.harSettVerdi){
    //   const modalRef = this.modalService.open(this.modalContent, {
    //     ariaLabelledBy: 'modal-basic-title'
    //   });

    //   modalRef.result.then((r) => {
    //     if(r == this.harSettVerdi){
    //       localStorage.setItem(this.getHjelpetekstId(), this.harSettVerdi)
    //     }
    //   });
    // }
  }

  private getHjelpetekstId() {
    return HjelpetekstComponent.HjelpetekstPrefiks +this.camelize(this.tittel);
  }

  // Credits: vitaly-t (https://stackoverflow.com/a/57927739)
  private camelize(text: string) : string {
    text = text.replace(/[-_\s.]+(.)?/g, (_, c) => c ? c.toUpperCase() : '');
    return text.substr(0, 1).toLowerCase() + text.substr(1);
  }
}
