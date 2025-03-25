import { Component, OnInit, ChangeDetectionStrategy, Input, Output, EventEmitter } from '@angular/core';
import { Aktivitet } from '../../models/api/Aktivitet';
import { faHandsWash, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AktivitetService } from '../../services/data/aktivitet.service';
import { AktivitetTypeKonstanter } from '../../models/api/AktivitetTypeKonstanter';

@Component({
  selector: 'app-missed-opportunity',
  templateUrl: './missed-opportunity.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MissedOpportunityComponent implements OnInit {

  faHandsWash = faHandsWash;
  faTimesCircle = faTimesCircle;
  ikonklasse = '';

  aktivitet: Aktivitet = null;

  @Input("hanskebrukSkalRegistreres") hanskebrukSkalRegistreres: boolean;
  @Input("deaktivert") deaktivert: boolean;
  @Input("erRegistrert") erRegistrert: boolean;

  @Output() aktivitetRegistertEvent = new EventEmitter<Aktivitet>();

  constructor(private modalService: NgbModal, private aktivitetService: AktivitetService) {
    this.aktivitetService.getAktivitetTyper().subscribe((aktivitetTyper) => {
      this.aktivitet = {
        aktivitetType: aktivitetTyper.find(x => x.kode === AktivitetTypeKonstanter.IkkeUtfort),
        tidtakingBleUtfort: false,
        benyttetHanske: null
      };
    });
  }

  ngOnInit(): void {
  }

  registrerIkkeUtfortAktivitet(modalName): void {
    if (this.hanskebrukSkalRegistreres) {
      this.modalService.open(modalName, { windowClass: 'hh-modal' });
    } else {
      this.aktivitetRegistertEvent.emit(this.aktivitet);
    }
  }

  registrerAktivitet(bleHanskerBrukt: boolean) {
    this.aktivitet.benyttetHanske = bleHanskerBrukt;
    this.aktivitetRegistertEvent.emit(this.aktivitet);
  }
}
