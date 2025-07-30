import { httpResource } from '@angular/common/http';
import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  computed,
  contentChild,
  contentChildren,
  inject,
  input,
  OnInit,
  signal,
  TemplateRef,
  ViewEncapsulation,
} from '@angular/core';
import {
  FlexiGridColumnComponent,
  FlexiGridModule,
  FlexiGridService,
  StateFilterModel,
  StateModel,
  StateSortModel,
} from 'flexi-grid';
import { ODataModel } from '../../models/odata.model';
import { RouterLink } from '@angular/router';
import { FlexiToastService } from 'flexi-toast';
import { HttpService } from '../../services/http';
import { BreadcrumbModel, BreadcrumbService } from '../../services/breadcrumb';
import { NgTemplateOutlet } from '@angular/common';
import { Common } from '../../services/common';

export interface BtnOptions {
  url: string;
  permission: string;
}

@Component({
  selector: 'app-grid',
  imports: [FlexiGridModule, RouterLink, NgTemplateOutlet],
  templateUrl: './grid.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Grid implements AfterViewInit, OnInit {
  readonly pageTitle = input.required<string>();
  readonly endpoint = input.required<string>();
  readonly showAudit = input<boolean>(true);
  readonly addOptions = input.required<BtnOptions>();
  readonly editOptions = input.required<BtnOptions>();
  readonly detailOptions = input.required<BtnOptions>();
  readonly deleteOptions = input.required<BtnOptions>();
  readonly breadcrumbs = input.required<BreadcrumbModel[]>();
  readonly commandColumnWidth = input<string>('130px');
  readonly showIndex = input<boolean>(true);
  readonly captionTitle = input.required<string>();
  readonly showIsActive = input<boolean>(true);
  readonly sort = input<StateSortModel>({ field: '', dir: 'asc' });
  readonly filter = input<StateFilterModel[]>([]);

  readonly columns = contentChildren(FlexiGridColumnComponent, {
    descendants: true,
  });
  readonly commandTemplateRef =
    contentChild<TemplateRef<any>>('commandTemplate');
  readonly columnCommandTemplateRef = contentChild<TemplateRef<any>>(
    'columnCommandTemplate'
  );

  readonly state = signal<StateModel>(new StateModel());
  readonly result = httpResource<ODataModel<any>>(() => {
    let enpoint = this.endpoint();
    if (enpoint.includes('?')) {
      enpoint += '&$count=true';
    } else {
      enpoint += '?$count=true';
    }
    const part = this.#grid.getODataEndpoint(this.state());
    enpoint += `&${part}`;

    return enpoint;
  });
  readonly data = computed(() => this.result.value()?.value ?? []);
  readonly total = computed(() => this.result.value()?.['@odata.count'] ?? 0);
  readonly loading = computed(() => this.result.isLoading());

  readonly #breadcrumb = inject(BreadcrumbService);
  readonly #grid = inject(FlexiGridService);
  readonly #toast = inject(FlexiToastService);
  readonly #http = inject(HttpService);
  readonly #common = inject(Common);

  ngOnInit(): void {
    this.state.update((prev) => ({
      ...prev,
      sort: this.sort(),
    }));
  }

  ngAfterViewInit(): void {
    this.#breadcrumb.reset(this.breadcrumbs());
  }

  dataStateChange(state: StateModel) {
    this.state.set(state);
  }

  delete(id: string) {
    this.#toast.showSwal(
      'Sil?',
      'Bu şubeyi silmek istediğinize emin misiniz?',
      'Sil',
      () => {
        this.#http.delete<string>(
          `${this.deleteOptions().url}/${id}`,
          (res) => {
            this.result.reload();
            this.#toast.showToast('Başarılı', res, 'info');
          }
        );
      }
    );
  }

  checkPermission(permission: string) {
    return this.#common.checkPermission(permission);
  }
}
