import {AbstractControl, ValidatorFn} from '@angular/forms';

export function uppercaseValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const hasUppercase = /[A-Z]/.test(control.value);
    if (control.value && !hasUppercase) {
      return {'uppercase': true};
    }
    return null;
  };
}
