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
  ReservationModel,
  initialReservation,
} from 'libraries/shared/src/lib/models/reservation.model';
import { DatePipe, NgClass } from '@angular/common';
import { NgxMaskPipe } from 'ngx-mask';

@Component({
  imports: [Blank, DatePipe, NgClass, NgxMaskPipe, TrCurrencyPipe],
  templateUrl: './detail.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Detail {
  readonly id = signal<string>('');
  readonly breadcrumbs = signal<BreadcrumbModel[]>([]);

  readonly result = httpResource<Result<ReservationModel>>(
    () => `/rent/reservations/${this.id()}`
  );
  readonly data = computed(
    () => this.result.value()?.data ?? initialReservation
  );
  readonly loading = computed(() => this.result.isLoading());
  readonly pageTitle = signal('Rezervasyon Detay');

  readonly #activated = inject(ActivatedRoute);
  readonly #breadcrumb = inject(BreadcrumbService);

  constructor() {
    this.#activated.params.subscribe((params) => {
      this.id.set(params['id']);
    });
    effect(() => {
      const breadCrumbs: BreadcrumbModel[] = [
        {
          title: 'Rezervasyonlar',
          icon: 'bi-calendar-check',
          url: '/reservations',
        },
      ];

      if (this.data()) {
        this.breadcrumbs.set(breadCrumbs);
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: this.data().reservationNumber,
            icon: 'bi-zoom-in',
            url: `/reservations/detail/${this.id()}`,
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());
      }
    });
  }

  getStatusClass() {
    switch (this.data().status) {
      case 'Bekliyor':
        return 'badge bg-warning';
      case 'Teslim Edildi':
        return 'badge bg-info';
      case 'Tamamlandı':
        return 'badge bg-success';
      case 'İptal Edildi':
        return 'badge bg-danger';
      default:
        return '';
    }
  }
}
