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
import { TranslationService } from '../../services/translation.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
@Component({
  imports: [
    NgClass,
    RouterLink,
    RouterOutlet,
    Breadcrumb,
    RouterLinkActive,
    FormsModule,
    FlexiPopupModule,
    TranslatePipe,
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
  readonly #translation = inject(TranslationService);

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
          this.#translation.translate('message.success'),
          this.#translation.translate(
            newStatus ? 'message.tfa-enabled' : 'message.tfa-disabled'
          ),
          'success'
        );
        this.logout();
      },
      () => {
        // Error callback - reset local state to actual state
        this.localTfaStatus.set(this.decode().tfaStatus);
        this.#toast.showToast(
          this.#translation.translate('message.error'),
          this.#translation.translate('message.tfa-error'),
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

  // Toggle language
  toggleLanguage() {
    const currentLang = this.#translation.currentLanguage();
    const newLang = currentLang === 'tr' ? 'en' : 'tr';
    this.#translation.setLanguage(newLang);
  }

  // Get current language info
  getCurrentLanguage() {
    return this.#translation
      .getSupportedLanguages()
      .find((lang) => lang.code === this.#translation.currentLanguage());
  }

  // Get the opposite language name for the toggle button
  readonly nextLanguageName = computed(() => {
    return this.#translation.currentLanguage() === 'tr' ? 'English' : 'Türkçe';
  });
}
