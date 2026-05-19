import { Component } from "@angular/core";
import { Logo } from "../../ui/logo";
import { Title } from "../../ui/title";
import { ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { RouterModule } from "@angular/router";

@Component({
  standalone: true,
  templateUrl: './register.template.html',
  styleUrl: './register.styles.css',
  imports: [
    Logo,
    Title,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    RouterModule
  ]
})
export class RegisterComponent { }
