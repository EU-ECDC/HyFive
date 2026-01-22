import { Injectable } from '@angular/core';
import { Subject, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BrowserViewportService {

  private innerWidth: number;
  private readonly isMobileSource = new Subject<boolean>();
  private readonly isDesktopSource = new Subject<boolean>();
  private readonly breakpointSource = new Subject<string>();
  private readonly windowInnerHeightSource = new BehaviorSubject<number>(globalThis.innerHeight);

  isMobile$ = this.isMobileSource.asObservable();
  isDesktop$ = this.isDesktopSource.asObservable();
  breakpoint$ = this.breakpointSource.asObservable();
  windowInnerHeight$ = this.windowInnerHeightSource.asObservable();

  isMobile: boolean;
  isDesktop: boolean;
  breakpoint: string;

  updateResponsiveProperties(): void {
    this.updateWindowInnerHeight();
    this.updateIsMobile();
    this.updateIsDesktop();
    this.updateBreakpoint();
  }

  updateWindowInnerHeight(): void {
    this.windowInnerHeightSource.next(globalThis.innerHeight);
  }

  updateIsMobile(): void {
    if (globalThis.innerWidth !== this.innerWidth) {
      this.innerWidth = globalThis.innerWidth;
      if (this.innerWidth < 1200) {
        this.isMobile = true;
        this.isMobileSource.next(true);
      } else {
        this.isMobile = false;
        this.isMobileSource.next(false);
      }
    }
  }

  updateIsDesktop(): void {
    if (globalThis.innerWidth !== this.innerWidth) {
      this.innerWidth = globalThis.innerWidth;
      if (this.innerWidth >= 1200) {
        this.isDesktop = true;
        this.isDesktopSource.next(true);
      } else {
        this.isDesktop = false;
        this.isDesktopSource.next(false);
      }
    }
  }

  updateBreakpoint(): void {
    if (globalThis.innerWidth !== this.innerWidth) {
      this.innerWidth = globalThis.innerWidth;

      if (this.innerWidth < 576) {
        this.breakpoint = 'xs';
        this.breakpointSource.next('xs');

      } else if (this.innerWidth < 768) {
        this.breakpoint = 'sm';
        this.breakpointSource.next('sm');

      } else if (this.innerWidth < 992) {
        this.breakpoint = 'md';
        this.breakpointSource.next('md');

      } else if (this.innerWidth < 1200) {
        this.breakpoint = 'lg';
        this.breakpointSource.next('lg');

      } else if (this.innerWidth < 1680) {
        this.breakpoint = 'xl';
        this.breakpointSource.next('xl');

      } else {
        this.breakpoint = 'xxl';
        this.breakpointSource.next('xxl');
      }
    }
  }

}
