import { Component, Input } from "@angular/core";

@Component({
  standalone: true,
  template: `<h1 class="title"><ng-content></ng-content></h1>`,
  styles: [`
    .title {
      font-size: 1.6rem;
      font-weight: 500;
    }
  `],
  selector: 'app-title'
})
export class Title {
}
