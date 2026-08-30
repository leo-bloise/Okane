import { HttpErrorResponse } from '@angular/common/http';
import { Component, effect, inject, input, output, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ApiErrorResponse } from '../../../core/models/api-response.model';
import { Wallet } from '../../../core/models/wallet.model';
import { Ledger } from '../../../core/services/ledger';

function walletsDifferentValidator(control: AbstractControl): ValidationErrors | null {
  const fromWalletId = control.get('fromWalletId')?.value;
  const toWalletId = control.get('toWalletId')?.value;

  return !fromWalletId || !toWalletId || fromWalletId !== toWalletId ? null : { walletsMatch: true };
}

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-create-transaction-drawer',
  styleUrl: './create-transaction-drawer.css',
  templateUrl: './create-transaction-drawer.html',
})
export class CreateTransactionDrawer {
  private readonly ledgerService = inject(Ledger);
  private readonly formBuilder = inject(FormBuilder);

  readonly wallets = input.required<Wallet[]>();
  readonly open = input.required<boolean>();

  readonly created = output<void>();
  readonly closed = output<void>();

  protected readonly submitting = signal(false);
  protected readonly formError = signal<string | null>(null);

  protected readonly form = this.formBuilder.nonNullable.group(
    {
      fromWalletId: ['', [Validators.required]],
      toWalletId: ['', [Validators.required]],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      description: [''],
    },
    { validators: walletsDifferentValidator },
  );

  private readonly resetOnClose = effect(() => {
    if (!this.open()) {
      this.form.reset({ fromWalletId: '', toWalletId: '', amount: 0, description: '' });
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

    const { fromWalletId, toWalletId, amount, description } = this.form.getRawValue();

    this.ledgerService
      .createTransaction({ fromWalletId, toWalletId, amount, description: description || undefined })
      .subscribe({
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
