import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation,
  inject,
  signal,
  computed,
} from '@angular/core';
import { NavigationModel, navigations } from '../../navigation';
import { NgClass } from '@angular/common';
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import Breadcrumb from './breadcrumb/breadcrumb';
import { Common } from '../../services/common';
import { FormsModule } from '@angular/forms';
import { FlexiPopupModule } from 'flexi-popup';
import { TfaService } from '../../services/tfa.service';
import { FlexiToastService } from 'flexi-toast';
@Component({
  imports: [
    NgClass,
    RouterLink,
    RouterOutlet,
    Breadcrumb,
    RouterLinkActive,
    FormsModule,
    FlexiPopupModule,
  ],
  templateUrl: './layouts.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Layouts {
  readonly id = signal<string>('');

  readonly navigations = signal<NavigationModel[]>(navigations);
  readonly decode = computed(() => {
    const decodedData = this.#common.decode();
    console.log('Decode data in layouts:', decodedData);
    console.log('TFA Status:', decodedData.tfaStatus);
    return decodedData;
  });

  // Local state for the TFA checkbox (separate from actual TFA status)
  readonly localTfaStatus = signal<boolean>(false);

  // Track if there are unsaved changes
  readonly hasUnsavedChanges = computed(
    () => this.localTfaStatus() !== this.decode().tfaStatus
  );

  isSettingsPopupVisible = false;
  readonly isSettingsPopupLoading = signal<boolean>(false);

  readonly #router = inject(Router);
  readonly #common = inject(Common);
  readonly #tfaService = inject(TfaService);
  readonly #toast = inject(FlexiToastService);

  logout() {
    localStorage.clear();
    this.#router.navigateByUrl('login');
  }

  checkPermission(permission: string) {
    return this.#common.checkPermission(permission);
  }

  // Initialize local TFA status when popup opens
  openSettingsPopup() {
    this.localTfaStatus.set(this.decode().tfaStatus);
    this.isSettingsPopupVisible = true;
  }

  // Handle checkbox change (only updates local state)
  onTfaCheckboxChange(event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;
    this.localTfaStatus.set(isChecked);
  }

  // Save the TFA status changes
  saveTfaSettings() {
    const newStatus = this.localTfaStatus();

    this.isSettingsPopupLoading.set(true);

    this.#tfaService.updateTfaStatus(
      newStatus,
      () => {
        // Success callback
        this.#toast.showToast(
          'Başarılı!',
          newStatus
            ? 'İki adımlı doğrulama etkinleştirildi, Tekrar giriş yapmanız gerekmektedir'
            : 'İki adımlı doğrulama devre dışı bırakıldı, Tekrar giriş yapmanız gerekmektedir',
          'success'
        );
        this.logout();
      },
      () => {
        // Error callback - reset local state to actual state
        this.localTfaStatus.set(this.decode().tfaStatus);
        this.#toast.showToast(
          'Hata!',
          'İki adımlı doğrulama ayarları güncellenemedi',
          'error'
        );
        this.isSettingsPopupLoading.set(false);
      }
    );
  }

  // Reset changes
  resetTfaSettings() {
    this.localTfaStatus.set(this.decode().tfaStatus);
  }
}
