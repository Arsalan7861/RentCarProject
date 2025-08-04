/* eslint-disable no-var */
/* eslint-disable @nx/enforce-module-boundaries */
import { NgClass } from '@angular/common';
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
import {
  initialRole,
  RoleModel,
} from 'libraries/shared/src/lib/models/role.model';

@Component({
  imports: [Blank, FormsModule, FormValidateDirective, NgClass],
  templateUrl: './create.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Create {
  readonly id = signal<string | undefined>(undefined);
  readonly breadcrumbs = signal<BreadcrumbModel[]>([
    {
      title: 'Roller',
      icon: 'bi-person-rolodex',
      url: '/roles',
    },
  ]);
  readonly pageTitle = computed(() => {
    return this.id() ? 'Rol Güncelle' : 'Yeni Rol';
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
        this.#http.getResource<RoleModel>(`/rent/roles/${this.id()}`)
      );

      this.breadcrumbs.update((prev) => [
        ...prev,
        {
          title: res.data!.name,
          icon: 'bi-pen',
          url: `/roles/edit/${this.id()}`,
          isActive: true,
        },
      ]);
      this.#breadcrumb.reset(this.breadcrumbs());
      return res.data;
    },
  });
  readonly data = linkedSignal(() => {
    return this.result.value() ?? { ...initialRole };
  });
  readonly loading = linkedSignal(() => this.result.isLoading());

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
            title: 'Yeni Rol',
            icon: 'bi-plus',
            url: '/roles/add',
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());
      }
    });
  }

  save(form: NgForm) {
    if (!form.valid) return;

    if (!this.id()) {
      this.loading.set(true);
      this.#http.post<string>(
        '/rent/roles',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'success');
          this.#router.navigateByUrl('/roles');
          this.loading.set(false);
        },
        () => {
          this.loading.set(false);
        }
      );
    } else {
      this.loading.set(true);
      this.#http.put<string>(
        '/rent/roles/',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'info');
          this.#router.navigateByUrl('/roles');
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
