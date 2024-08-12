import {AbstractControl, ValidatorFn} from '@angular/forms';

export function lowercaseValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const hasLowercase = /[a-z]/.test(control.value);
    if (control.value && !hasLowercase) {
      return {'lowercase': true};
    }
    return null;
  };
}
