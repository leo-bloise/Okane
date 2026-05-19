import { HTTP_INTERCEPTORS, provideHttpClient, withFetch, withInterceptorsFromDi } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { ErrorInterceptor } from "./interceptors/error.interceptor";
import { ToastComponent } from "./toast/toast.component";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatButton } from "@angular/material/button";

@NgModule({
  imports: [
    MatProgressSpinnerModule,
    MatButton
  ], 
  declarations: [
    ToastComponent
  ],
  providers: [
    provideHttpClient(
      withFetch(),
      withInterceptorsFromDi()
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    }
  ],
  exports: [
    ToastComponent
  ]
})
export class CoreModule { }
