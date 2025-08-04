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
import { Result } from 'libraries/shared/src/lib/models/result.model';
import {
  BreadcrumbModel,
  BreadcrumbService,
} from 'apps/admin/src/services/breadcrumb';
import Blank from 'apps/admin/src/components/blank/blank';
import {
  BranchModel,
  initialBranch,
} from 'libraries/shared/src/lib/models/branch.model';
import { NgxMaskPipe } from 'ngx-mask';

@Component({
  imports: [Blank, NgxMaskPipe],
  templateUrl: './detail.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Detail {
  readonly id = signal<string>('');
  readonly breadcrumbs = signal<BreadcrumbModel[]>([]);

  readonly result = httpResource<Result<BranchModel>>(
    () => `/rent/branches/${this.id()}`
  );
  readonly data = computed(() => this.result.value()?.data ?? initialBranch);
  readonly loading = computed(() => this.result.isLoading());
  readonly pageTitle = signal('Şube Detay');

  readonly #activated = inject(ActivatedRoute);
  readonly #breadcrumb = inject(BreadcrumbService);

  constructor() {
    this.#activated.params.subscribe((params) => {
      this.id.set(params['id']);
    });
    effect(() => {
      const breadCrumbs: BreadcrumbModel[] = [
        {
          title: 'Şubeler',
          icon: 'bi-buildings',
          url: '/branches',
        },
      ];

      if (this.data()) {
        this.breadcrumbs.set(breadCrumbs);
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: this.data().name,
            icon: 'bi-zoom-in',
            url: `/branches/detail/${this.id()}`,
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());
      }
    });
  }
}
