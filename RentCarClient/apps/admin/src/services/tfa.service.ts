import { inject, Injectable, signal } from '@angular/core';
import { HttpService } from '../../../../libraries/shared/src/lib/services/http';
import { Result } from '../../../../libraries/shared/src/lib/models/result.model';
import { Common } from './common';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class TfaService {
  readonly #http = inject(HttpService);
  readonly #common = inject(Common);

  updateTfaStatus(
    status: boolean,
    callback?: () => void,
    errorCallback?: () => void
  ) {
    this.#http.post<Result<{ newToken: string }>>(
      '/rent/auth/update-tfa-status',
      { status: status },
      (res) => {
        // Update token with new TFA status
        if (res.data?.newToken) {
          localStorage.setItem('response', res.data.newToken);

          // Update the decode signal immediately
          const currentDecode = this.#common.decode();
          this.#common.decode.set({
            ...currentDecode,
            tfaStatus: status,
          });
        }
        callback?.();
      },
      () => {
        errorCallback?.();
      }
    );
  }
}
