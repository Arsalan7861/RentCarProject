import { Pipe, PipeTransform, inject } from '@angular/core';
import {
  TranslationService,
  Translations,
} from '../services/translation.service';

@Pipe({
  name: 'translate',
  pure: false, // Make it impure so it updates when language changes
  standalone: true,
})
export class TranslatePipe implements PipeTransform {
  readonly #translationService = inject(TranslationService);

  transform(key: keyof Translations): string {
    return this.#translationService.translate(key);
  }
}
