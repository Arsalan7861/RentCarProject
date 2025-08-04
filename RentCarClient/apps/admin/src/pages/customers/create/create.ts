/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable no-var */
/* eslint-disable @nx/enforce-module-boundaries */
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  linkedSignal,
  resource,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import Blank from 'apps/admin/src/components/blank/blank';
import {
  BreadcrumbModel,
  BreadcrumbService,
} from 'apps/admin/src/services/breadcrumb';
import { FormValidateDirective } from 'form-validate-angular';
import { HttpService } from 'apps/admin/src/services/http';
import { FlexiToastService } from 'flexi-toast';
import { lastValueFrom } from 'rxjs';
import { FlexiSelectModule } from 'flexi-select';
import {
  CustomerModel,
  initialCustomerModel,
} from 'libraries/shared/src/lib/models/customer.model';
import { DatePipe } from '@angular/common';
import { NgxMaskDirective } from 'ngx-mask';
import { Chart, ChartConfiguration, ChartType, registerables } from 'chart.js';

Chart.register(...registerables);

interface ChartData {
  data: any[];
  borderColor: string[];
}

@Component({
  imports: [
    Blank,
    FormsModule,
    FormValidateDirective,
    FlexiSelectModule,
    NgxMaskDirective,
  ],
  templateUrl: './create.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [DatePipe],
})
export default class Create {
  readonly id = signal<string | undefined>(undefined);
  readonly breadcrumbs = signal<BreadcrumbModel[]>([
    {
      title: 'Müşteriler',
      icon: 'bi-person',
      url: '/customers',
    },
  ]);
  readonly pageTitle = computed(() => {
    return this.id() ? 'Müşteri Güncelle' : 'Yeni Müşteri';
  });
  readonly pageIcon = computed(() => {
    return this.id() ? 'bi-pencil-square' : 'bi-plus';
  });
  readonly btnName = computed(() => {
    return this.id() ? 'Güncelle' : 'Ekle';
  });

  readonly result = resource({
    params: () => this.id(),
    loader: async () => {
      var res = await lastValueFrom(
        this.#http.getResource<CustomerModel>(`/rent/customers/${this.id()}`)
      );

      this.breadcrumbs.update((prev) => [
        ...prev,
        {
          title: res.data!.fullName,
          icon: 'bi-pen',
          url: `/customers/edit/${this.id()}`,
          isActive: true,
        },
      ]);
      this.#breadcrumb.reset(this.breadcrumbs());
      return res.data;
    },
  });
  readonly data = linkedSignal(() => {
    return this.result.value() ?? { ...initialCustomerModel };
  });
  readonly loading = linkedSignal(() => this.result.isLoading());

  readonly #breadcrumb = inject(BreadcrumbService);
  readonly #activated = inject(ActivatedRoute);
  readonly #http = inject(HttpService);
  readonly #toast = inject(FlexiToastService);
  readonly #router = inject(Router);
  readonly #date = inject(DatePipe);

  constructor() {
    this.#activated.params.subscribe((res) => {
      if (res['id']) {
        this.id.set(res['id']);
      } else {
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: 'Yeni Müşteri',
            icon: 'bi-plus',
            url: '/customers/add',
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());

        const date = this.#date.transform(new Date(), 'yyyy-MM-dd');
        this.data.update((prev) => ({
          ...prev,
          birthDate: date!,
          drivingLicenseIssuanceDate: date!,
        }));
      }
    });
  }

  save(form: NgForm) {
    if (!form.valid) return;

    this.loading.set(true);

    if (!this.id()) {
      this.#http.post<string>(
        '/rent/customers',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'success');
          this.#router.navigateByUrl('/customers');
          this.loading.set(false);
        },
        () => {
          this.loading.set(false);
        }
      );
    } else {
      this.#http.put<string>(
        '/rent/customers',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'info');
          this.#router.navigateByUrl('/customers');
          this.loading.set(false);
        },
        () => {
          this.loading.set(false);
        }
      );
    }
  }

  changeStatus(status: boolean) {
    this.data.update((prev) => ({
      ...prev,
      isActive: status,
    }));
  }
}
