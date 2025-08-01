import { Route } from '@angular/router';
import { authGuard } from './guards/auth-guard';
import { inject } from '@angular/core';
import { Common } from './services/common';

export const appRoutes: Route[] = [
  {
    path: 'unauthorized',
    loadComponent: () => import('./pages/unauthorized/unauthorized'),
  },
  {
    path: 'connection-error',
    loadComponent: () => import('./pages/connection-error/connection-error'),
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/auth/login/login'),
  },
  {
    path: 'reset-password/:id',
    loadComponent: () => import('./pages/auth/reset-password/reset-password'),
  },
  {
    path: 'form/:reservationId/:type',
    loadComponent: () => import('./pages/reservations/forms/forms'),
    // canActivate: [() => inject(Common).checkPermissionForRoute('form:view')],
  },
  {
    path: '',
    loadComponent: () => import('./pages/layouts/layouts'),
    canActivateChild: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/dashboard/dashboard'),
        canActivate: [
          () => inject(Common).checkPermissionForRoute('dashboard:view'),
        ],
      },
      {
        path: 'branches',
        loadChildren: () => import('./pages/branches/router'),
      },
      {
        path: 'roles',
        loadChildren: () => import('./pages/roles/router'),
      },
      {
        path: 'users',
        loadChildren: () => import('./pages/users/router'),
      },
      {
        path: 'categories',
        loadChildren: () => import('./pages/categories/router'),
      },
      {
        path: 'protection-packages',
        loadChildren: () => import('./pages/protection-packages/router'),
      },
      {
        path: 'extras',
        loadChildren: () => import('./pages/extras/router'),
      },
      {
        path: 'vehicles',
        loadChildren: () => import('./pages/vehicles/router'),
      },
      {
        path: 'customers',
        loadChildren: () => import('./pages/customers/router'),
      },
      {
        path: 'reservations',
        loadChildren: () => import('./pages/reservations/router'),
      },
    ],
  },
];
