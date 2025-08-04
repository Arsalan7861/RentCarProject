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
  initialUser,
  UserModel,
} from 'libraries/shared/src/lib/models/user.model';
import { httpResource } from '@angular/common/http';
import { ODataModel } from 'libraries/shared/src/lib/models/odata.model';
import { BranchModel } from 'libraries/shared/src/lib/models/branch.model';
import { RoleModel } from 'libraries/shared/src/lib/models/role.model';
import { FlexiSelectModule } from 'flexi-select';
import { Common } from 'apps/admin/src/services/common';

@Component({
  imports: [
    Blank,
    FormsModule,
    FormValidateDirective,
    NgClass,
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
      title: 'Kullanıcılar',
      icon: 'bi-people',
      url: '/users',
    },
  ]);
  readonly pageTitle = computed(() => {
    return this.id() ? 'Kullanıcı Güncelle' : 'Yeni Kullanıcı';
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
        this.#http.getResource<UserModel>(`/rent/users/${this.id()}`)
      );

      this.breadcrumbs.update((prev) => [
        ...prev,
        {
          title: res.data!.fullName,
          icon: 'bi-pen',
          url: `/users/edit/${this.id()}`,
          isActive: true,
        },
      ]);
      this.#breadcrumb.reset(this.breadcrumbs());
      return res.data;
    },
  });
  readonly data = linkedSignal(() => {
    return this.result.value() ?? { ...initialUser };
  });
  readonly loading = linkedSignal(() => this.result.isLoading());

  readonly branchResult = httpResource<ODataModel<BranchModel>>(
    () => '/rent/odata/branches'
  );
  readonly branches = computed(() => {
    return this.branchResult.value()?.value ?? [];
  });
  readonly branchLoading = computed(() => this.branchResult.isLoading());

  readonly roleResult = httpResource<ODataModel<RoleModel>>(
    () => '/rent/odata/roles'
  );
  readonly roles = computed(() => {
    if (this.checkIsAdmin()) {
      return this.roleResult.value()?.value ?? [];
    }
    return (
      this.roleResult
        .value()
        ?.value.filter((role) => role.name !== 'sys_admin') ?? []
    );
  });
  readonly roleLoading = computed(() => this.roleResult.isLoading());

  readonly #breadcrumb = inject(BreadcrumbService);
  readonly #activated = inject(ActivatedRoute);
  readonly #http = inject(HttpService);
  readonly #toast = inject(FlexiToastService);
  readonly #router = inject(Router);
  readonly #common = inject(Common);

  constructor() {
    this.#activated.params.subscribe((res) => {
      if (res['id']) {
        this.id.set(res['id']);
      } else {
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: 'Yeni Kullanıcı',
            icon: 'bi-plus',
            url: '/users/add',
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
        '/rent/users',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'success');
          this.#router.navigateByUrl('/users');
          this.loading.set(false);
        },
        () => {
          this.loading.set(false);
        }
      );
    } else {
      this.loading.set(true);
      this.#http.put<string>(
        '/rent/users/',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'info');
          this.#router.navigateByUrl('/users');
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

  checkIsAdmin() {
    return this.#common.decode().role === 'sys_admin';
  }
}
