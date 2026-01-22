import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class LanguageInterceptor implements HttpInterceptor {
  
  constructor(private readonly translate: TranslateService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Get current language
    const currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    // Clone the request and add the Accept-Language header
    const authReq = req.clone({
      headers: req.headers.set('Accept-Language', currentLang)
    });

    // Pass the cloned request to the next handler
    return next.handle(authReq);
  }
}