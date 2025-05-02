import { Component, OnInit, ChangeDetectionStrategy, Input, Output, EventEmitter } from '@angular/core';
import { Activity } from '../../models/api/Activity';
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

  activity: Activity = null;

  @Input("hanskebrukSkalRegistreres") hanskebrukSkalRegistreres: boolean;
  @Input("deaktivert") deaktivert: boolean;
  @Input("erRegistrert") erRegistrert: boolean;

  @Output() aktivitetRegistertEvent = new EventEmitter<Activity>();

  constructor(private modalService: NgbModal, private aktivitetService: AktivitetService) {
    this.aktivitetService.getAktivitetTyper().subscribe((aktivitetTyper) => {
      this.activity = {
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
      this.aktivitetRegistertEvent.emit(this.activity);
    }
  }

  registrerAktivitet(bleHanskerBrukt: boolean) {
    this.activity.benyttetHanske = bleHanskerBrukt;
    this.aktivitetRegistertEvent.emit(this.activity);
  }
}
