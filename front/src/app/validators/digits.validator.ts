import { AbstractControl, ValidatorFn } from '@angular/forms';

export function digitsValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const hasDigits = /[0-9]/.test(control.value);
    if (control.value && !hasDigits) {
      return { 'digits': true };
    }
    return null;
  };
}
