import { NgClass } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  inject,
  ViewEncapsulation,
} from '@angular/core';
import { RouterLink } from '@angular/router';

// eslint-disable-next-line @nx/enforce-module-boundaries
import { BreadcrumbService } from 'apps/admin/src/services/breadcrumb';

@Component({
  selector: 'app-breadcrumb',
  imports: [NgClass, RouterLink],
  templateUrl: './breadcrumb.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Breadcrumb {
  readonly breadcrumb = inject(BreadcrumbService);
}
