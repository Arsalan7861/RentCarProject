import { Translations } from './services/translation.service';

export interface NavigationModel {
  title: string;
  translationKey?: keyof Translations;
  url?: string;
  icon?: string;
  haveSubNav?: boolean;
  subNavs?: NavigationModel[];
  permission: string;
}

export const navigations: NavigationModel[] = [
  {
    title: 'Dashboard',
    translationKey: 'nav.dashboard',
    url: '/',
    icon: 'bi-speedometer2',
    permission: 'dashboard:view',
  },
  {
    title: 'Rezervasyonlar',
    translationKey: 'nav.reservations',
    url: '/reservations',
    icon: 'bi-calendar-check',
    permission: 'reservation:view',
  },
  {
    title: 'Şubeler',
    translationKey: 'nav.branches',
    url: '/branches',
    icon: 'bi-buildings',
    permission: 'branch:view',
  },
  {
    title: 'Roller',
    translationKey: 'nav.roles',
    url: '/roles',
    icon: 'bi-person-rolodex',
    permission: 'role:view',
  },
  {
    title: 'Kullanıcılar',
    translationKey: 'nav.users',
    url: '/users',
    icon: 'bi-people',
    permission: 'user:view',
  },
  {
    title: 'Müşteriler',
    translationKey: 'nav.customers',
    url: '/customers',
    icon: 'bi-person',
    permission: 'customer:view',
  },
  {
    title: 'Kategoriler',
    translationKey: 'nav.categories',
    url: '/categories',
    icon: 'bi-tags',
    permission: 'category:view',
  },
  {
    title: 'Araçlar',
    translationKey: 'nav.vehicles',
    url: '/vehicles',
    icon: 'bi-car-front',
    permission: 'vehicle:view',
  },
  {
    title: 'Güvence Paketleri',
    translationKey: 'nav.protection-packages',
    url: '/protection-packages',
    icon: 'bi-shield-check',
    permission: 'protectionpackage:view',
  },
  {
    title: 'Ekstralar',
    translationKey: 'nav.extras',
    url: '/extras',
    icon: 'bi-plus-square',
    permission: 'extra:view',
  },
];
