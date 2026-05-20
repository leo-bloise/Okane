// toast.service.ts
import {
    ApplicationRef,
    ComponentRef,
    createComponent,
    EnvironmentInjector,
    Injectable,
} from "@angular/core";
import { ToastComponent } from "../toast.component";

export interface ToastOptions {
    message: string;
    variant?: ToastComponent["variant"];
    size?: ToastComponent["size"];
    icon?: ToastComponent["icon"];
    loading?: boolean;
    duration?: number;
}

const TRANSITION_MS = 250; // Must match the CSS transition duration

@Injectable({ providedIn: "root" })
export class ToastService {

    private componentRef: ComponentRef<ToastComponent> | null = null;
    private dismissTimer: ReturnType<typeof setTimeout> | null = null;

    constructor(
        private appRef: ApplicationRef,
        private injector: EnvironmentInjector,
    ) {}

    show(options: ToastOptions): void {
        this.destroyNow();

        const ref = createComponent(ToastComponent, {
            environmentInjector: this.injector,
        });

        ref.setInput("message", options.message);
        ref.setInput("variant", options.variant ?? "surface");
        ref.setInput("size",    options.size    ?? "md");
        ref.setInput("icon",    options.icon    ?? null);
        ref.setInput("loading", options.loading ?? false);

        this.appRef.attachView(ref.hostView);
        document.body.appendChild(ref.location.nativeElement);
        this.componentRef = ref;

        requestAnimationFrame(() => {
            ref.location.nativeElement.classList.add("visible");
        });

        if (options.duration) {
            this.dismissTimer = setTimeout(() => this.hide(), options.duration);
        }
    }

    update(options: Partial<ToastOptions>): void {
        if (!this.componentRef) return;

        if (options.message  !== undefined) this.componentRef.setInput("message",  options.message);
        if (options.variant  !== undefined) this.componentRef.setInput("variant",  options.variant);
        if (options.size     !== undefined) this.componentRef.setInput("size",      options.size);
        if (options.icon     !== undefined) this.componentRef.setInput("icon",      options.icon);
        if (options.loading  !== undefined) this.componentRef.setInput("loading",   options.loading);
    }

    hide(): void {
        this.clearDismissTimer();

        const ref = this.componentRef;
        if (!ref) return;

        this.componentRef = null;

        const el: HTMLElement = ref.location.nativeElement;

        el.classList.remove("visible");

        el.addEventListener(
            "transitionend",
            () => {
                this.appRef.detachView(ref.hostView);
                ref.destroy();
            },
            { once: true }
        );

        setTimeout(() => {
            if (!ref.hostView.destroyed) {
                this.appRef.detachView(ref.hostView);
                ref.destroy();
            }
        }, TRANSITION_MS + 50);
    }

    private clearDismissTimer(): void {
        if (this.dismissTimer) {
            clearTimeout(this.dismissTimer);
            this.dismissTimer = null;
        }
    }

    private destroyNow(): void {
        this.clearDismissTimer();

        if (this.componentRef) {
            this.appRef.detachView(this.componentRef.hostView);
            this.componentRef.destroy();
            this.componentRef = null;
        }
    }
}