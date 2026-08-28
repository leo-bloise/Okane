import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Auth } from '../../../core/services/auth';
import { ApiErrorResponse } from '../../../core/models/api-response.model';
import { RegisteredUser } from '../../../core/models/auth.model';

function passwordsMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;

  return password === confirmPassword ? null : { passwordsMismatch: true };
}

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-register',
  styleUrl: './register.css',
  templateUrl: './register.html',
})
export class Register {
  private readonly authService = inject(Auth);
  private readonly formBuilder = inject(FormBuilder);

  protected readonly submitting = signal(false);
  protected readonly formError = signal<string | null>(null);
  protected readonly registeredUser = signal<RegisteredUser | null>(null);

  protected readonly form = this.formBuilder.nonNullable.group(
    {
      name: ['', [Validators.required, Validators.maxLength(200)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordsMatchValidator },
  );

  protected submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.formError.set(null);

    const { name, email, password } = this.form.getRawValue();

    this.authService.register({ name, email, password }).subscribe({
      next: (response) => {
        this.submitting.set(false);
        this.registeredUser.set(response.details ?? null);
        this.form.reset();
      },
      error: (error: HttpErrorResponse) => {
        this.submitting.set(false);
        this.applyServerErrors(error);
      },
    });
  }

  private applyServerErrors(error: HttpErrorResponse): void {
    if (error.status === 0) {
      this.formError.set('Unable to reach the server. Please check your connection and try again.');
      return;
    }

    const body = error.error as ApiErrorResponse | undefined;
    const details = body?.details;

    if (details) {
      for (const [field, messages] of Object.entries(details)) {
        const control = this.form.get(field.toLowerCase());
        if (control && messages.length > 0) {
          control.setErrors({ server: messages[0] });
        }
      }
    }

    this.formError.set(body?.message ?? 'Something went wrong. Please try again.');
  }
}
