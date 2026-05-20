import { Component, signal, WritableSignal } from "@angular/core";
import { FormControl, FormGroup, FormSubmittedEvent, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { Title } from "../../ui/title";
import { Logo } from "../../ui/logo";
import { AuthStore } from "../stores/auth.store";
import { ToastService } from "../../core/toast/services/toast.service";
import { firstValueFrom } from "rxjs";

@Component({
  selector: 'app-login',
  templateUrl: './login.template.html',
  styleUrl: './login.styles.css',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    Title,
    Logo,
    RouterModule,
  ]
})
export class LoginComponent {
  public errorMessage: WritableSignal<string | null> = signal(null);

  constructor(
    private readonly authStore: AuthStore,
    private readonly toastService: ToastService
  ) { }

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  async cleanMessages() {
    if(this.errorMessage() === null) return;

    this.errorMessage.set(null);
  }

  async onSubmit() {
    const request = {
      email: this.loginForm.value.email ?? '',
      password: this.loginForm.value.password ?? '',
    };
    
    this.authStore.login(request)
      .subscribe({
        next: (response) => {
          console.log('Login successful:', response);
        },
        error: (err) => {
          this.errorMessage.set(err.message);
          this.toastService.hide();
        },
        complete: () => {
          console.log('Login request completed');
        }
      });
    
    this.toastService.show({
      message: '',
      variant: 'surface',
      loading: await firstValueFrom(this.authStore.loading$)
    })
  }
}
