import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexiGridFilterDataModel, FlexiGridModule } from 'flexi-grid';
import Grid from '../../components/grid/grid';
import { BreadcrumbModel } from '../../services/breadcrumb';
import { Common } from '../../services/common';
import { NgxMaskPipe } from 'ngx-mask';

@Component({
  imports: [Grid, FlexiGridModule, CommonModule, NgxMaskPipe],
  templateUrl: './reservations.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Reservations {
  readonly breadcrumbs = signal<BreadcrumbModel[]>([
    {
      title: 'Rezervasyonlar',
      icon: 'bi-calendar-check',
      url: '/reservations',
      isActive: true,
    },
  ]);

  readonly statusFilterData = signal<FlexiGridFilterDataModel[]>([
    {
      name: 'Bekliyor',
      value: 'Bekliyor',
    },
    {
      name: 'Teslim Edildi',
      value: 'Teslim Edildi',
    },
    {
      name: 'Tamamlandı',
      value: 'Tamamlandı',
    },
    {
      name: 'İptal Edildi',
      value: 'İptal Edildi',
    },
  ]);

  readonly #common = inject(Common);

  checkPermission(permission: string) {
    return this.#common.checkPermission(permission);
  }

  getStatusClass(status: string) {
    switch (status) {
      case 'Bekliyor':
        return 'flexi-grid-card-warning';
      case 'Teslim Edildi':
        return 'flexi-grid-card-info';
      case 'Tamamlandı':
        return 'flexi-grid-card-success';
      case 'İptal Edildi':
        return 'flexi-grid-card-danger';
      default:
        return '';
    }
  }
}
