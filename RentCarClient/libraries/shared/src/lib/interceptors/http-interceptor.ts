import { HttpInterceptorFn } from '@angular/common/http';

export const httpInterceptor: HttpInterceptorFn = (req, next) => {
  const url = req.url;
  const endpoint = 'https://localhost:7033/';
  const clone = req.clone({
    // clone -> creates a new request with the modified URL because the original request is immutable
    url: url.replace('/rent/', endpoint),
  });

  return next(clone);
};
