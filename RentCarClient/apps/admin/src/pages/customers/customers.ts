import { ChangeDetectionStrategy, Component, signal, ViewEncapsulation } from '@angular/core';
import Grid from '../../components/grid/grid';
import { FlexiGridModule } from 'flexi-grid';
import { BreadcrumbModel } from '../../services/breadcrumb';
import { NgxMaskPipe } from 'ngx-mask';

@Component({
  imports: [
    Grid,
    FlexiGridModule,
    NgxMaskPipe
  ],
  templateUrl: './customers.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export default class Customers {
  readonly breadcrumbs = signal<BreadcrumbModel[]>([
    {
      title: 'Müşteriler',
      icon: 'bi-person',
      url: '/customers',
      isActive: true,
    },
  ]);

}
