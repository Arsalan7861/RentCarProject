import {
  ApplicationConfig,
  LOCALE_ID,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { appRoutes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { httpInterceptor } from '@shared/lib/interceptors/http-interceptor';
import { errorInterceptor } from '@shared/lib/interceptors/error-interceptor';
import { authInterceptor } from '@shared/lib/interceptors/auth-interceptor';
import { provideNgxMask } from 'ngx-mask';
import localTr from '@angular/common/locales/tr';
import { registerLocaleData } from '@angular/common';

registerLocaleData(localTr);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(appRoutes),
    provideNgxMask(),
    provideHttpClient(
      withInterceptors([httpInterceptor, authInterceptor, errorInterceptor])
    ), // burdan errorInterceptor'ı sildik çünkü normal http isteklerin hatalarını eziyor
    {
      provide: LOCALE_ID,
      useValue: 'tr-TR', // Türkçe dil desteği için
    },
  ],
};

// export const SKIP_ERROR_HANDLER = new HttpContextToken<boolean>(() => false);
