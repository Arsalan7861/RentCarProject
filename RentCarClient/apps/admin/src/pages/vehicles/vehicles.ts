import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  signal,
  ViewEncapsulation,
} from '@angular/core';
import { FlexiGridFilterDataModel, FlexiGridModule } from 'flexi-grid';
import Grid from '../../components/grid/grid';
import { BreadcrumbModel } from '../../services/breadcrumb';
import { Common } from '../../services/common';
import {
  brandList,
  colorList,
  fuelTypeList,
  modelYearList,
  transmissionList,
} from './create/create';
import { httpResource } from '@angular/common/http';
import { CategoryModel } from '../../../../../libraries/shared/src/lib/models/category.model';
import { BranchModel } from '../../../../../libraries/shared/src/lib/models/branch.model';
import { ODataModel } from '../../../../../libraries/shared/src/lib/models/odata.model';

@Component({
  imports: [Grid, FlexiGridModule],
  templateUrl: './vehicles.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Vehicles {
  readonly bredcrumbs = signal<BreadcrumbModel[]>([
    {
      title: 'Araçlar',
      icon: 'bi-car-front',
      url: '/vehicles',
      isActive: true,
    },
  ]);
  readonly brandFilterData = computed<FlexiGridFilterDataModel[]>(() =>
    brandList.map((val) => ({
      value: val,
      name: val,
    }))
  );
  readonly modelYearFilterData = computed<FlexiGridFilterDataModel[]>(() =>
    modelYearList.map((val) => ({
      value: val,
      name: val,
    }))
  );
  readonly colorFilterData = computed<FlexiGridFilterDataModel[]>(() =>
    colorList.map((val) => ({
      value: val,
      name: val,
    }))
  );
  readonly fuelTypeFilterData = computed<FlexiGridFilterDataModel[]>(() =>
    fuelTypeList.map((val) => ({
      value: val,
      name: val,
    }))
  );
  readonly transmissionFilterData = computed<FlexiGridFilterDataModel[]>(() =>
    transmissionList.map((val) => ({
      value: val,
      name: val,
    }))
  );

  readonly categoryResource = httpResource<ODataModel<CategoryModel>>(
    () => '/rent/odata/categories'
  );
  readonly categoryList = computed(
    () => this.categoryResource.value()?.value ?? []
  );
  readonly categoryFilterData = computed<FlexiGridFilterDataModel[]>(() =>
    this.categoryList().map((val) => ({
      value: val.name,
      name: val.name,
    }))
  );

  readonly branchResource = httpResource<ODataModel<BranchModel>>(
    () => '/rent/odata/branches'
  );
  readonly branchList = computed(
    () => this.branchResource.value()?.value ?? []
  );
  readonly branchFilterData = computed<FlexiGridFilterDataModel[]>(() =>
    this.branchList().map((val) => ({
      value: val.name,
      name: val.name,
    }))
  );

  readonly #common = inject(Common);

  checkPermission(permission: string) {
    return this.#common.checkPermission(permission);
  }
}
