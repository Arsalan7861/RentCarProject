import { DatePipe, Location, NgClass } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  output,
  ViewEncapsulation,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { EntityModel } from '../../../../../libraries/shared/src/lib/models/entity.model';
import { FormsModule } from '@angular/forms';
import Loading from '../loading/loading';

@Component({
  selector: 'app-blank',
  imports: [NgClass, RouterLink, DatePipe, FormsModule, Loading],
  templateUrl: './blank.html',
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class Blank {
  readonly pageIcon = input.required<string>();
  readonly pageTitle = input.required<string>();
  readonly pageDescription = input<string>('');
  readonly status = input<boolean>(false);
  readonly showStatus = input<boolean>(false);
  readonly showStatusCheckbox = input<boolean>(true);
  readonly showBackBtn = input<boolean>(true);
  readonly showEditBtn = input<boolean>(false);
  readonly editBtnUrl = input<string>('');
  readonly audit = input<EntityModel>();
  readonly showAudit = input<boolean>(false);
  readonly loading = input<boolean>(false);
  readonly size = input<string>('col-md-12');

  readonly changeStatusEvent = output<boolean>();

  readonly #location = inject(Location);

  back() {
    this.#location.back();
  }

  changeStatus(event: any) {
    const checked = event.target.checked;
    this.changeStatusEvent.emit(checked);
  }
}
