import { Component, Input } from "@angular/core";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
    standalone: true,
    imports: [
        MatProgressSpinnerModule
    ],
    selector: 'app-toast',    
    template: `
        <div
            class="toast"
            [class]="variant"
            [class.sm]="size === 'sm'"
            [class.md]="size === 'md'"
            [class.lg]="size === 'lg'"
        >
            @if(loading) {
                <mat-spinner
                    [diameter]="spinnerDiameter"
                    [class]="'spinner-' + variant">
                </mat-spinner>
            } @else if(icon) {
                <span class="material-symbols-outlined icon"
                    [class.icon-sm]="size === 'sm'"
                    [class.icon-md]="size === 'md'"
                    [class.icon-lg]="size === 'lg'">
                    {{ icon }}
                </span>
            }

            <p>{{ message }}</p>
        </div>
    `,

    styles: [`
        :host {
            width: 100vw;
            height: 100vh;

            position: fixed;

            top: 0;
            left: 0;

            background-color: rgba(0, 0, 0, .4);

            z-index: 1000;

            display: flex;
            align-items: center;
            justify-content: center;

            opacity: 0;
            transition: opacity .25s ease;
        }

        :host(.visible) {
            opacity: 1;
        }

        .toast {
            border-radius: 1rem;

            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 1rem;

            padding: 1.5rem;

            box-sizing: border-box;

            box-shadow: 0 4px 12px rgba(0, 0, 0, .15);

            font-family: inherit;

            transform: translateY(12px);
            transition:
                transform .25s ease,
                opacity .25s ease;
        }

        :host(.visible) .toast {
            transform: translateY(0);
        }

        .toast p {
            margin: 0;
        }

        .material-symbols-outlined.icon {
            line-height: 1;
        }

        .icon-sm { font-size: 1.5rem; }
        .icon-md { font-size: 2rem; }
        .icon-lg { font-size: 2.75rem; }

        .toast.info {
            background-color: var(--mat-sys-primary);
            color: var(--mat-sys-on-primary);
        }

        .toast.error {
            background-color: var(--mat-sys-error);
            color: var(--mat-sys-on-error);
        }

        .toast.surface {
            background-color: var(--mat-sys-surface);
            color: var(--mat-sys-on-surface);
            border: 1px solid var(--mat-sys-outline);
        }

        .toast.sm { width: 240px; min-height: 80px; }
        .toast.md { width: 320px; min-height: 120px; }
        .toast.lg { width: 420px; min-height: 160px; }

        .spinner-info    circle { stroke: var(--mat-sys-on-primary); }
        .spinner-error   circle { stroke: var(--mat-sys-on-error); }
        .spinner-surface circle { stroke: var(--mat-sys-primary); }
    `]
})
export class ToastComponent {

    @Input() message = 'Toast message';
    @Input() variant: 'info' | 'error' | 'surface' = 'surface';
    @Input() size: 'sm' | 'md' | 'lg' = 'md';
    @Input() icon:
        | 'check_circle'
        | 'error'
        | 'info'
        | 'warning'
        | 'account_balance_wallet'
        | 'payments'
        | 'savings'
        | 'close'
        | null = null;
    @Input() loading = false;

    get spinnerDiameter(): number {
        return { sm: 24, md: 32, lg: 44 }[this.size];
    }
}