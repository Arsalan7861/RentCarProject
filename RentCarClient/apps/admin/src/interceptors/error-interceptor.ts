import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, of } from 'rxjs';
import { ErrorService } from '../services/error';
import { SKIP_ERROR_HANDLER } from '../app.config';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.context.get(SKIP_ERROR_HANDLER)) {
    // If the request has SKIP_ERROR_HANDLER set to true, skip error handling
    return next(req);
  }

  const errorService = inject(ErrorService);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle the error
      errorService.handle(error);
      return of();
    })
  );
};
