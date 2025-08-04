/* eslint-disable @nx/enforce-module-boundaries */
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
import { httpResource } from '@angular/common/http';
import { TrCurrencyPipe } from 'tr-currency';
import Blank from 'apps/admin/src/components/blank/blank';
import {
  initialProtectionPackage,
  ProtectionPackageModel,
} from 'libraries/shared/src/lib/models/protection-package.model';
import { Result } from 'libraries/shared/src/lib/models/result.model';
import {
  BreadcrumbModel,
  BreadcrumbService,
} from 'apps/admin/src/services/breadcrumb';

@Component({
  imports: [Blank, TrCurrencyPipe],
  templateUrl: './detail.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ProtectionPackageDetail {
  readonly id = signal<string>('');
  readonly bredcrumbs = signal<BreadcrumbModel[]>([]);
  readonly result = httpResource<Result<ProtectionPackageModel>>(
    () => `/rent/protection-packages/${this.id()}`
  );
  readonly data = computed(
    () => this.result.value()?.data ?? initialProtectionPackage
  );
  readonly loading = computed(() => this.result.isLoading());
  readonly pageTitle = signal<string>('Güvence Paketi Detay');

  readonly #activated = inject(ActivatedRoute);
  readonly #breadcrumb = inject(BreadcrumbService);

  constructor() {
    this.#activated.params.subscribe((res) => {
      this.id.set(res['id']);
    });

    effect(() => {
      const breadCrumbs: BreadcrumbModel[] = [
        {
          title: 'Güvence Paketleri',
          icon: 'bi-shield-check',
          url: '/protection-packages',
        },
      ];

      if (this.data()) {
        this.bredcrumbs.set(breadCrumbs);
        this.bredcrumbs.update((prev) => [
          ...prev,
          {
            title: this.data().name,
            icon: 'bi-zoom-in',
            url: `/protection-packages/detail/${this.id()}`,
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.bredcrumbs());
      }
    });
  }
}
