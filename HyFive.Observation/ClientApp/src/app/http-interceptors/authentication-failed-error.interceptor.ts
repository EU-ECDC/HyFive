import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';

import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {AuthenticationEventService} from '../services/events/authentication-event.service';

@Injectable()
export class AuthenticationFailedErrorInterceptor implements HttpInterceptor {

  constructor(private authenticationEventService: AuthenticationEventService) {
  }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
          if(error.status == 401 || error.status == 403)
          {
            this.authenticationEventService.authenticationFailedEvent.emit(true);
          }
        return throwError(error);
      })
    );
  }
}
