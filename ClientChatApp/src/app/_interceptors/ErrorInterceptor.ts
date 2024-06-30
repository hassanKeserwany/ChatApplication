import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMsg = '';
        if (error.error instanceof ErrorEvent) {
          // Client-side error
          console.log("Client-side error")
          errorMsg = `Error: ${error.error.message}`;
        } else {
          // Server-side error
          console.log("sever-side error")

          errorMsg = `Error Code: ${error.status}\nMessage: ${error.message}`;
        }
        // Show error message or handle error globally
        console.error(errorMsg);
        return throwError(errorMsg);
      })
    );
  }
}
