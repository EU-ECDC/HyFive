import { Subject, Observable } from 'rxjs';
import { UrlService } from './url.service';

export class UrlParam {
  constructor(
    private readonly name: string,
    private readonly urlService: UrlService
  ) {
    this.urlService.registerNewParameter(this.name);
  }

  private readonly paramSubject = new Subject<string>();

  get observable$(): Observable<string> {
    return this.paramSubject.asObservable();
  }

  get value(): string {
    return this.urlService.allUrlParameters[this.name];
  }

  set value(value: string) {
    if (value.length === 0) {
      this.reset();
    } else {
      this.urlService.updateUrlTree({
        [this.name]: value
      });
      this.paramSubject.next(value);
    }
  }

  unregisterParameter(): void {
    const array = this.urlService.registeredUrlParameters;
    for (let i = 0; i < array.length; i++) {
      if (array[i] === this.name) {
        array.splice(i, 1);
      }
    }
  }

  private reset(): void {
    delete this.urlService.allUrlParameters[this.name];
  }

}
