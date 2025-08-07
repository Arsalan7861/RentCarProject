import { Injectable, signal } from '@angular/core';

export type Language = 'tr' | 'en';

export interface Translations {
  // Navigation
  'nav.dashboard': string;
  'nav.reservations': string;
  'nav.branches': string;
  'nav.roles': string;
  'nav.users': string;
  'nav.customers': string;
  'nav.categories': string;
  'nav.vehicles': string;
  'nav.protection-packages': string;
  'nav.extras': string;

  // Profile Menu
  'profile.my-profile': string;
  'profile.account-settings': string;
  'profile.security': string;
  'profile.help-support': string;
  'profile.logout': string;

  // Security Settings
  'security.title': string;
  'security.tfa': string;
  'security.tfa-description-enabled': string;
  'security.tfa-description-disabled': string;

  // Buttons
  'button.save': string;
  'button.cancel': string;
  'button.close': string;
  'button.check-token': string;

  // Messages
  'message.tfa-enabled': string;
  'message.tfa-disabled': string;
  'message.tfa-error': string;
  'message.success': string;
  'message.error': string;
}

const translations: Record<Language, Translations> = {
  tr: {
    // Navigation
    'nav.dashboard': 'Dashboard',
    'nav.reservations': 'Rezervasyonlar',
    'nav.branches': 'Şubeler',
    'nav.roles': 'Roller',
    'nav.users': 'Kullanıcılar',
    'nav.customers': 'Müşteriler',
    'nav.categories': 'Kategoriler',
    'nav.vehicles': 'Araçlar',
    'nav.protection-packages': 'Korunma Paketleri',
    'nav.extras': 'Ekstra Hizmetler',

    // Profile Menu
    'profile.my-profile': 'Profilim',
    'profile.account-settings': 'Hesap Ayarları',
    'profile.security': 'Güvenlik',
    'profile.help-support': 'Yardım & Destek',
    'profile.logout': 'Çıkış Yap',

    // Security Settings
    'security.title': 'Güvenlik Ayarları',
    'security.tfa': 'İki Adımlı Doğrulama',
    'security.tfa-description-enabled':
      'Etkinleştirildiğinde, giriş yaparken telefon numaranıza gönderilen kodu girmeniz gerekecek.',
    'security.tfa-description-disabled':
      'Devre dışı bırakıldığında, sadece kullanıcı adı ve şifre ile giriş yapabilirsiniz.',

    // Buttons
    'button.save': 'Kaydet',
    'button.cancel': 'İptal Et',
    'button.close': 'Kapat',
    'button.check-token': 'Token Kontrol Et',

    // Messages
    'message.tfa-enabled':
      'İki adımlı doğrulama etkinleştirildi, Tekrar giriş yapmanız gerekmektedir',
    'message.tfa-disabled':
      'İki adımlı doğrulama devre dışı bırakıldı, Tekrar giriş yapmanız gerekmektedir',
    'message.tfa-error': 'İki adımlı doğrulama ayarları güncellenemedi',
    'message.success': 'Başarılı!',
    'message.error': 'Hata!',
  },
  en: {
    // Navigation
    'nav.dashboard': 'Dashboard',
    'nav.reservations': 'Reservations',
    'nav.branches': 'Branches',
    'nav.roles': 'Roles',
    'nav.users': 'Users',
    'nav.customers': 'Customers',
    'nav.categories': 'Categories',
    'nav.vehicles': 'Vehicles',
    'nav.protection-packages': 'Protection Packages',
    'nav.extras': 'Extra Services',

    // Profile Menu
    'profile.my-profile': 'My Profile',
    'profile.account-settings': 'Account Settings',
    'profile.security': 'Security',
    'profile.help-support': 'Help & Support',
    'profile.logout': 'Logout',

    // Security Settings
    'security.title': 'Security Settings',
    'security.tfa': 'Two-Factor Authentication',
    'security.tfa-description-enabled':
      'When enabled, you will need to enter a code sent to your phone number when logging in.',
    'security.tfa-description-disabled':
      'When disabled, you can log in with just your username and password.',

    // Buttons
    'button.save': 'Save',
    'button.cancel': 'Cancel',
    'button.close': 'Close',
    'button.check-token': 'Check Token',

    // Messages
    'message.tfa-enabled':
      'Two-factor authentication enabled, you need to log in again',
    'message.tfa-disabled':
      'Two-factor authentication disabled, you need to log in again',
    'message.tfa-error': 'Could not update two-factor authentication settings',
    'message.success': 'Success!',
    'message.error': 'Error!',
  },
};

@Injectable({
  providedIn: 'root',
})
export class TranslationService {
  readonly currentLanguage = signal<Language>('tr');

  constructor() {
    // Load language from localStorage or browser preference
    const savedLanguage = localStorage.getItem('language') as Language;
    const browserLanguage = navigator.language.startsWith('en') ? 'en' : 'tr';

    this.currentLanguage.set(savedLanguage || browserLanguage);
  }

  /**
   * Get translation for a key
   */
  translate(key: keyof Translations): string {
    return translations[this.currentLanguage()][key] || key;
  }

  /**
   * Set current language
   */
  setLanguage(language: Language) {
    this.currentLanguage.set(language);
    localStorage.setItem('language', language);
  }

  /**
   * Get all supported languages
   */
  getSupportedLanguages(): { code: Language; name: string }[] {
    return [
      { code: 'tr', name: 'Türkçe' },
      { code: 'en', name: 'English' },
    ];
  }
}
