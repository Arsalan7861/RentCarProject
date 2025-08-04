/* eslint-disable no-var */
/* eslint-disable @nx/enforce-module-boundaries */
import { NgClass } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
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
import { NgxMaskDirective } from 'ngx-mask';
import { lastValueFrom } from 'rxjs';
import {
  BranchModel,
  initialBranch,
} from 'libraries/shared/src/lib/models/branch.model';
import { httpResource } from '@angular/common/http';
import { FlexiSelectModule } from 'flexi-select';

@Component({
  imports: [
    Blank,
    FormsModule,
    FormValidateDirective,
    NgClass,
    NgxMaskDirective,
    FlexiSelectModule,
  ],
  templateUrl: './create.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Create {
  readonly id = signal<string | undefined>(undefined);
  readonly breadcrumbs = signal<BreadcrumbModel[]>([
    {
      title: 'Şubeler',
      icon: 'bi-buildings',
      url: '/branches',
    },
  ]);
  readonly pageTitle = computed(() => {
    return this.id() ? 'Şube Güncelle' : 'Yeni Şube';
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
        this.#http.getResource<BranchModel>(`/rent/branches/${this.id()}`)
      );

      this.breadcrumbs.update((prev) => [
        ...prev,
        {
          title: res.data!.name,
          icon: 'bi-pen',
          url: `/branches/edit/${this.id()}`,
          isActive: true,
        },
      ]);
      this.#breadcrumb.reset(this.breadcrumbs());
      return res.data;
    },
  });
  readonly data = linkedSignal(() => {
    return this.result.value() ?? { ...initialBranch };
  });
  readonly loading = linkedSignal(() => this.result.isLoading());
  readonly ilResult = httpResource<any[]>(() => '/il-ilce.json');
  readonly iller = computed(() => this.ilResult.value() ?? []);
  readonly ilLoading = computed(() => this.ilResult.isLoading());
  readonly ilceler = signal<any[]>([]);

  readonly #breadcrumb = inject(BreadcrumbService);
  readonly #activated = inject(ActivatedRoute);
  readonly #http = inject(HttpService);
  readonly #toast = inject(FlexiToastService);
  readonly #router = inject(Router);

  constructor() {
    this.#activated.params.subscribe((res) => {
      if (res['id']) {
        this.id.set(res['id']);
      } else {
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: 'Yeni Şube',
            icon: 'bi-plus',
            url: '/branches/add',
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());
      }
    });

    effect(() => {
      if (this.data().address.city) {
        this.getIlceler();
      }
    });
  }

  save(form: NgForm) {
    if (!form.valid) return;

    if (!this.id()) {
      this.loading.set(true);
      this.#http.post<string>(
        '/rent/branches',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'success');
          this.#router.navigateByUrl('/branches');
          this.loading.set(false);
        },
        () => {
          this.loading.set(false);
        }
      );
    } else {
      this.loading.set(true);
      this.#http.put<string>(
        '/rent/branches/',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'info');
          this.#router.navigateByUrl('/branches');
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

  getIlceler() {
    const il = this.iller().find(
      (item) => item.il_adi === this.data().address.city
    );
    if (il) {
      this.ilceler.set(il.ilceler);
    }
  }
}
