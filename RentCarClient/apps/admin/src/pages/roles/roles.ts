import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { BreadcrumbModel } from '../../services/breadcrumb';
import Grid from '../../components/grid/grid';
import { FlexiGridModule } from 'flexi-grid';
import { RouterLink } from '@angular/router';
import { Common } from '../../services/common';
import { TranslationService } from '../../services/translation.service';

@Component({
  imports: [Grid, FlexiGridModule, RouterLink],
  templateUrl: './roles.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Roles {
  
  readonly #common = inject(Common);
  readonly #translation = inject(TranslationService);
  
  readonly breadcrumbs = signal<BreadcrumbModel[]>([
    {
      title: this.#translation.translate('nav.roles'),
      icon: 'bi-person-rolodex',
      url: '/roles',
      isActive: true,
    },
  ]);

  checkPermission(permission: string) {
    return this.#common.checkPermission(permission);
  }
}
