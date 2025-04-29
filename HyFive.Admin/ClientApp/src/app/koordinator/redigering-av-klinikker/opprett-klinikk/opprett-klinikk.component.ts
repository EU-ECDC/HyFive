import { Component, EventEmitter, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { InstitutionService } from '../../../services/data/institution.service';
import { ToastrService } from 'ngx-toastr';
import { KlinikkService } from '../../../services/data/klinikk.service';
import { Klinikk } from '../../../models/api/Klinikk';
import { Avdelingsvalg } from '../../../models/code-work/avdelingsvalg.model';
import { DepartmentService } from '../../../services/data/department.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-opprett-klinikk',
  templateUrl: './opprett-klinikk.component.html'
})
export class OpprettKlinikkComponent implements OnInit, OnDestroy {

  nyKlinikk: Klinikk;
  avdelingsvalg: Avdelingsvalg[] = [];

  klinikkerListe: Klinikk[] = [];

  fawarningicon = faExclamationTriangle;

  @Input() institutionId: number;
  @Output() klinikkOpprettetEvent: EventEmitter<Klinikk> = new EventEmitter<Klinikk>();


  constructor(
    private institutionService: InstitutionService,
    private klinikkService: KlinikkService,
    private departmentService: DepartmentService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.nullstillSkjema();
    this.loadDepartments();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  opprettKlinikk() {
    this.nyKlinikk.departments = this.avdelingsvalg
      .filter(r => r.erValgt)
      .map((r) => ({ id: r.avdeling.id, departmentTypeId: 0, roles: null, institutionId: this.institutionId, name: null, departmentType: null }));

    this.klinikkService.opprettKlinikk(this.nyKlinikk).subscribe((klinikk) => {
      this.toastrService.success('Klinikk opprettet', `Klinikk med ID: ${klinikk.id} opprettet`);
      this.klinikkOpprettetEvent.emit(klinikk);

      this.loadDepartments();
    },
      (error) => this.toastrService.error(`An error occurred while creating klinikk. Feilmelding fra server: ${error?.message ? error.message : error}`, 'Error under opprettelse av klinikk', { disableTimeOut: true}),
      () => { this.nullstillSkjema(); }
    );
  }

  loadDepartments() {

    this.klinikkService.hentKlinikkerForInstitusjon(this.institutionId).subscribe((result: Klinikk[]) => {
      this.klinikkerListe = result;

      this.institutionService.getDepartments(this.institutionId).subscribe(
        (departments) => {
          this.avdelingsvalg = departments.map(a =>
          ({
            avdeling: a, erValgt: false,
            erAlleredePaKlinikk: this.klinikkerListe.some(k => k.departments.some(av => av.id === a.id))
          }));
        },
        (err) => this.toastrService.error(`Could not load klinikker: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
      );
    });
  }

  nullstillSkjema() {
    this.nyKlinikk = {
      id: 0,
      name: null,
      institutionId: this.institutionId,
      departments: []
    };
    for (const avdeling of this.avdelingsvalg) {
      avdeling.erValgt = false;
    }
  }

  kanIkkeOppretteKlinikk(): boolean {
    return this.kanOppretteKlinikk() === false;
  }

  kanOppretteKlinikk(): boolean {
    return this.nyKlinikk.institutionId > 0
      && this.avdelingsvalg?.filter(r => r.erValgt)?.length > 0
      && this.nyKlinikk.name?.length > 0;
  }

}
