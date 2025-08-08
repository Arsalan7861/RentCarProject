import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  inject,
  signal,
  viewChild,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Result } from '../../../../../../libraries/shared/src/lib/models/result.model';
import { Router } from '@angular/router';
import { FormValidateDirective } from 'form-validate-angular';
import { CommonModule } from '@angular/common';
import { HttpService } from '../../../../../../libraries/shared/src/lib/services/http';
import { FlexiToastService } from 'flexi-toast';

@Component({
  imports: [FormsModule, FormValidateDirective, CommonModule],
  templateUrl: './login.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Login {
  readonly loading = signal<boolean>(false);
  readonly email = signal<string>('');
  readonly emailOrUsername = signal<string>('');
  readonly tfaCode = signal<string>('');
  readonly showTFAForm = signal<boolean>(false);
  readonly tfaConfirmCode = signal<string>('');
  readonly time = signal<{ min: number; sec: number }>({ min: 5, sec: 0 });

  readonly forgotPasswordModalLoading = signal<boolean>(false);
  readonly rememberMe = signal<boolean>(false);

  readonly closeBtn = viewChild<ElementRef<HTMLButtonElement>>('modalCloseBtn');
  readonly passwordEl = viewChild<ElementRef<HTMLInputElement>>('passwordEl');

  readonly #http = inject(HttpService);
  readonly #router = inject(Router);
  readonly #toast = inject(FlexiToastService);

  togglePassword() {
    const passwordInput = this.passwordEl()?.nativeElement;
    if (passwordInput) {
      passwordInput.type =
        passwordInput.type === 'password' ? 'text' : 'password';
    }
  }

  login(form: NgForm) {
    if (form.valid) {
      this.loading.set(true);

      const loginData = {
        emailOrUsername: form.value.emailOrUsername,
        password: form.value.password,
        rememberMe: this.rememberMe(), // Send to backend
      };

      this.#http.post<{ token: string | null; tfaCode: string | null }>(
        '/rent/auth/login',
        loginData,
        (res) => {
          if (res.token !== null) {
            localStorage.setItem('response', res.token);
            this.#router.navigateByUrl('/');
          } else if (res.tfaCode !== null) {
            this.tfaCode.set(res.tfaCode);
            this.showTFAForm.set(true);
            this.time.set({ min: 5, sec: 0 });

            const interval: any = setInterval(() => {
              let min = this.time().min;
              let sec = this.time().sec;

              sec--;
              if (sec < 0) {
                sec = 59;
                min--;
                if (min < 0) {
                  min = 0;
                  interval.clear();
                  this.showTFAForm.set(false);
                }
              }
              this.time.set({ min, sec });
            }, 1000);
          }
          this.loading.set(false);
        },
        (error) => {
          this.loading.set(false);
        }
      );
    }
  }

  onRememberMeChange(event: Event) {
    const target = event.target as HTMLInputElement;
    this.rememberMe.set(target.checked);
  }

  loginWithTFA(form: NgForm) {
    if (!form.valid) {
      return;
    }

    const data = {
      emailOrUsername: this.emailOrUsername(),
      tfaCode: this.tfaCode(),
      tfaConfirmCode: this.tfaConfirmCode(),
    };

    this.loading.set(true);
    this.#http.post<{ token: string | null; tfaCode: string | null }>(
      '/rent/auth/login-with-tfa',
      data,
      (res) => {
        localStorage.setItem('response', res.token!);
        this.#router.navigateByUrl('/');
        this.loading.set(false);
      },
      () => {
        this.loading.set(false);
      }
    );
  }

  forgotPassword() {
    this.forgotPasswordModalLoading.set(true);
    this.#http.post<string>(
      `/rent/auth/forgot-password/${this.email()}`,
      {},
      (res) => {
        this.#toast.showToast('Başarılı!', res, 'success');
        this.closeBtn()!.nativeElement.click();
        this.forgotPasswordModalLoading.set(false);
      },
      () => {
        this.forgotPasswordModalLoading.set(false);
      }
    );
  }
}
