import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  ViewEncapsulation,
} from '@angular/core';
import { BreadcrumbService } from '../../services/breadcrumb';
import Blank from '../../components/blank/blank';

@Component({
  imports: [Blank],
  templateUrl: './dashboard.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Dashboard implements OnInit {
  readonly #breadcrumb = inject(BreadcrumbService);
  // readonly result = httpResource(() => '/rent/');
  // readonly result = resource({
  //   loader: async () => {
  //     const res = await lastValueFrom(this.#http.getResource('/rent/'));
  //     return res;
  //   },
  // });

  ngOnInit(): void {
    this.#breadcrumb.setDashboard();
  }

  // makeRequest() {
  //   this.result.reload();
  // }
}
