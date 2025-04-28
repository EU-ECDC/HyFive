import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Klinikk } from '../../../models/api/Klinikk';
import { InstitutionService } from '../../../services/data/institution.service';
import { DepartmentService } from '../../../services/data/department.service';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { Avdelingsvalg } from '../../../models/code-work/avdelingsvalg.model';
import { KlinikkService } from '../../../services/data/klinikk.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-rediger-en-klinikk',
  templateUrl: './rediger-en-klinikk.component.html'
})
export class RedigerEnKlinikkComponent implements OnInit, OnDestroy {

  @Input() klinikk: Klinikk;
  klinikkKopi: Klinikk;
  avdelingsvalg: Avdelingsvalg[];
  UrlPaths = UrlPaths;

  klinikkerListe: Klinikk[] = [];

  fawarningicon = faExclamationTriangle;

  constructor(
    private institutionService: InstitutionService,
    private departmentService: DepartmentService,
    private toastrService: ToastrService,
    private klinikkService: KlinikkService) { }

  ngOnInit(): void {
    if (this.klinikk) {
      this.klinikkKopi = JSON.parse(JSON.stringify(this.klinikk));
      this.lastAvdelinger();
    }
    else {
      this.toastrService.error('Departmentikke lastet', 'Technical error', { disableTimeOut: true});
    }
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastAvdelinger() {

    this.klinikkService.hentKlinikkerForInstitusjon(this.klinikkKopi.institutionId).subscribe((institusjon) => {
      this.klinikkerListe = institusjon;

      this.institutionService.getDepartments(this.klinikkKopi.institutionId).subscribe(
        (departments) => {
          this.avdelingsvalg = departments.map(a => (
            {
              avdeling: a, erValgt: this.klinikkKopi.departments.map(k => k.id).indexOf(a.id) !== -1,
              erAlleredePaKlinikk: this.klinikkerListe.some(k => k.departments.some(av => av.id === a.id) && k.id !== this.klinikkKopi.id)
            }));
        },
        (err) => this.toastrService.error(`Could not load klinikker: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
      );

     });

    
  }

  kanIkkeLagreKlinikk(): boolean {
    return this.kanLagreKlinikk() === false;
  }

  kanLagreKlinikk(): boolean {
    return this.klinikkKopi.institutionId > 0
      && this.klinikkKopi.name?.length > 0
      && this.avdelingsvalg?.filter(r => r.erValgt)?.length > 0;
  }

  lagreKlinikk() {
    this.klinikkKopi.departments = this.avdelingsvalg.filter(m => m.erValgt).map(r => r.avdeling);
    this.klinikkService.oppdaterKlinikk(this.klinikkKopi).subscribe(
      (k) => {
        // Må replace verdier på original-objektet for å støtte oppdatering av liste når en navigerer tilbake til klinikk-oversikt
        this.klinikk.name = k.name;
        this.klinikk.institutionId = k.institutionId;
        this.klinikk.departments = k.departments;
        this.toastrService.success('Klinikk oppdatert');
      },
      (err) => this.toastrService.error(`Technical error ved oppdatering: ${err?.message ? err.message : err}`, '', { disableTimeOut: true})
    );
  }
}
