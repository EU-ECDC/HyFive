import { Component, ComponentFactoryResolver, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { KodeverkSidemenyModel } from '../../models/kodeverk/kodeverk-sidemeny.model';
import { RedigeringAvAktivitettypeComponent } from './redigering-av-aktivitettype/redigering-av-aktivitettype.component';
import { RedigeringAvIndikasjonstyperComponent } from './redigering-av-indikasjonstyper/redigering-av-indikasjonstyper.component';
import { RedigeringAvInstitusjonstyperComponent } from './redigering-av-institusjonstyper/redigering-av-institusjonstyper.component';
import { RedigeringAvBeskyttelsesutstyrtyperComponent } from './redigering-av-beskyttelsesutstyrtyper/redigering-av-beskyttelsesutstyrtyper.component';
import { RedigeringAvHandsmykketypeComponent } from './redigering-av-handsmykketype/redigering-av-handsmykketype.component';
import { RedigeringAvBeskyttelsesutstyrsettingtyperComponent } from './redigering-av-beskyttelsesutstyrsettingtyper/redigering-av-beskyttelsesutstyrsettingtyper.component';
import { RedigeringAvHanskemedindikasjontyperComponent } from './redigering-av-hanskemedindikasjontyper/redigering-av-hanskemedindikasjontyper.component';
import { RedigeringAvHanskeutenindikasjontyperComponent } from './redigering-av-hanskeutenindikasjontyper/redigering-av-hanskeutenindikasjontyper.component';
import { RedigeringAvHandhygieneetterhanskebruktyperComponent } from './redigering-av-handhygieneetterhanskebruktyper/redigering-av-handhygieneetterhanskebruktyper.component';
import { RedigeringAvAvdelingstyperComponent } from './redigering-av-avdelingstyper/redigering-av-avdelingstyper.component';
import { RedigeringAvRegionComponent } from './redigering-av-region/redigering-av-region.component';
import { RedigeringAvRollerComponent } from './redigering-av-roller/redigering-av-roller.component';

@Component({
  selector: 'app-redigering-av-kodeverk',
  templateUrl: './redigering-av-kodeverk.component.html'
})
export class RedigeringAvKodeverkComponent implements OnInit {

  @ViewChild('kodeverkContainer', { static: true, read: ViewContainerRef }) kodeverkContainer: ViewContainerRef;

  kodeverk = [
    { navn: 'Aktivitetstyper', erAktiv: false, component: RedigeringAvAktivitettypeComponent },
    { navn: 'Avdelingstyper', erAktiv: false, component: RedigeringAvAvdelingstyperComponent },
    { navn: 'Beskyttelsesutstyrtyper', erAktiv: false, component: RedigeringAvBeskyttelsesutstyrtyperComponent },
    { navn: 'Beskyttelsesustyrsettingtyper', erAktiv: false, component: RedigeringAvBeskyttelsesutstyrsettingtyperComponent },
    { navn: 'Handsmykketyper', erAktiv: false, component: RedigeringAvHandsmykketypeComponent },
    { navn: '\'Hanske ved indikasjon\'-typer', erAktiv: false, component: RedigeringAvHanskemedindikasjontyperComponent },
    { navn: '\'Hanske uten indikasjon\'-typer', erAktiv: false, component: RedigeringAvHanskeutenindikasjontyperComponent },
    { navn: '\'Håndhygiene etter hanskebruk\'-typer', erAktiv: false, component: RedigeringAvHandhygieneetterhanskebruktyperComponent },
    { navn: 'Indikasjonstyper', erAktiv: false, component: RedigeringAvIndikasjonstyperComponent },
    { navn: 'Institusjonstyper', erAktiv: false, component: RedigeringAvInstitusjonstyperComponent },
    { navn: 'Region', erAktiv: false, component: RedigeringAvRegionComponent },
    { navn: 'Roller', erAktiv: false, component: RedigeringAvRollerComponent }
  ] as KodeverkSidemenyModel[];

  kodeverkValgt = false;

  constructor(
    private resolver: ComponentFactoryResolver) { }

  ngOnInit(): void {
  }

  openComponent(kodeverk: KodeverkSidemenyModel) {
    this.kodeverkValgt = true;
    this.kodeverkContainer.clear();

    this.kodeverk = this.kodeverk.map(x => { x.erAktiv = (x.navn === kodeverk.navn) ? true : false; return x; });

    const factory = this.resolver.resolveComponentFactory(kodeverk.component);
    const componentRef = this.kodeverkContainer.createComponent(factory);
  }
}
