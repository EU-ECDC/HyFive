import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AktivitetType} from "../../../../models/api/AktivitetType";
import {AktivitettypeService} from "../../../../services/data/aktivitettype.service";

@Component({
  selector: 'app-aktivitet-valg-dropdown',
  templateUrl: './aktivitet-valg-dropdown.component.html'
})
export class AktivitetValgDropdownComponent implements OnInit{

  @Input('aktivitetTypeId') aktivitetTypeId: number;
  @Output('aktivitetTypeValgt') aktivitetTypeValgt: EventEmitter<AktivitetType> = new EventEmitter<AktivitetType>();
  valgtAktivitetType: AktivitetType;
  aktivitetTyper: AktivitetType[] = [];

  valgtAktivitetTypeId: string;

  constructor(private aktivitettypeService: AktivitettypeService){}

  ngOnInit() {
    this.aktivitettypeService.hentAktivitettyper().subscribe(
      (aktivitetTyper) => {
      this.aktivitetTyper = aktivitetTyper;
      this.valgtAktivitetTypeId = this.aktivitetTypeId+'';
      this.velgAktivitetType();
      }
    );
  }

  valgtAktivitetTypeEndret() {
    this.velgAktivitetType();
    this.aktivitetTypeValgt.emit(this.valgtAktivitetType);
  }

  velgAktivitetType() {
    if (this.aktivitetTyper){
      this.valgtAktivitetType = this.aktivitetTyper[this.aktivitetTyper.map(r => r.id).indexOf(parseInt(this.valgtAktivitetTypeId))];
    }
  }
}
