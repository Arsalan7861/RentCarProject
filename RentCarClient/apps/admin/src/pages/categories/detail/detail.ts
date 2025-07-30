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
import { Result } from 'apps/admin/src/models/result.model';
import {
  BreadcrumbModel,
  BreadcrumbService,
} from 'apps/admin/src/services/breadcrumb';
import Blank from 'apps/admin/src/components/blank/blank';
import {
  initialCategory,
  CategoryModel,
} from 'apps/admin/src/models/category.model';

@Component({
  imports: [Blank],
  templateUrl: './detail.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Detail {
  readonly id = signal<string>('');
  readonly breadcrumbs = signal<BreadcrumbModel[]>([]);

  readonly result = httpResource<Result<CategoryModel>>(
    () => `/rent/categories/${this.id()}`
  );
  readonly data = computed(() => this.result.value()?.data ?? initialCategory);
  readonly loading = computed(() => this.result.isLoading());
  readonly pageTitle = signal("Kategori Detay");

  readonly #activated = inject(ActivatedRoute);
  readonly #breadcrumb = inject(BreadcrumbService);

  constructor() {
    this.#activated.params.subscribe((params) => {
      this.id.set(params['id']);
    });
    effect(() => {
      const breadCrumbs: BreadcrumbModel[] = [
        {
          title: 'Kategoriler',
          icon: 'bi-tags',
          url: '/categories',
        },
      ];

      if (this.data()) {
        this.breadcrumbs.set(breadCrumbs);
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: this.data().name,
            icon: 'bi-zoom-in',
            url: `/categories/detail/${this.id()}`,
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());
      }
    });
  }
}
