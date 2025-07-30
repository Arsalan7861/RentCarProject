import { Routes } from '@angular/router';

const router: Routes = [
  {
    path: '',
    loadComponent: () => import('./customers'),
  },
  {
    path: 'add',
    loadComponent: () => import('./create/create'),
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./create/create'),
  },
  {
    path: 'detail/:id',
    loadComponent: () => import('./detail/detail'),
  },
];

export default router;
