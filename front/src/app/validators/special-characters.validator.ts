import {AbstractControl, ValidatorFn} from '@angular/forms';

export function specialCharactersValidator(): ValidatorFn {
  return (control: AbstractControl): { [key: string]: any } | null => {
    const specialCharacters = /[!@#$%^&*(),.?":{}|<>]/;
    if (control.value && !specialCharacters.test(control.value)) {
      return {'specialCharacters': true};
    }
    return null;
  };
}
