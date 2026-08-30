import { HttpErrorResponse } from '@angular/common/http';
import { Component, effect, inject, input, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiErrorResponse } from '../../../core/models/api-response.model';
import { Wallets } from '../../../core/services/wallets';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-create-wallet-drawer',
  styleUrl: './create-wallet-drawer.css',
  templateUrl: './create-wallet-drawer.html',
})
export class CreateWalletDrawer {
  private readonly walletsService = inject(Wallets);
  private readonly formBuilder = inject(FormBuilder);

  readonly open = input.required<boolean>();

  readonly created = output<void>();
  readonly closed = output<void>();

  protected readonly submitting = signal(false);
  protected readonly formError = signal<string | null>(null);

  protected readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required]],
  });

  private readonly resetOnClose = effect(() => {
    if (!this.open()) {
      this.form.reset({ name: '' });
      this.formError.set(null);
      this.submitting.set(false);
    }
  });

  protected submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.formError.set(null);

    const { name } = this.form.getRawValue();

    this.walletsService.createWallet({ name }).subscribe({
      next: () => {
        this.submitting.set(false);
        this.created.emit();
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
        const control = this.form.get(field.charAt(0).toLowerCase() + field.slice(1));
        if (control && messages.length > 0) {
          control.setErrors({ server: messages[0] });
        }
      }
    }

    this.formError.set(body?.message ?? 'Something went wrong. Please try again.');
  }
}
