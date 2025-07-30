/* eslint-disable @nx/enforce-module-boundaries */
import { DatePipe, NgClass, NgTemplateOutlet } from '@angular/common';
import { httpResource } from '@angular/common/http';
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
import { BranchModel } from 'apps/admin/src/models/branch.model';
import { CategoryModel } from 'apps/admin/src/models/category.model';
import {
  CustomerModel,
  initialCustomerModel,
} from 'apps/admin/src/models/customer.model';
import { ODataModel } from 'apps/admin/src/models/odata.model';
import {
  initialReservation,
  ReservationModel,
} from 'apps/admin/src/models/reservation.model';
import { VehicleModel } from 'apps/admin/src/models/vehicle.model';
import {
  BreadcrumbModel,
  BreadcrumbService,
} from 'apps/admin/src/services/breadcrumb';
import { Common } from 'apps/admin/src/services/common';
import { HttpService } from 'apps/admin/src/services/http';
import { FlexiGridModule, FlexiGridService, StateModel } from 'flexi-grid';
import { FlexiPopupModule } from 'flexi-popup';
import { FlexiSelectModule } from 'flexi-select';
import { FlexiToastService } from 'flexi-toast';
import { FormValidateDirective } from 'form-validate-angular';
import { NgxMaskDirective, NgxMaskPipe } from 'ngx-mask';
import { lastValueFrom } from 'rxjs';
import { TrCurrencyPipe } from 'tr-currency';
import { fuelTypeList, transmissionList } from '../../vehicles/create/create';
import { VehiclePipe } from 'apps/admin/src/pipes/vehicle-pipe';
import { ProtectionPackageModel } from 'apps/admin/src/models/protection-package.model';

@Component({
  imports: [
    Blank,
    FormsModule,
    FormValidateDirective,
    NgxMaskDirective,
    NgClass,
    FlexiPopupModule,
    FlexiGridModule,
    NgxMaskPipe,
    NgTemplateOutlet,
    FlexiSelectModule,
    DatePipe,
    TrCurrencyPipe,
    VehiclePipe,
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
      title: 'Rezervasyonlar',
      icon: 'bi-calendar-check',
      url: '/reservations',
    },
  ]);
  readonly pageTitle = computed(() =>
    this.id() ? 'Rezervasyon Güncelle' : 'Yeni Rezervasyon'
  );
  readonly pageIcon = computed(() =>
    this.id() ? 'bi-pencil-square' : 'bi-plus'
  );
  readonly btnName = computed(() => (this.id() ? 'Güncelle' : 'Ekle'));

  readonly result = resource({
    params: () => this.id(),
    loader: async () => {
      if (!this.id()) return null;
      const res = await lastValueFrom(
        this.#http.getResource<ReservationModel>(
          `/rent/reservations/${this.id()}`
        )
      );
      this.breadcrumbs.update((prev) => [
        ...prev,
        {
          title: res.data!.customer.fullName,
          icon: 'bi-pen',
          url: `/reservations/edit/${this.id()}`,
          isActive: true,
        },
      ]);
      this.#breadcrumb.reset(this.breadcrumbs());
      return res.data;
    },
  });
  readonly data = linkedSignal(() => {
    return this.result.value() ?? { ...initialReservation };
  });
  readonly loading = linkedSignal(() => this.result.isLoading());
  isCustomerPopupVisible = false;
  readonly isCustomerPopupLoading = signal<boolean>(false);
  readonly customerPopupData = signal<CustomerModel>({
    ...initialCustomerModel,
  });
  readonly customerState = signal<StateModel>(new StateModel());
  readonly customerResult = httpResource<ODataModel<CustomerModel>>(() => {
    let endpoint = '/rent/odata/customers?count=true&';
    const part = this.#grid.getODataEndpoint(this.customerState());
    endpoint += part;
    return endpoint;
  });
  readonly customersData = computed(
    () => this.customerResult.value()?.value ?? []
  );
  readonly customersTotal = computed(
    () => this.customerResult.value()?.['@odata.count'] ?? 0
  );
  readonly customersLoading = computed(() => this.customerResult.isLoading());
  readonly selectedCustomer = signal<CustomerModel | undefined>(undefined);

  readonly branchResult = httpResource<ODataModel<BranchModel>>(
    () => '/rent/odata/branches'
  );
  readonly branchesData = computed(
    () => this.branchResult.value()?.value ?? []
  );
  readonly branchesLoading = computed(() => this.branchResult.isLoading());
  readonly isAdmin = computed(() => this.#common.decode().role === 'sys_admin');
  readonly timeData = signal<string[]>(
    Array.from({ length: 31 }, (_, i) => {
      const hour = 9 + Math.floor(i / 2);
      const min = i % 2 === 0 ? '00' : '30';
      return `${hour.toString().padStart(2, '0')}:${min}`;
    })
  );
  readonly branchName = linkedSignal(() => this.#common.decode().branch);

  readonly vehicles = signal<VehicleModel[]>([]);
  readonly vehiclesLoading = signal<boolean>(false);

  readonly categoryResult = httpResource<ODataModel<CategoryModel>>(
    () => '/rent/odata/categories'
  );
  readonly categoriesData = computed(
    () => this.categoryResult.value()?.value ?? []
  );
  readonly categoriesLoading = computed(() => this.categoryResult.isLoading());

  readonly fuelTypeList = computed(() => fuelTypeList);
  readonly transmissionList = computed(() => transmissionList);

  readonly vehicleFilter = signal<{
    categoryName: string;
    fuelType: string;
    transmission: string;
  }>({
    categoryName: '',
    fuelType: '',
    transmission: '',
  });
  readonly selectedVehicle = signal<VehicleModel | undefined>(undefined);

  readonly protectionPackageResult = httpResource<
    ODataModel<ProtectionPackageModel>
  >(() => '/rent/odata/protection-packages?&$orderby=OrderNumber');
  readonly protectionPackagesData = computed(
    () => this.protectionPackageResult.value()?.value ?? []
  );
  readonly protectionPackagesLoading = computed(() =>
    this.protectionPackageResult.isLoading()
  );

  readonly #breadcrumb = inject(BreadcrumbService);
  readonly #activated = inject(ActivatedRoute);
  readonly #http = inject(HttpService);
  readonly #toast = inject(FlexiToastService);
  readonly #router = inject(Router);
  readonly #date = inject(DatePipe);
  readonly #grid = inject(FlexiGridService);
  readonly #common = inject(Common);

  constructor() {
    this.#activated.params.subscribe((res) => {
      if (res['id']) {
        this.id.set(res['id']);
      } else {
        this.breadcrumbs.update((prev) => [
          ...prev,
          {
            title: 'Ekle',
            icon: 'bi-plus',
            url: '/reservations/add',
            isActive: true,
          },
        ]);
        this.#breadcrumb.reset(this.breadcrumbs());

        const date = this.#date.transform(new Date(), 'yyyy-MM-dd');
        this.customerPopupData.update((prev) => ({
          ...prev,
          birthDate: date!,
          drivingLicenseIssuanceDate: date!,
        }));

        const now = this.#date.transform(new Date(), 'yyyy-MM-dd')!;
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        const tomorrowDate = this.#date.transform(tomorrow, 'yyyy-MM-dd')!;

        this.data.update((prev) => ({
          ...prev,
          pickUpDate: now,
          deliveryDate: tomorrowDate,
        }));
      }
    });
    this.calculateDayDifference();
  }

  save(form: NgForm) {
    if (!form.valid) return;
    this.loading.set(true);
    if (!this.id()) {
      this.#http.post<string>(
        '/rent/reservations',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'success');
          this.#router.navigateByUrl('/reservations');
          this.loading.set(false);
        },
        () => this.loading.set(false)
      );
    } else {
      this.#http.put<string>(
        '/rent/reservations',
        this.data(),
        (res) => {
          this.#toast.showToast('Başarılı', res, 'info');
          this.#router.navigateByUrl('/reservations');
          this.loading.set(false);
        },
        () => this.loading.set(false)
      );
    }
  }

  saveCustomer(customerForm: NgForm) {
    if (!customerForm.valid) return;

    this.loading.set(true);

    this.#http.post<string>(
      '/rent/customers',
      this.customerPopupData(),
      (res) => {
        this.#toast.showToast('Başarılı', res, 'success');
        this.loading.set(false);
      },
      () => {
        this.loading.set(false);
      }
    );
  }

  customersDataStateChange(state: StateModel) {
    this.customerState.set(state);
  }

  selectCustomer(item: CustomerModel) {
    this.selectedCustomer.set(item);
    this.data.update((prev) => ({
      ...prev,
      customerId: item.id,
    }));
  }

  clearCustomer() {
    this.selectedCustomer.set(undefined);
    this.data.update((prev) => ({
      ...prev,
      customerId: '',
    }));
  }

  calculateDayDifference() {
    this.vehicles.set([]);
    const pickUpDateTime = new Date(
      `${this.data().pickUpDate}T${this.data().pickUpTime}`
    );
    const deliveryDateTime = new Date(
      `${this.data().deliveryDate}T${this.data().deliveryTime}`
    );

    const diffMs = deliveryDateTime.getTime() - pickUpDateTime.getTime();

    if (diffMs <= 0) {
      this.data.update((prev) => ({ ...prev, totalDay: 0 }));
      return;
    }

    const oneDayMs = 24 * 60 * 60 * 1000;

    const fullDays = Math.floor(diffMs / oneDayMs);
    const remainder = diffMs % oneDayMs;

    const totalDay = remainder > 0 ? fullDays + 1 : fullDays;
    this.data.update((prev) => ({ ...prev, totalDay: totalDay }));
  }

  setLocation(id: any) {
    const branch = this.branchesData().find((b) => b.id === id)!;
    this.branchName.set(branch.name);
  }

  getVehicles() {
    const data = {
      branchId: !this.data().pickUpLocationId
        ? this.#common.decode().branchId
        : this.data().pickUpLocationId,
      pickUpDate: this.data().pickUpDate,
      pickUpTime: this.data().pickUpTime,
      deliveryDate: this.data().deliveryDate,
      deliveryTime: this.data().deliveryTime,
    };
    this.vehiclesLoading.set(true);
    this.#http.post<VehicleModel[]>(
      '/rent/reservations/vehicle-getall',
      data,
      (res) => {
        this.vehicles.set(res);
        this.vehiclesLoading.set(false);
      },
      () => {
        this.vehiclesLoading.set(false);
      }
    );
  }

  getVehicleImage(vehicle: VehicleModel) {
    const endpoint = 'https://localhost:7033/images/';
    return endpoint + vehicle.imageUrl;
  }

  selectVehicle(vehicle: VehicleModel) {
    this.selectedVehicle.set(vehicle);
    this.data.update((prev) => ({
      ...prev,
      vehicleId: vehicle.id,
      vehicleDailyPrice: vehicle.dailyPrice,
      vehicle: vehicle,
    }));
    this.calculateTotal();
  }

  selectProtectionPackage(val: ProtectionPackageModel) {
    if (this.data().protectionPackageId === val.id) {
      this.data.update((prev) => ({
        ...prev,
        protectionPackageId: '',
        protectionPackagePrice: 0,
        protectionPackageName: '',
      }));
    } else {
      this.data.update((prev) => ({
        ...prev,
        protectionPackageId: val.id,
        protectionPackagePrice: val.price,
        protectionPackageName: val.name,
      }));
    }
    this.calculateTotal();
  }

  calculateTotal() {
    const totalVehicle = this.data().vehicleDailyPrice * this.data().totalDay;
    const totalProtectionPackage =
      this.data().protectionPackagePrice * this.data().totalDay;
    const total = totalVehicle + totalProtectionPackage;
    this.data.update((prev) => ({
      ...prev,
      total: total,
    }));
  }
}
