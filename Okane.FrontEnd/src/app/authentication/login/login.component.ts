import { Component } from "@angular/core";
import { FormControl, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { Title } from "../../ui/title";
import { Logo } from "../../ui/logo";

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
    RouterModule
  ]
})
export class LoginComponent {
  email = new FormControl('');
}
