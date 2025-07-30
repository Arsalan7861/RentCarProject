import {
  ChangeDetectionStrategy,
  Component,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { BreadcrumbModel } from '../../services/breadcrumb';
import { NgxMaskPipe } from 'ngx-mask';
import Grid from '../../components/grid/grid';
import { FlexiGridModule } from 'flexi-grid';
import { FormsModule } from "@angular/forms";

@Component({
  imports: [
    Grid,
    NgxMaskPipe,
    FlexiGridModule,
    FormsModule
],
  templateUrl: './branches.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Branches {
  readonly breadcrumbs = signal<BreadcrumbModel[]>([
    {
      title: 'Şubeler',
      icon: 'bi-buildings',
      url: '/branches',
      isActive: true,
    },
  ]);
}
