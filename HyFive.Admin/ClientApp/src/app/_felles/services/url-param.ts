import { Subject, Observable } from 'rxjs';
import { UrlService } from './url.service';

export class UrlParam {
  constructor(
    private name: string,
    private urlService: UrlService
  ) {
    this.urlService.registrerNyParameter(this.name);
  }

  private paramSubject = new Subject<string>();

  get observable$(): Observable<string> {
    return this.paramSubject.asObservable();
  }

  get verdi(): string {
    return this.urlService.alleUrlParametere[this.name];
  }

  set verdi(verdi: string) {
    if (verdi.length === 0) {
      this.nullstill();
    } else {
      this.urlService.updateUrlTree({
        [this.name]: verdi
      });
      this.paramSubject.next(verdi);
    }
  }

  avregistrerParameter(): void {
    const array = this.urlService.registrerteUrlParametere;
    for (let i = 0; i < array.length; i++) {
      if (array[i] === this.name) {
        array.splice(i, 1);
      }
    }
  }

  private nullstill(): void {
    delete this.urlService.alleUrlParametere[this.name];
  }

}
