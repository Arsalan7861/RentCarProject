import { Routes } from '@angular/router';

export const Router: Routes = [
  {
    path: '',
    loadComponent: () => import('./reservations'),
  },
  {
    path: 'add',
    loadComponent: () => import('./create/create'),
  },
];

export default Router;
