import { Component } from "@angular/core";
import { FormControl, FormGroup, FormSubmittedEvent, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { Title } from "../../ui/title";
import { Logo } from "../../ui/logo";
import { AuthStore } from "../stores/auth.store";

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
  constructor(
    private readonly authStore: AuthStore
  ) { }

  loginForm = new FormGroup({
    email: new FormControl(''),
    password: new FormControl(''),
  });

  onSubmit($event: FormSubmittedEvent) {
    const request = {
      email: this.loginForm.value.email ?? '',
      password: this.loginForm.value.password ?? '',
    }
    this.authStore.login(request)
    .subscribe({
      next: (response) => {
        console.log('Login successful:', response);
      },
      error: (err) => {
        console.error('Login failed:', err);
      },
      complete: () => {
        console.log('Login request completed');
      }
    });
  }
}
