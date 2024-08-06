import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';
import { BusyService } from '../_services/busy.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private busyService: BusyService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    
    this.busyService.busy();
    return next.handle(request).pipe(
      // delay(800),
      finalize(() => {
        this.busyService.idle();
      })
    );
  }
}
