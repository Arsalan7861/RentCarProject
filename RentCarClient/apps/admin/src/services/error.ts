import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { FlexiToastService } from 'flexi-toast';

@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  readonly #toast = inject(FlexiToastService);
  readonly #router = inject(Router);

  handle(error: HttpErrorResponse) {
    console.error('Error occurred:', error);

    const status = error.status;

    // Bağlantı hatası (status 0) - API'ye ulaşılamıyor
    if (status === 0) {
      console.error('Connection error: Unable to reach the server');
      this.#router.navigateByUrl('/connection-error');
      return;
    } else if (status === 422 || status === 403 || status === 500) {
      // Handle specific error statuses
      const messages = error.error.errorMessages;
      if (messages) {
        messages.forEach((msg: string) => {
          console.error('Error message:', msg);
          this.#toast.showToast('Hata!', msg, 'error');
        });
      }
    } else if (status === 401) {
      const message = 'Tekrar giriş yapmanız gerekiyor.';
      this.#toast.showToast('Hata!', message, 'error');
      this.#router.navigateByUrl('/login');
      localStorage.clear(); // tokeni sil login sayfasına yönlendir
    } else {
      this.#toast.showToast('Hata!', error.message, 'error');
    }
  }
}
