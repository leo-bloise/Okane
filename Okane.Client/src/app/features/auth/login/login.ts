import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Auth } from '../../../core/services/auth';
import { ApiErrorResponse } from '../../../core/models/api-response.model';

@Component({
  imports: [ReactiveFormsModule, RouterLink],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {
  private readonly authService = inject(Auth);
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);

  protected readonly submitting = signal(false);
  protected readonly formError = signal<string | null>(null);

  protected readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  protected submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.formError.set(null);

    const { email, password } = this.form.getRawValue();

    this.authService.login({ email, password }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.router.navigateByUrl('/app/ledger');
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
