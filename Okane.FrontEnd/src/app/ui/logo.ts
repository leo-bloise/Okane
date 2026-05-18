import { CommonModule } from "@angular/common";
import { Component, Input } from "@angular/core";

@Component({
  standalone: true,
  selector: 'app-logo',
  imports: [CommonModule],
  template: `
    <svg
      [attr.width]="width"
      [attr.height]="height"
      viewBox="0 0 400 500"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
      <!-- Background -->
      <rect
        *ngIf="!removeBackground"
        width="400"
        height="500"
        [attr.fill]="backgroundColor"
      />

      <!-- Outer ring -->
      <circle
        cx="200"
        cy="150"
        r="70"
        [attr.stroke]="primaryColor"
        stroke-width="28"
        fill="none"
        stroke-dasharray="400"
        stroke-dashoffset="20"
      />

      <!-- Inner ring -->
      <circle
        cx="200"
        cy="150"
        r="38"
        [attr.stroke]="primaryColor"
        stroke-width="28"
        fill="none"
        stroke-dasharray="220"
        stroke-dashoffset="10"
      />

      <!-- Text -->
      <text
        x="200"
        y="340"
        text-anchor="middle"
        [attr.fill]="primaryColor"
        font-size="72"
        font-family="Poppins, Arial, sans-serif"
        font-weight="300"
        letter-spacing="1"
      >
        Okane
      </text>
    </svg>
  `
})
export class Logo {
  @Input() primaryColor: string = "white";
  @Input() backgroundColor: string = "black";

  @Input() removeBackground: boolean = false;

  @Input() width: number | string = 400;
  @Input() height: number | string = 500;
}
