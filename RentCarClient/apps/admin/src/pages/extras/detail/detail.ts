/* eslint-disable @nx/enforce-module-boundaries */
import { httpResource } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TrCurrencyPipe } from 'tr-currency';
import { Result } from 'libraries/shared/src/lib/models/result.model';
import {
  BreadcrumbModel,
  BreadcrumbService,
} from 'apps/admin/src/services/breadcrumb';
import Blank from 'apps/admin/src/components/blank/blank';
import {
  initialExtra,
  ExtraModel,
} from 'libraries/shared/src/lib/models/extra.model';

@Component({
  imports: [Blank, TrCurrencyPipe],
  templateUrl: './detail.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Detail {
  readonly id = signal<string>('');
  readonly breadcrumbs = signal<BreadcrumbModel[]>([]);

  readonly result = httpResource<Result<ExtraModel>>(
    () => `/rent/extras/${this.id()}`
  );
  readonly data = computed(() => this.result.value()?.data ?? initialExtra);
  readonly loading = computed(() => this.result.isLoading());
  readonly pageTitle = signal('Ekstra Detay');

  readonly #activated = inject(ActivatedRoute);
  readonly #breadcrumb = inject(BreadcrumbService);

  constructor() {
    this.#activated.params.subscribe((params) => {
      this.id.set(params['id']);
    });
    effect(() => {
      const breadCrumbs: BreadcrumbModel[] = [
        {
          title: 'Ekstralar',
          icon: 'bi-plus-square',
          url: '/extras',
        },
      ];

      if (this.data()) {
        this.breadcrumbs.set(breadCrumbs);
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: this.data().name,
            icon: 'bi-zoom-in',
            url: `/extras/detail/${this.id()}`,
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());
      }
    });
  }
}
