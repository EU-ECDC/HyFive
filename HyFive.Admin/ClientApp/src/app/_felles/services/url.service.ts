import { Injectable } from '@angular/core';
import { UrlTree, UrlSegment, Params } from '@angular/router';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UrlService {

  private urlUpdatedTime: number;

  private allUrlParametersSubject = new Subject<Params>();
  private urlSegmentsSubject = new Subject<UrlSegment[]>();
  allUrlParameters$ = this.allUrlParametersSubject.asObservable();
  urlSegments$ = this.urlSegmentsSubject.asObservable();

  urlTree: UrlTree;
  urlSegments: UrlSegment[];
  urlFragment: string;
  urlWithoutParameters: string;
  allUrlParameters: Params;
  registeredUrlParameters: Array<string>;

  updateAfterNavigationEnd(urlTree: UrlTree): void {
    this.checkForDoubleUpdate();
    this.urlTree = urlTree;
    this.urlSegments = this.getSegments();
    this.urlFragment = this.getFragment();
    this.allUrlParameters = this.getQueryParams();

    if (this.urlSegments !== undefined) {
      this.urlSegmentsSubject.next(this.urlSegments);
    }
    if (this.allUrlParameters !== undefined) {
      this.allUrlParametersSubject.next(this.allUrlParameters);
    }
  }

  updateUrlTree(params: Params, fragment?: string): void {
    this.urlTree.queryParams = Object.assign(this.allUrlParameters, params);
    this.urlTree.fragment = fragment;
  }

  registerNewParameter(newParameterName: string): void {
    if (this.registeredUrlParameters === undefined) {
      this.registeredUrlParameters = [newParameterName];
    } else if (!this.registeredUrlParameters.includes(newParameterName)) {
      this.registeredUrlParameters.push(newParameterName);
    } else {
      const errorMsg = 'UrlService.registerNewParameter(): UrlParam with name "'
        + newParameterName + '" cannot be registered more than once!';
      throw new Error(errorMsg);
    }
  }

  private getSegments(): Array<UrlSegment> {
    if (this.urlTree.root.numberOfChildren !== 0) {
      return this.urlTree.root.children.primary.segments;
    } else {
      return undefined;
    }
  }

  private getFragment(): any {
    if (this.urlTree.fragment) {
      return this.urlTree.fragment;
    } else {
      return undefined;
    }
  }

  private getQueryParams(): Params {
    return this.urlTree.queryParams;
  }

  private checkForDoubleUpdate(): void {
    const timeDifference = Date.now() - this.urlUpdatedTime;
    const errorMsg = 'UrlService.checkForDoubleUpdate(): '
      + 'THE TIME BETWEEN TWO URL UPDATES IS < 75 MS!\n'
      + 'This may indicate that a double update has been introduced URL!\n'
      + 'Difference [ms]: ' + timeDifference;

    if (timeDifference < 75) {
      console.error(errorMsg);
    }
    this.urlUpdatedTime = Date.now();
  }

}
